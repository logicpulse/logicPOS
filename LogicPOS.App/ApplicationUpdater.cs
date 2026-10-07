using System.Diagnostics;
using System.IO.Compression;
using System.Net.Http;

namespace LogicPOS.App;

/// <summary>
/// Downloads the published zip and replaces program files after this process exits.
/// Local settings and database files are left in place.
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

        progress.Report("A descarregar...");
        await DownloadAsync(zipPath, progress, cancellationToken);

        progress.Report("A preparar os ficheiros...");
        ZipFile.ExtractToDirectory(zipPath, extractPath);
        var payload = FindPayload(extractPath)
            ?? throw new InvalidOperationException("O pacote não contém logicpos.exe.");

        var scriptPath = Path.Combine(tempRoot, "apply.ps1");
        await File.WriteAllTextAsync(scriptPath, BuildScript(Environment.ProcessId, payload, install), cancellationToken);
        StartScript(scriptPath);
        progress.Report("A reiniciar...");
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
        while ((read = await input.ReadAsync(buffer, cancellationToken)) > 0)
        {
            await output.WriteAsync(buffer.AsMemory(0, read), cancellationToken);
            received += read;
            if (total is > 0)
            {
                progress.Report($"A descarregar... {received * 100 / total.Value}%");
            }
        }

        output.Close();
        await using var check = File.OpenRead(zipPath);
        var signature = new byte[2];
        if (await check.ReadAsync(signature, cancellationToken) < 2 || signature[0] != (byte)'P' || signature[1] != (byte)'K')
        {
            throw new InvalidOperationException("A transferência não devolveu um zip válido.");
        }
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
        var started = Process.Start(new ProcessStartInfo
        {
            FileName = "cmd.exe",
            Arguments = "/c start \"logicpos-update\" /min powershell.exe -NoProfile -ExecutionPolicy Bypass -File \"" + scriptPath + "\"",
            CreateNoWindow = true,
            UseShellExecute = false,
            WindowStyle = ProcessWindowStyle.Hidden
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
                return $n -eq 'appsettings.json' -or $n.StartsWith('appsettings.') -or $n.EndsWith('.db') -or $n.EndsWith('.db-wal') -or $n.EndsWith('.db-shm') -or $n.EndsWith('.sqlite') -or $n.EndsWith('.sqlite-wal') -or $n.EndsWith('.sqlite-shm')
            }
            Write-Log "waiting $ProcessId"
            $deadline = (Get-Date).AddMinutes(2)
            while (Get-Process -Id $ProcessId -ErrorAction SilentlyContinue) {
                if ((Get-Date) -gt $deadline) {
                    Write-Log 'timeout waiting for exit'
                    exit 1
                }
                Start-Sleep -Milliseconds 300
            }
            Start-Sleep -Seconds 1
            $sourceRoot = (Resolve-Path -LiteralPath $Source).Path.TrimEnd('\')
            Get-ChildItem -LiteralPath $sourceRoot -Recurse -File | ForEach-Object {
                if (Keep-Local $_.Name) {
                    Write-Log ("skip " + $_.FullName.Substring($sourceRoot.Length))
                    return
                }
                $relative = $_.FullName.Substring($sourceRoot.Length).TrimStart('\')
                $destination = Join-Path $Target $relative
                $folder = Split-Path -Parent $destination
                if (-not (Test-Path -LiteralPath $folder)) {
                    New-Item -ItemType Directory -Path $folder -Force | Out-Null
                }
                $copied = $false
                for ($try = 0; $try -lt 8 -and -not $copied; $try++) {
                    try {
                        Copy-Item -LiteralPath $_.FullName -Destination $destination -Force
                        $copied = $true
                    } catch {
                        Start-Sleep -Milliseconds 400
                    }
                }
                if (-not $copied) {
                    Write-Log ("fail " + $relative)
                }
            }
            Write-Log 'start'
            Start-Process -FilePath (Join-Path $Target 'logicpos.exe') -WorkingDirectory $Target
            """;
    }
}
