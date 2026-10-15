using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http;
using System.Reflection;
using System.Text;
using LogicPOS.Core.Updates;

namespace LogicPOS.App;

/// <summary>
/// Downloads the published zip and replaces program files after this process exits.
/// Local settings and database files are never replaced.
/// </summary>
internal static class ApplicationUpdater
{
    public const string PackageUrl = "https://box.logicpulse.com/files/logicPOS/Installation.Windows/Avalonia/logicpos.zip";

    public const string VersionUrl = "https://box.logicpulse.com/files/logicPOS/Installation.Windows/Avalonia/version.txt";

    private const string TempFolderPrefix = "logicpos-update-";

    public static async Task DownloadAndScheduleAsync(IProgress<string> progress, CancellationToken cancellationToken = default)
    {
        // Drop leftovers from failed/previous updates before allocating a new unique temp dir.
        PurgeStaleUpdateFolders();

        var installed = ReadInstalledVersion();
        var published = await TryReadPublishedVersionAsync(cancellationToken);
        if (published is not null && ApplicationVersionComparer.ParseRelease(published) is not null
            && ApplicationVersionComparer.IsNewer(published, installed) == false)
        {
            throw AlreadyCurrent(installed);
        }

        // No manifest: skip a repeat download of a package we already proved is not newer.
        if (published is null)
        {
            var remoteLength = await TryGetPackageLengthAsync(cancellationToken);
            if (IsKnownCurrentPackage(remoteLength, installed))
            {
                throw AlreadyCurrent(installed);
            }
        }

        var install = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var tempRoot = Path.Combine(Path.GetTempPath(), TempFolderPrefix + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        var zipPath = Path.Combine(tempRoot, "logicpos.zip");
        var extractPath = Path.Combine(tempRoot, "files");

        try
        {
            progress.Report("A descarregar o pacote...");
            await DownloadAsync(zipPath, progress, cancellationToken);
            var packageLength = new FileInfo(zipPath).Length;

            // Extract/copy prep must not run on the UI sync context — a large zip freezes the
            // progress UI at 100% and delays Shutdown so the apply script never sees process exit.
            progress.Report("A extrair ficheiros...");
            var payload = await Task.Run(() =>
            {
                ExtractZipWithRetry(zipPath, extractPath, cancellationToken);

                // Zip is no longer needed; free space before apply runs.
                TryDeleteFile(zipPath);

                var found = FindPayload(extractPath)
                    ?? throw new InvalidOperationException("O pacote não contém logicpos.exe.");

                // Never ship install-time defaults over a live cloud/local configuration.
                StripProtectedFiles(found);
                return found;
            }, cancellationToken);

            var packageVersion = ReadPackageVersion(payload);
            if (packageVersion is not null && ApplicationVersionComparer.IsNewer(packageVersion, installed) == false)
            {
                // Same release (1.6.33, 1.6.33.0, 1.6.33+develop[.sha]) must not restart the download.
                RememberPackage(packageVersion, packageLength);
                throw AlreadyCurrent(installed);
            }

            progress.Report("A preparar a aplicação dos ficheiros...");
            var scriptPath = Path.Combine(tempRoot, "apply.ps1");
            // Windows PowerShell 5.1 reads a BOM-less UTF-8 script as the ANSI code page.
            await File.WriteAllTextAsync(
                scriptPath,
                BuildScript(Environment.ProcessId, payload, install, tempRoot),
                new UTF8Encoding(encoderShouldEmitUTF8Identifier: true),
                cancellationToken);
            StartScript(scriptPath);
            progress.Report("A fechar para aplicar a atualização (janela de progresso a abrir)...");
        }
        catch
        {
            TryDeleteDirectory(tempRoot);
            throw;
        }
    }

    /// <summary>
    /// Removes leftover logicpos-update-* folders under Local Temp (best-effort).
    /// </summary>
    internal static void PurgeStaleUpdateFolders(string? keepTempRoot = null)
    {
        string keepFull = string.Empty;
        if (!string.IsNullOrWhiteSpace(keepTempRoot))
        {
            try
            {
                keepFull = Path.GetFullPath(keepTempRoot);
            }
            catch
            {
                keepFull = string.Empty;
            }
        }

        string[] dirs;
        try
        {
            dirs = Directory.GetDirectories(Path.GetTempPath(), TempFolderPrefix + "*");
        }
        catch
        {
            return;
        }

        foreach (var dir in dirs)
        {
            if (keepFull.Length > 0)
            {
                try
                {
                    if (string.Equals(Path.GetFullPath(dir), keepFull, StringComparison.OrdinalIgnoreCase))
                    {
                        continue;
                    }
                }
                catch
                {
                    // Fall through and attempt delete.
                }
            }

            TryDeleteDirectory(dir);
        }
    }

    internal static string ReadInstalledVersion()
    {
        var informational = typeof(ApplicationUpdater).Assembly
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion;
        if (string.IsNullOrWhiteSpace(informational) == false)
        {
            return informational;
        }

        return typeof(ApplicationUpdater).Assembly.GetName().Version?.ToString() ?? "0.0.0";
    }

    /// <summary>
    /// Product version from the payload's LogicPOS.App.dll, without loading the assembly.
    /// </summary>
    internal static string? ReadPackageVersion(string payloadRoot)
    {
        var dll = Path.Combine(payloadRoot, "LogicPOS.App.dll");
        if (File.Exists(dll) == false)
        {
            dll = Directory.EnumerateFiles(payloadRoot, "LogicPOS.App.dll", SearchOption.AllDirectories).FirstOrDefault()
                ?? string.Empty;
        }

        if (dll.Length == 0 || File.Exists(dll) == false)
        {
            return null;
        }

        var info = FileVersionInfo.GetVersionInfo(dll);
        if (string.IsNullOrWhiteSpace(info.ProductVersion) == false)
        {
            return info.ProductVersion.Trim().TrimEnd('\0');
        }

        return string.IsNullOrWhiteSpace(info.FileVersion) ? null : info.FileVersion.Trim().TrimEnd('\0');
    }

    private static InvalidOperationException AlreadyCurrent(string installed)
        => new($"A aplicação já está na versão {ApplicationVersionComparer.Display(installed)}.");

    private static async Task<string?> TryReadPublishedVersionAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("logicpos-updater");
            using var response = await client.GetAsync(VersionUrl, cancellationToken);
            if (response.IsSuccessStatusCode == false)
            {
                return null;
            }

            var body = await response.Content.ReadAsStringAsync(cancellationToken);
            var line = body.Replace("\r", string.Empty).Split('\n')[0].Trim().TrimStart('\uFEFF');
            return string.IsNullOrWhiteSpace(line) ? null : line;
        }
        catch (Exception) when (cancellationToken.IsCancellationRequested == false)
        {
            return null;
        }
    }

    private static async Task<long?> TryGetPackageLengthAsync(CancellationToken cancellationToken)
    {
        try
        {
            using var client = new HttpClient { Timeout = TimeSpan.FromSeconds(20) };
            client.DefaultRequestHeaders.UserAgent.ParseAdd("logicpos-updater");
            using var request = new HttpRequestMessage(HttpMethod.Head, PackageUrl);
            using var response = await client.SendAsync(request, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
            if (response.IsSuccessStatusCode == false)
            {
                return null;
            }

            return response.Content.Headers.ContentLength;
        }
        catch (Exception) when (cancellationToken.IsCancellationRequested == false)
        {
            return null;
        }
    }

    private static string UpdateStatePath =>
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "logicPOS", "update-package.txt");

    private static void RememberPackage(string packageVersion, long length)
    {
        var parsed = ApplicationVersionComparer.ParseRelease(packageVersion);
        if (parsed is null || length <= 0)
        {
            return;
        }

        try
        {
            var path = UpdateStatePath;
            Directory.CreateDirectory(Path.GetDirectoryName(path)!);
            File.WriteAllText(path, parsed + Environment.NewLine + length.ToString());
        }
        catch
        {
            // The next launch will compare the assembly again.
        }
    }

    private static bool IsKnownCurrentPackage(long? remoteLength, string installedVersion)
    {
        if (remoteLength is not > 0)
        {
            return false;
        }

        try
        {
            var path = UpdateStatePath;
            if (File.Exists(path) == false)
            {
                return false;
            }

            var lines = File.ReadAllLines(path);
            if (lines.Length < 2 || long.TryParse(lines[1].Trim(), out var storedLength) == false || storedLength != remoteLength)
            {
                return false;
            }

            return ApplicationVersionComparer.IsNewer(lines[0], installedVersion) == false;
        }
        catch
        {
            return false;
        }
    }

    private static void StripProtectedFiles(string payloadRoot)
    {
        foreach (var path in Directory.EnumerateFiles(payloadRoot, "*", SearchOption.AllDirectories))
        {
            var name = Path.GetFileName(path);
            if (IsProtectedFileName(name))
            {
                TryDeleteFile(path);
            }
        }
    }

    internal static bool IsProtectedFileName(string name)
    {
        var n = name.ToLowerInvariant();
        return n == "appsettings.json"
            || n.StartsWith("appsettings.", StringComparison.Ordinal)
            || n.EndsWith(".db", StringComparison.Ordinal)
            || n.EndsWith(".db-wal", StringComparison.Ordinal)
            || n.EndsWith(".db-shm", StringComparison.Ordinal)
            || n.EndsWith(".sqlite", StringComparison.Ordinal)
            || n.EndsWith(".sqlite-wal", StringComparison.Ordinal)
            || n.EndsWith(".sqlite-shm", StringComparison.Ordinal)
            || n is "hardware.id" or "licence.dat" or "license.dat";
    }

    private static async Task DownloadAsync(string zipPath, IProgress<string> progress, CancellationToken cancellationToken)
    {
        using var client = new HttpClient { Timeout = TimeSpan.FromMinutes(15) };
        client.DefaultRequestHeaders.UserAgent.ParseAdd("logicpos-updater");
        using var response = await client.GetAsync(PackageUrl, HttpCompletionOption.ResponseHeadersRead, cancellationToken);
        response.EnsureSuccessStatusCode();
        var total = response.Content.Headers.ContentLength;

        // Dispose write handle before any read/extract — holding File.Create open causes
        // sharing violations on Windows when opening the same path again.
        await using (var input = await response.Content.ReadAsStreamAsync(cancellationToken))
        await using (var output = new FileStream(
            zipPath,
            FileMode.Create,
            FileAccess.Write,
            FileShare.None,
            bufferSize: 81920,
            options: FileOptions.Asynchronous))
        {
            var buffer = new byte[81920];
            long received = 0;
            int read;
            var lastPercent = -1;
            while ((read = await input.ReadAsync(buffer, cancellationToken)) > 0)
            {
                await output.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
                received += read;
                if (total is > 0)
                {
                    // Cap at 99% so the bar does not look finished while extract/apply still run.
                    var percent = Math.Min(99, (int)(received * 100 / total.Value));
                    if (percent != lastPercent)
                    {
                        lastPercent = percent;
                        progress.Report($"A descarregar... {percent}% ({FormatSize(received)} / {FormatSize(total.Value)})");
                    }
                }
                else
                {
                    progress.Report($"A descarregar... {FormatSize(received)}");
                }
            }

            await output.FlushAsync(cancellationToken);
        }

        await ValidateZipSignatureAsync(zipPath, cancellationToken);
    }

    private static async Task ValidateZipSignatureAsync(string zipPath, CancellationToken cancellationToken)
    {
        await using var check = new FileStream(
            zipPath,
            FileMode.Open,
            FileAccess.Read,
            FileShare.Read,
            bufferSize: 4096,
            options: FileOptions.Asynchronous);
        var signature = new byte[2];
        if (await check.ReadAsync(signature, cancellationToken) < 2 || signature[0] != (byte)'P' || signature[1] != (byte)'K')
        {
            throw new InvalidOperationException("A transferência não devolveu um zip válido.");
        }
    }

    private static void ExtractZipWithRetry(string zipPath, string extractPath, CancellationToken cancellationToken)
    {
        const int maxAttempts = 5;
        for (var attempt = 1; attempt <= maxAttempts; attempt++)
        {
            cancellationToken.ThrowIfCancellationRequested();
            try
            {
                if (Directory.Exists(extractPath))
                {
                    Directory.Delete(extractPath, recursive: true);
                }

                ZipFile.ExtractToDirectory(zipPath, extractPath);
                return;
            }
            catch (IOException) when (attempt < maxAttempts)
            {
                // Antivirus or delayed handle release after download.
                Thread.Sleep(200 * attempt);
            }
        }
    }

    private static string FormatSize(long bytes)
    {
        if (bytes < 1024)
        {
            return $"{bytes} B";
        }

        if (bytes < 1024 * 1024)
        {
            return $"{bytes / 1024.0:0.#} KB";
        }

        return $"{bytes / (1024.0 * 1024.0):0.0} MB";
    }

    private static string? FindPayload(string extracted)
    {
        if (File.Exists(Path.Combine(extracted, "logicpos.exe")))
        {
            return extracted;
        }

        var matches = Directory.GetFiles(extracted, "logicpos.exe", SearchOption.AllDirectories);
        if (matches.Length == 0)
        {
            return null;
        }

        return Path.GetDirectoryName(matches.OrderBy(path => path.Length).First());
    }

    private static void StartScript(string scriptPath)
    {
        // Visible window: user must see progress while files are copied after this process exits.
        var started = Process.Start(new ProcessStartInfo
        {
            FileName = "powershell.exe",
            Arguments = "-NoProfile -ExecutionPolicy Bypass -File \"" + scriptPath + "\"",
            UseShellExecute = true,
            WindowStyle = ProcessWindowStyle.Normal
        });
        if (started is null)
        {
            throw new InvalidOperationException("Não foi possível iniciar a atualização.");
        }
    }

    private static void TryDeleteFile(string path)
    {
        try
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
        catch
        {
            // Best-effort cleanup.
        }
    }

    private static void TryDeleteDirectory(string path)
    {
        try
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, recursive: true);
            }
        }
        catch
        {
            // Best-effort: AV or explorer may hold a handle.
        }
    }

    private static string BuildScript(int processId, string source, string target, string tempRoot)
    {
        var sourceLiteral = source.Replace("'", "''");
        var targetLiteral = target.Replace("'", "''");
        var tempRootLiteral = tempRoot.Replace("'", "''");
        return $$"""
            $ErrorActionPreference = 'Stop'
            $ProcessId = {{processId}}
            $Source = '{{sourceLiteral}}'
            $Target = '{{targetLiteral}}'
            $TempRoot = '{{tempRootLiteral}}'
            $BackupRoot = Join-Path $Target '.update-backup'
            $log = Join-Path $env:TEMP 'logicpos-update.log'
            function Write-Log([string]$message) {
                Add-Content -Path $log -Value ((Get-Date -Format o) + ' ' + $message)
            }
            function Keep-Local([string]$name) {
                $n = $name.ToLowerInvariant()
                return $n -eq 'appsettings.json' -or $n.StartsWith('appsettings.') -or
                       $n.EndsWith('.db') -or $n.EndsWith('.db-wal') -or $n.EndsWith('.db-shm') -or
                       $n.EndsWith('.sqlite') -or $n.EndsWith('.sqlite-wal') -or $n.EndsWith('.sqlite-shm') -or
                       $n -eq 'hardware.id' -or $n -eq 'licence.dat' -or $n -eq 'license.dat'
            }
            function Remove-UpdateTempFolders {
                if ($TempRoot -and (Test-Path -LiteralPath $TempRoot)) {
                    Remove-Item -LiteralPath $TempRoot -Recurse -Force -ErrorAction SilentlyContinue
                }
                Get-ChildItem -LiteralPath $env:TEMP -Directory -Filter 'logicpos-update-*' -ErrorAction SilentlyContinue |
                    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue
            }
            function Restore-UpdateBackup {
                Write-Log 'restoring backup'
                $label.Text = 'A repor os ficheiros anteriores...'
                [System.Windows.Forms.Application]::DoEvents()
                if (Test-Path -LiteralPath $BackupRoot) {
                    Get-ChildItem -LiteralPath $BackupRoot -Recurse -File -ErrorAction SilentlyContinue | ForEach-Object {
                        $rel = $_.FullName.Substring($BackupRoot.Length).TrimStart('\')
                        $dest = Join-Path $Target $rel
                        $folder = Split-Path -Parent $dest
                        try {
                            if (-not (Test-Path -LiteralPath $folder)) {
                                New-Item -ItemType Directory -Path $folder -Force | Out-Null
                            }
                            Copy-Item -LiteralPath $_.FullName -Destination $dest -Force
                        } catch {
                            Write-Log ("restore fail " + $rel)
                        }
                    }
                }
                foreach ($created in $createdFiles) {
                    try {
                        if (Test-Path -LiteralPath $created) {
                            Remove-Item -LiteralPath $created -Force
                        }
                    } catch {
                        Write-Log ("remove new fail " + $created)
                    }
                }
                if (Test-Path -LiteralPath $BackupRoot) {
                    Remove-Item -LiteralPath $BackupRoot -Recurse -Force -ErrorAction SilentlyContinue
                }
            }

            Add-Type -AssemblyName System.Windows.Forms
            Add-Type -AssemblyName System.Drawing
            $form = New-Object System.Windows.Forms.Form
            $form.Text = 'logicPOS - A atualizar'
            $form.Width = 480
            $form.Height = 160
            $form.StartPosition = 'CenterScreen'
            $form.FormBorderStyle = 'FixedDialog'
            $form.MaximizeBox = $false
            $form.MinimizeBox = $false
            $form.TopMost = $true
            $label = New-Object System.Windows.Forms.Label
            $label.AutoSize = $false
            $label.Left = 16
            $label.Top = 16
            $label.Width = 430
            $label.Height = 40
            $label.Text = 'A aguardar o fecho da aplicação...'
            $bar = New-Object System.Windows.Forms.ProgressBar
            $bar.Left = 16
            $bar.Top = 64
            $bar.Width = 430
            $bar.Height = 24
            $bar.Style = 'Continuous'
            $bar.Minimum = 0
            $bar.Maximum = 100
            $form.Controls.Add($label)
            $form.Controls.Add($bar)
            $form.Show()
            [System.Windows.Forms.Application]::DoEvents()

            Write-Log "waiting $ProcessId"
            $deadline = (Get-Date).AddSeconds(20)
            $stopped = $false
            while (Get-Process -Id $ProcessId -ErrorAction SilentlyContinue) {
                if ((Get-Date) -gt $deadline) {
                    if (-not $stopped) {
                        Write-Log 'stopping process so files can be replaced'
                        Stop-Process -Id $ProcessId -Force -ErrorAction SilentlyContinue
                        $stopped = $true
                        $deadline = (Get-Date).AddSeconds(15)
                    } else {
                        Write-Log 'timeout waiting for exit'
                        $label.Text = 'Timeout à espera do fecho da aplicação.'
                        [System.Windows.Forms.Application]::DoEvents()
                        Start-Sleep -Seconds 3
                        exit 1
                    }
                }
                Start-Sleep -Milliseconds 300
                [System.Windows.Forms.Application]::DoEvents()
            }
            Start-Sleep -Seconds 1

            if (-not (Test-Path -LiteralPath $Source)) {
                Write-Log "source missing $Source"
                $label.Text = 'O pacote de atualização não foi encontrado.'
                [System.Windows.Forms.Application]::DoEvents()
                Start-Sleep -Seconds 4
                exit 1
            }

            $sourceRoot = (Resolve-Path -LiteralPath $Source).Path.TrimEnd('\')
            $files = @(Get-ChildItem -LiteralPath $sourceRoot -Recurse -File | Where-Object { -not (Keep-Local $_.Name) })
            $total = [Math]::Max(1, $files.Count)
            $index = 0
            $copyFailed = $false
            $createdFiles = New-Object System.Collections.Generic.List[string]
            try {
                if (Test-Path -LiteralPath $BackupRoot) {
                    Remove-Item -LiteralPath $BackupRoot -Recurse -Force
                }
                New-Item -ItemType Directory -Path $BackupRoot -Force | Out-Null
                foreach ($file in $files) {
                    $index++
                    $relative = $file.FullName.Substring($sourceRoot.Length).TrimStart('\')
                    $destination = Join-Path $Target $relative
                    $folder = Split-Path -Parent $destination
                    if (-not (Test-Path -LiteralPath $folder)) {
                        New-Item -ItemType Directory -Path $folder -Force | Out-Null
                    }
                    $percent = [int](($index * 100) / $total)
                    $bar.Value = [Math]::Min(100, $percent)
                    $label.Text = "A copiar ($index / $($files.Count)): $relative"
                    [System.Windows.Forms.Application]::DoEvents()

                    $existed = Test-Path -LiteralPath $destination
                    if ($existed) {
                        $backupDest = Join-Path $BackupRoot $relative
                        $backupFolder = Split-Path -Parent $backupDest
                        if (-not (Test-Path -LiteralPath $backupFolder)) {
                            New-Item -ItemType Directory -Path $backupFolder -Force | Out-Null
                        }
                        Copy-Item -LiteralPath $destination -Destination $backupDest -Force
                    }

                    $copied = $false
                    for ($try = 0; $try -lt 8 -and -not $copied; $try++) {
                        try {
                            Copy-Item -LiteralPath $file.FullName -Destination $destination -Force
                            $copied = $true
                        } catch {
                            Start-Sleep -Milliseconds 400
                        }
                    }
                    if (-not $copied) {
                        Write-Log ("fail " + $relative)
                        if (-not $existed -and (Test-Path -LiteralPath $destination)) {
                            Remove-Item -LiteralPath $destination -Force -ErrorAction SilentlyContinue
                        }
                        $copyFailed = $true
                        break
                    }
                    if (-not $existed) {
                        [void]$createdFiles.Add($destination)
                    }
                }
            } catch {
                Write-Log ("copy error " + $_.Exception.Message)
                $copyFailed = $true
            }

            if ($copyFailed) {
                Restore-UpdateBackup
                Write-Log 'update aborted; previous files restored'
                $label.Text = 'A atualização falhou. Os ficheiros anteriores foram repostos.'
                [System.Windows.Forms.Application]::DoEvents()
                Start-Sleep -Seconds 4
                exit 1
            }

            if (Test-Path -LiteralPath $BackupRoot) {
                Remove-Item -LiteralPath $BackupRoot -Recurse -Force -ErrorAction SilentlyContinue
            }

            $label.Text = 'A reiniciar o logicPOS...'
            $bar.Value = 100
            [System.Windows.Forms.Application]::DoEvents()
            Write-Log 'start'
            Start-Process -FilePath (Join-Path $Target 'logicpos.exe') -WorkingDirectory $Target
            Start-Sleep -Milliseconds 800
            $form.Close()
            Remove-UpdateTempFolders
            Write-Log 'temp cleaned'
            """;
    }
}
