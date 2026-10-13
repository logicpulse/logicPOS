param (
    [string]$PublishDir = "..\..\artifacts\publish\pos",
    [string]$ZipOutput = "..\..\artifacts\logicpos.zip",
    [string]$ShareZip = "\\lproot\box.logicpulse.pt\logicPOS\Installation.Windows\Avalonia\logicpos.zip",
    [switch]$SkipPublish,
    [string]$ProductVersion = "1.6.3"
)

$ErrorActionPreference = "Stop"

if (-not [System.IO.Path]::IsPathRooted($PublishDir)) {
    $PublishDir = Join-Path $PSScriptRoot $PublishDir
}
if (-not [System.IO.Path]::IsPathRooted($ZipOutput)) {
    $ZipOutput = Join-Path $PSScriptRoot $ZipOutput
}
$PublishDir = [System.IO.Path]::GetFullPath($PublishDir)
$ZipOutput = [System.IO.Path]::GetFullPath($ZipOutput)

if (-not $SkipPublish) {
    & (Join-Path $PSScriptRoot "publish.ps1") -Configuration Release -OutputDir $PublishDir -ProductVersion $ProductVersion -ReleaseChannel retail
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }
}

if (-not (Test-Path -LiteralPath (Join-Path $PublishDir "logicpos.exe"))) {
    throw "Publish folder missing logicpos.exe: $PublishDir"
}

# Staging copy without protected local settings/databases.
$stage = Join-Path $env:TEMP ("logicpos-update-zip-" + [Guid]::NewGuid().ToString("N"))
New-Item -ItemType Directory -Path $stage | Out-Null
try {
    # Do not use -LiteralPath with wildcards — they are not expanded.
    Copy-Item -Path (Join-Path $PublishDir "*") -Destination $stage -Recurse -Force
    $stagedCount = @(Get-ChildItem -LiteralPath $stage -Force).Count
    if ($stagedCount -eq 0) {
        throw "Staging folder is empty after copy from: $PublishDir"
    }
    Write-Host "[INFO] Staged $stagedCount items from publish folder."

    $protectedNames = @(
        "appsettings.json",
        "appsettings.Development.json",
        "appsettings.Production.json",
        "hardware.id",
        "licence.dat",
        "license.dat"
    )
    Get-ChildItem -LiteralPath $stage -Recurse -File | ForEach-Object {
        $name = $_.Name
        $lower = $name.ToLowerInvariant()
        $drop = $protectedNames -contains $name -or
            $lower.StartsWith("appsettings.") -or
            $lower.EndsWith(".db") -or
            $lower.EndsWith(".db-wal") -or
            $lower.EndsWith(".db-shm") -or
            $lower.EndsWith(".sqlite") -or
            $lower.EndsWith(".sqlite-wal") -or
            $lower.EndsWith(".sqlite-shm")
        if ($drop) {
            Remove-Item -LiteralPath $_.FullName -Force
            Write-Host "[INFO] Excluded from update zip: $name"
        }
    }

    if (Test-Path -LiteralPath (Join-Path $stage "appsettings.json")) {
        throw "Refuse to pack update zip: appsettings.json is still present."
    }

    $zipDir = Split-Path -Parent $ZipOutput
    if (-not (Test-Path -LiteralPath $zipDir)) {
        New-Item -ItemType Directory -Path $zipDir -Force | Out-Null
    }
    if (Test-Path -LiteralPath $ZipOutput) {
        Remove-Item -LiteralPath $ZipOutput -Force
    }
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    [System.IO.Compression.ZipFile]::CreateFromDirectory($stage, $ZipOutput, [System.IO.Compression.CompressionLevel]::Optimal, $false)
    if (-not (Test-Path -LiteralPath $ZipOutput)) {
        throw "Failed to create update zip: $ZipOutput"
    }

    $zip = [System.IO.Compression.ZipFile]::OpenRead($ZipOutput)
    try {
        $hits = @($zip.Entries | Where-Object { $_.FullName -match '(?i)appsettings' } | ForEach-Object FullName)
        if ($hits.Count -gt 0) {
            throw "Update zip still contains appsettings: $($hits -join ', ')"
        }
    }
    finally {
        $zip.Dispose()
    }

    if (-not [string]::IsNullOrWhiteSpace($ShareZip)) {
        $shareDir = Split-Path -Parent $ShareZip
        if (-not (Test-Path -LiteralPath $shareDir)) {
            throw "Share folder not reachable: $shareDir"
        }
        $stamp = Get-Date -Format "yyyyMMdd-HHmmss"
        if (Test-Path -LiteralPath $ShareZip) {
            Copy-Item -LiteralPath $ShareZip -Destination (Join-Path $shareDir "logicpos.$stamp.bak.zip") -Force
        }
        Copy-Item -LiteralPath $ZipOutput -Destination $ShareZip -Force
        Write-Host "[INFO] Published update zip -> $ShareZip"
    }

    $info = Get-Item -LiteralPath $ZipOutput
    Write-Host ("[INFO] Update zip OK: {0} ({1} MB) - no appsettings." -f $info.FullName, [math]::Round($info.Length / 1MB, 1))
}
finally {
    Remove-Item -LiteralPath $stage -Recurse -Force -ErrorAction SilentlyContinue
}
