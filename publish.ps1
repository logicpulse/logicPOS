param (
    [string]$Configuration = "Release",
    [string]$OutputDir = "..\..\artifacts\publish\pos\"
)

$ErrorActionPreference = "Stop"
$project = Join-Path $PSScriptRoot "LogicPOS.App\logicpos.csproj"

if (Test-Path $OutputDir) {
    Remove-Item -Path (Join-Path $OutputDir "*") -Recurse -Force
}
else {
    New-Item -ItemType Directory -Path $OutputDir | Out-Null
}

dotnet publish $project -c $Configuration -o $OutputDir
if ($LASTEXITCODE -ne 0) {
    Write-Host "[ERROR] Publish failed." -ForegroundColor Red
    exit 1
}

Write-Host "[INFO] Publish succeeded. Output available at: $OutputDir"
