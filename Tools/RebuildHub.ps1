#requires -Version 5.1
<#
.SYNOPSIS
  Regenerates Assets/_Project/Scenes/Hub.unity via EvaverseHubSceneBuilder.BuildFromCommandLine.

.NOTES
  Unity locks the project: close the Unity Editor (or any other Unity using this folder) first.
  Match Editor version to ProjectSettings/ProjectVersion.txt (default path is Unity 6000.0.65f1 Hub install).
#>
param(
    [string] $UnityEditor = "${env:ProgramFiles}\Unity\Hub\Editor\6000.0.65f1\Editor\Unity.exe",
    [string] $ProjectPath = (Resolve-Path (Join-Path $PSScriptRoot "..")).Path,
    [string] $LogFile = ""
)

$ErrorActionPreference = "Stop"

if (-not (Test-Path -LiteralPath $UnityEditor)) {
    Write-Error "Unity.exe not found: $UnityEditor`nInstall matching editor or pass -UnityEditor."
}

if (-not (Test-Path -LiteralPath $ProjectPath)) {
    Write-Error "Project path not found: $ProjectPath"
}

if ([string]::IsNullOrWhiteSpace($LogFile)) {
    New-Item -ItemType Directory -Force -Path (Join-Path $ProjectPath "logs") | Out-Null
    $LogFile = Join-Path $ProjectPath "logs\unity-rebuild-hub.log"
}

$argsList = @(
    "-batchmode",
    "-nographics",
    "-quit",
    "-projectPath", $ProjectPath,
    "-executeMethod", "Evaverse.World.Editor.EvaverseHubSceneBuilder.BuildFromCommandLine",
    "-logFile", $LogFile
)

Write-Host "Unity: $UnityEditor"
Write-Host "Project: $ProjectPath"
Write-Host "Log: $LogFile"
Write-Host "Running... (close Unity Editor first if rebuild fails with project lock)"

$p = Start-Process -FilePath $UnityEditor -ArgumentList $argsList -Wait -PassThru -NoNewWindow

Write-Host "Exit code: $($p.ExitCode)"
if ($p.ExitCode -ne 0) {
    Write-Host "Tail log:"
    if (Test-Path -LiteralPath $LogFile) { Get-Content -LiteralPath $LogFile -Tail 40 }
    exit $p.ExitCode
}

Write-Host "Done. Open $LogFile and search for 'Evaverse Revival Hub generated'."
exit 0
