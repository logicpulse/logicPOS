param(
    [ValidateSet('Build', 'Rebuild', 'Clean')]
    [string]$Target = 'Build',
    [ValidateSet('Debug', 'Release')]
    [string]$Configuration = 'Debug'
)
$ErrorActionPreference = 'Stop'
$sln = Join-Path (Split-Path -Parent $PSScriptRoot) 'LogicPOS.sln'
$vswhere = Join-Path ${env:ProgramFiles(x86)} 'Microsoft Visual Studio\Installer\vswhere.exe'
if (-not (Test-Path $vswhere)) {
    Write-Error "vswhere.exe not found. Install Visual Studio or Build Tools with the MSBuild workload."
}
$msbuild = & $vswhere -latest -requires Microsoft.Component.MSBuild -find 'MSBuild\**\Bin\MSBuild.exe' | Select-Object -First 1
if (-not $msbuild) {
    Write-Error "MSBuild.exe not found. Install Visual Studio or Build Tools with MSBuild."
}
& $msbuild $sln /restore /t:$Target /p:Configuration=$Configuration /p:Platform="Any CPU" /verbosity:minimal
exit $LASTEXITCODE
