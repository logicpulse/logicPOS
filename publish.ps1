param (
    [string]$SolutionOrProject = "LogicPOS.UI\LogicPOS.UI.csproj",
    [string]$Configuration = "Release",
    [string]$OutputDir = "..\..\artifacts\publish\pos"
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

MSBuild $SolutionOrProject `
    /t:Clean,Build `
    /p:Configuration=$Configuration `
    /p:OutputPath=$OutputDir `
    /verbosity:minimal

if ($LASTEXITCODE -ne 0) {
    Fail "Build failed."
}

Log "Build succeeded. Output available at: $OutputDir"
