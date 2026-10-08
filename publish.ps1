param (
    [string]$Configuration = "Release",
    [string]$OutputDir = "..\..\artifacts\publish\pos\",
    [string]$ProductVersion = "",
    [string]$ReleaseChannel = "retail",
    [string]$RuntimeIdentifier = "win-x64"
)

$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "LogicPOS.Desktop\LogicPOS.Desktop.csproj"
$seedsSource = Join-Path $PSScriptRoot "db_seeds"

if (-not [System.IO.Path]::IsPathRooted($OutputDir)) {
    $OutputDir = Join-Path $PSScriptRoot $OutputDir
}
$OutputDir = [System.IO.Path]::GetFullPath($OutputDir)

if (Test-Path $OutputDir) {
    Remove-Item -Path (Join-Path $OutputDir "*") -Recurse -Force
}
else {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

$publishArgs = @(
    "publish", $project,
    "-c", $Configuration,
    "-r", $RuntimeIdentifier,
    "--self-contained", "true",
    "-o", $OutputDir,
    "-p:PublishSingleFile=false",
    "-p:ErrorOnDuplicatePublishOutputFiles=false"
)

if (-not [string]::IsNullOrWhiteSpace($ProductVersion)) {
    $version = $ProductVersion.Trim()
    $channel = if ([string]::IsNullOrWhiteSpace($ReleaseChannel)) { "retail" } else { $ReleaseChannel.Trim() }
    $publishArgs += "-p:Version=$version"
    $publishArgs += "-p:AssemblyVersion=$version.0"
    $publishArgs += "-p:FileVersion=$version.0"
    $publishArgs += "-p:InformationalVersion=$version+$channel"
}

dotnet @publishArgs
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Publish failed." -ForegroundColor Red
    exit 1
}

if (-not (Test-Path -LiteralPath $seedsSource)) {
    Write-Host "[ERROR] db_seeds not found: $seedsSource" -ForegroundColor Red
    exit 1
}

$seedsTarget = Join-Path $OutputDir "db_seeds"
Copy-Item -LiteralPath $seedsSource -Destination $seedsTarget -Recurse -Force

$exe = Join-Path $OutputDir "logicpos.exe"
$core = Join-Path $OutputDir "LogicPOS.Core.dll"
$avalonia = Join-Path $OutputDir "Avalonia.dll"
foreach ($required in @($exe, $core, $avalonia)) {
    if (-not (Test-Path -LiteralPath $required)) {
        Write-Host "[ERROR] Avalonia POS publish is missing: $required" -ForegroundColor Red
        exit 1
    }
}

Write-Host "[INFO] Avalonia POS published to: $OutputDir"
