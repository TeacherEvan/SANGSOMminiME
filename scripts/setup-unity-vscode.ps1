<#
Automates Unity ↔ VS Code integration on Windows.
- Detects Unity Editor (Hub install paths).
- Detects VS Code (Code.exe or `code` on PATH).
- Sets Unity's external script editor to VS Code via registry.
- Merges recommended VS Code extensions for Unity/C# into .vscode/extensions.json.
#>

[CmdletBinding()]
param(
    [string]$RepoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..')).Path
)

function Resolve-UnityEditor {
    $defaultHub = 'C:\Program Files\Unity\Hub\Editor'
    $legacyPath = 'C:\Program Files\Unity\Editor\Unity.exe'
    $candidates = @()

    if (Test-Path $defaultHub) {
        $candidates += Get-ChildItem -Path $defaultHub -Directory -ErrorAction SilentlyContinue |
        Sort-Object Name -Descending |
        ForEach-Object { Join-Path $_.FullName 'Editor\Unity.exe' }
    }
    $candidates += $legacyPath

    foreach ($c in $candidates) {
        if (Test-Path $c) { return (Resolve-Path $c).Path }
    }

    return $null
}

function Resolve-Code {
    $fromPath = Get-Command code -ErrorAction SilentlyContinue
    if ($fromPath) { return $fromPath.Source }

    $default = Join-Path $Env:LOCALAPPDATA 'Programs\Microsoft VS Code\Code.exe'
    if (Test-Path $default) { return $default }

    $programFiles = 'C:\Program Files\Microsoft VS Code\Code.exe'
    if (Test-Path $programFiles) { return $programFiles }

    return $null
}

function Set-UnityExternalEditor ([string]$codePath) {
    $regPath = 'HKCU:\Software\Unity Technologies\Unity Editor 5.x'
    if (-not (Test-Path $regPath)) { New-Item -Path $regPath -Force | Out-Null }

    Set-ItemProperty -Path $regPath -Name 'kScriptsDefaultApp' -Value $codePath -Force
    Set-ItemProperty -Path $regPath -Name 'kScriptEditorArgs' -Value '-r -g "{file}":{line}:{column}' -Force
}

function Update-VSCodeRecommendations ([string]$root) {
    $vscodeDir = Join-Path $root '.vscode'
    if (-not (Test-Path $vscodeDir)) { return }

    $extFile = Join-Path $vscodeDir 'extensions.json'
    $desired = @(
        'ms-dotnettools.csharp',
        'ms-dotnettools.csdevkit',
        'unity.unity-debug',
        'kreativ-software.unity-snippets',
        'editorconfig.editorconfig'
    )

    if (-not (Test-Path $extFile)) {
        $obj = @{ recommendations = $desired }
        $obj | ConvertTo-Json -Depth 4 | Set-Content -Path $extFile -Encoding UTF8
        return
    }

    $json = Get-Content -Path $extFile -Raw | ConvertFrom-Json
    if (-not $json.recommendations) { $json | Add-Member -Name recommendations -Value @() -MemberType NoteProperty }

    $merged = @($json.recommendations + $desired) | Select-Object -Unique
    $json.recommendations = $merged
    $json | ConvertTo-Json -Depth 4 | Set-Content -Path $extFile -Encoding UTF8
}

$unity = Resolve-UnityEditor
if (-not $unity) {
    Write-Warning 'Unity Editor not detected. Install via Unity Hub, then re-run this script.'
    exit 1
}

$code = Resolve-Code
if (-not $code) {
    Write-Warning 'VS Code not found. Install VS Code and ensure `code` is on PATH, then re-run.'
    exit 1
}

Set-UnityExternalEditor -codePath $code
Update-VSCodeRecommendations -root $RepoRoot

Write-Host "Unity detected at: $unity"
Write-Host "VS Code detected at: $code"
Write-Host 'External script editor set to VS Code. Recommended extensions merged.'
Write-Host 'If IntelliSense is missing, in Unity: Edit > Preferences > External Tools > Regenerate project files.'
