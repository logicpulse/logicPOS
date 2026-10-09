using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http;

namespace LogicPOS.App;

/// <summary>
/// Downloads the published zip and replaces program files after this process exits.
/// Local settings and database files are never replaced.
/// </summary>
internal static class ApplicationUpdater
{
    public const string PackageUrl = "https://box.logicpulse.com/files/logicPOS/Installation.Windows/Avalonia/logicpos.zip";

    public static async Task DownloadAndScheduleAsync(IProgress<string> progress, CancellationToken cancellationToken = default)
    {
        var install = AppContext.BaseDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
        var tempRoot = Path.Combine(Path.GetTempPath(), "logicpos-update-" + Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(tempRoot);
        var zipPath = Path.Combine(tempRoot, "logicpos.zip");
        var extractPath = Path.Combine(tempRoot, "files");

        progress.Report("A descarregar o pacote...");
        await DownloadAsync(zipPath, progress, cancellationToken);

        progress.Report("A extrair ficheiros...");
        if (Directory.Exists(extractPath))
        {
            Directory.Delete(extractPath, recursive: true);
        }

        ZipFile.ExtractToDirectory(zipPath, extractPath);
        var payload = FindPayload(extractPath)
            ?? throw new InvalidOperationException("O pacote não contém logicpos.exe.");

        // Never ship install-time defaults over a live cloud/local configuration.
        StripProtectedFiles(payload);

        progress.Report("A preparar a aplicação dos ficheiros...");
        var scriptPath = Path.Combine(tempRoot, "apply.ps1");
        await File.WriteAllTextAsync(scriptPath, BuildScript(Environment.ProcessId, payload, install), cancellationToken);
        StartScript(scriptPath);
        progress.Report("A fechar para aplicar a atualização (janela de progresso a abrir)...");
    }

    private static void StripProtectedFiles(string payloadRoot)
    {
        foreach (var path in Directory.EnumerateFiles(payloadRoot, "*", SearchOption.AllDirectories))
        {
            var name = Path.GetFileName(path);
            if (IsProtectedFileName(name))
            {
                try
                {
                    File.Delete(path);
                }
                catch
                {
                    // Apply script still skips these names.
                }
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
        await using var input = await response.Content.ReadAsStreamAsync(cancellationToken);
        await using var output = File.Create(zipPath);
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
                var percent = (int)(received * 100 / total.Value);
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
        await using var check = File.OpenRead(zipPath);
        var signature = new byte[2];
        if (await check.ReadAsync(signature, cancellationToken) < 2 || signature[0] != (byte)'P' || signature[1] != (byte)'K')
        {
            throw new InvalidOperationException("A transferência não devolveu um zip válido.");
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

    private static string BuildScript(int processId, string source, string target)
    {
        var sourceLiteral = source.Replace("'", "''");
        var targetLiteral = target.Replace("'", "''");
        return $$"""
            $ErrorActionPreference = 'Continue'
            $ProcessId = {{processId}}
            $Source = '{{sourceLiteral}}'
            $Target = '{{targetLiteral}}'
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
            $deadline = (Get-Date).AddMinutes(2)
            while (Get-Process -Id $ProcessId -ErrorAction SilentlyContinue) {
                if ((Get-Date) -gt $deadline) {
                    Write-Log 'timeout waiting for exit'
                    $label.Text = 'Timeout à espera do fecho da aplicação.'
                    [System.Windows.Forms.Application]::DoEvents()
                    Start-Sleep -Seconds 3
                    exit 1
                }
                Start-Sleep -Milliseconds 300
                [System.Windows.Forms.Application]::DoEvents()
            }
            Start-Sleep -Seconds 1

            $sourceRoot = (Resolve-Path -LiteralPath $Source).Path.TrimEnd('\')
            $files = @(Get-ChildItem -LiteralPath $sourceRoot -Recurse -File | Where-Object { -not (Keep-Local $_.Name) })
            $total = [Math]::Max(1, $files.Count)
            $index = 0
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
                }
            }

            $label.Text = 'A reiniciar o logicPOS...'
            $bar.Value = 100
            [System.Windows.Forms.Application]::DoEvents()
            Write-Log 'start'
            Start-Process -FilePath (Join-Path $Target 'logicpos.exe') -WorkingDirectory $Target
            Start-Sleep -Milliseconds 800
            $form.Close()
            """;
    }
}
