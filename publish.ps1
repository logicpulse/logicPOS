param (
    [string]$Configuration = "Release",
    [string]$OutputDir = "..\..\artifacts\publish\pos\",
    [string]$ProductVersion = "",
    [string]$ReleaseChannel = "retail",
    [string]$RuntimeIdentifier = "win-x64"
)

$ErrorActionPreference = "Stop"
# Production Avalonia entry: LogicPulse host (Licence + Cloud). Fiscal is a side-by-side plugin.
$project = Join-Path $PSScriptRoot "..\logicPOS-internal\host\LogicPOS.Host\LogicPOS.Host.csproj"
$fiscalProject = Join-Path $PSScriptRoot "..\logicPOS-internal\fiscal\src\LogicPOS.Fiscal\LogicPOS.Fiscal.csproj"
$seedsSource = Join-Path $PSScriptRoot "db_seeds"
$iconSource = Join-Path $PSScriptRoot "LogicPOS.App\Assets\application.ico"

if (-not [System.IO.Path]::IsPathRooted($OutputDir)) {
    $OutputDir = Join-Path $PSScriptRoot $OutputDir
}
$OutputDir = [System.IO.Path]::GetFullPath($OutputDir)

if (-not (Test-Path -LiteralPath $project)) {
    Write-Host "[ERROR] Host project not found: $project" -ForegroundColor Red
    exit 1
}

if (Test-Path $OutputDir) {
    Remove-Item -Path (Join-Path $OutputDir "*") -Recurse -Force
}
else {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

function Get-PublishArgs([string]$proj, [string]$outDir) {
    $args = @(
        "publish", $proj,
        "-c", $Configuration,
        "-r", $RuntimeIdentifier,
        "--self-contained", "true",
        "-o", $outDir,
        "-p:PublishSingleFile=false",
        "-p:ErrorOnDuplicatePublishOutputFiles=false"
    )
    if (-not [string]::IsNullOrWhiteSpace($ProductVersion)) {
        $version = $ProductVersion.Trim()
        $channel = if ([string]::IsNullOrWhiteSpace($ReleaseChannel)) { "retail" } else { $ReleaseChannel.Trim() }
        $args += "-p:Version=$version"
        $args += "-p:AssemblyVersion=$version.0"
        $args += "-p:FileVersion=$version.0"
        $args += "-p:InformationalVersion=$version+$channel"
    }
    return $args
}

Write-Host "[INFO] Publishing Avalonia LogicPulse host..."
dotnet @(Get-PublishArgs $project $OutputDir)
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Host publish failed." -ForegroundColor Red
    exit 1
}

if (Test-Path -LiteralPath $fiscalProject) {
    $fiscalOut = Join-Path $env:TEMP ("logicpos-fiscal-publish-" + [Guid]::NewGuid().ToString("N"))
    New-Item -ItemType Directory -Path $fiscalOut | Out-Null
    try {
        Write-Host "[INFO] Publishing LogicPOS.Fiscal plugin..."
        # Plugin is framework-dependent; only the DLL needs to sit beside logicpos.exe.
        $fiscalArgs = @(
            "publish", $fiscalProject,
            "-c", $Configuration,
            "-o", $fiscalOut,
            "-p:PublishSingleFile=false"
        )
        if (-not [string]::IsNullOrWhiteSpace($ProductVersion)) {
            $version = $ProductVersion.Trim()
            $fiscalArgs += "-p:Version=$version"
        }
        dotnet @fiscalArgs
        if ($LASTEXITCODE -ne 0) {
            Write-Host "[ERROR] Fiscal publish failed." -ForegroundColor Red
            exit 1
        }

        $fiscalDll = Join-Path $fiscalOut "LogicPOS.Fiscal.dll"
        if (-not (Test-Path -LiteralPath $fiscalDll)) {
            Write-Host "[ERROR] LogicPOS.Fiscal.dll missing after publish: $fiscalDll" -ForegroundColor Red
            exit 1
        }

        Copy-Item -LiteralPath $fiscalDll -Destination (Join-Path $OutputDir "LogicPOS.Fiscal.dll") -Force
    }
    finally {
        Remove-Item -LiteralPath $fiscalOut -Recurse -Force -ErrorAction SilentlyContinue
    }
}
else {
    Write-Host "[ERROR] Fiscal project not found: $fiscalProject" -ForegroundColor Red
    exit 1
}

if (-not (Test-Path -LiteralPath $seedsSource)) {
    Write-Host "[ERROR] db_seeds not found: $seedsSource" -ForegroundColor Red
    exit 1
}

$seedsTarget = Join-Path $OutputDir "db_seeds"
Copy-Item -LiteralPath $seedsSource -Destination $seedsTarget -Recurse -Force

if ((Test-Path -LiteralPath $iconSource) -and -not (Test-Path -LiteralPath (Join-Path $OutputDir "application.ico"))) {
    Copy-Item -LiteralPath $iconSource -Destination (Join-Path $OutputDir "application.ico") -Force
}

$required = @(
    (Join-Path $OutputDir "logicpos.exe"),
    (Join-Path $OutputDir "LogicPOS.Core.dll"),
    (Join-Path $OutputDir "LogicPOS.App.dll"),
    (Join-Path $OutputDir "Avalonia.dll"),
    (Join-Path $OutputDir "LogicPOS.Fiscal.dll"),
    (Join-Path $OutputDir "LogicPOS.Cloud.dll"),
    (Join-Path $OutputDir "LogicPOS.Licence.dll"),
    (Join-Path $OutputDir "application.ico"),
    (Join-Path $OutputDir "db_seeds\modules\default\users.json")
)
foreach ($path in $required) {
    if (-not (Test-Path -LiteralPath $path)) {
        Write-Host "[ERROR] Avalonia POS publish is missing: $path" -ForegroundColor Red
        exit 1
    }
}

$forbidden = @(
    (Join-Path $OutputDir "GtkRuntime"),
    (Join-Path $OutputDir "gtk-sharp.dll"),
    (Join-Path $OutputDir "logicpos.exe.config")
)
foreach ($path in $forbidden) {
    if (Test-Path -LiteralPath $path) {
        Write-Host "[ERROR] GTK leftover in Avalonia POS publish: $path" -ForegroundColor Red
        exit 1
    }
}

Write-Host "[INFO] Avalonia LogicPulse POS published to: $OutputDir"
