param (
    [string]$SolutionOrProject = "LogicPOS.UI\LogicPOS.UI.csproj",
    [string]$Configuration = "Release",
    [string]$ProductVersion = "",
    [string]$OutputDir = "..\..\artifacts\publish\pos\"
)

function Log {
    param ([string]$Message)
    Write-Host "[INFO] $Message"
}

function Fail {
    param ([string]$Message)
    Write-Host "[ERROR] $Message" -ForegroundColor Red
    exit 1
}

function Set-AssemblyInfoVersion {
    param (
        [string]$AssemblyInfoPath,
        [string]$Version
    )

    if (-not (Test-Path -LiteralPath $AssemblyInfoPath)) {
        Fail "AssemblyInfo not found: $AssemblyInfoPath"
    }

    $content = Get-Content -LiteralPath $AssemblyInfoPath -Raw
    $content = $content -replace '\[assembly: AssemblyVersion\("[^"]+"\)\]', "[assembly: AssemblyVersion(`"$Version`")]"
    $content = $content -replace '\[assembly: AssemblyFileVersion\("[^"]+"\)\]', "[assembly: AssemblyFileVersion(`"$Version`")]"
    Set-Content -LiteralPath $AssemblyInfoPath -Value $content -Encoding UTF8
}

function Clear-StaleBuildFolders {
    param ([string]$ProjectPath)

    $projectDir = Split-Path -Path $ProjectPath -Parent
    foreach ($stale in @((Join-Path $projectDir "obj"), (Join-Path $projectDir "bin"))) {
        if (Test-Path -LiteralPath $stale) {
            Log "Cleaning stale build folder: $stale"
            Remove-Item -LiteralPath $stale -Recurse -Force -ErrorAction SilentlyContinue
        }
    }
}

# --- Validate project/solution ---
if (-not (Test-Path $SolutionOrProject)) {
    Fail "Project or solution not found: $SolutionOrProject"
}

# --- Prepare output directory ---
if (Test-Path $OutputDir) {
    Log "Cleaning output directory: $OutputDir"
    Remove-Item -Path $OutputDir\* -Recurse -Force
} else {
    Log "Creating output directory: $OutputDir"
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

# --- Build ---
Log "Building $SolutionOrProject ($Configuration) using global MSBuild"
if ($ProductVersion) {
    Log "Product version: $ProductVersion"
}

$versionProperties = @()
if (-not [string]::IsNullOrWhiteSpace($ProductVersion)) {
    $v = $ProductVersion.Trim()
    $parts = $v.Split('.')
    $fileVersion = if ($parts.Count -eq 3) { "$v.0" } elseif ($parts.Count -ge 4) { $v } else { "$v.0.0" }
    $assemblyInfoPath = Join-Path (Split-Path $SolutionOrProject -Parent) "Properties\AssemblyInfo.cs"
    Set-AssemblyInfoVersion -AssemblyInfoPath $assemblyInfoPath -Version $v
    Clear-StaleBuildFolders -ProjectPath $SolutionOrProject
    $versionProperties = @(
        "/p:ApplicationVersion=$fileVersion",
        "/p:AssemblyVersion=$fileVersion",
        "/p:FileVersion=$fileVersion"
    )
}

MSBuild $SolutionOrProject `
    /restore `
    /t:Clean,Build `
    /p:Configuration=$Configuration `
    /p:OutputPath=$OutputDir `
    @versionProperties `
    /verbosity:minimal

if ($LASTEXITCODE -ne 0) {
    Fail "Build failed."
}

Log "Build succeeded. Output available at: $OutputDir"
