param(
    [string]$ModRoot = (Join-Path $PSScriptRoot '..\OrganicIntegration'),
    [string]$GameConfigRoot = 'C:\Program Files (x86)\Steam\steamapps\common\HeartOfTheMachine\GameData\Configuration'
)

$ErrorActionPreference = 'Stop'

function Get-TableNameFromFolder {
    param([string]$FolderName)

    if ($FolderName -match '^\d+_(.+)$') {
        return $Matches[1]
    }

    return $FolderName
}

function Get-AllowedAttributes {
    param([xml]$Metadata)

    $allowed = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
    [void]$allowed.Add('id')
    [void]$allowed.Add('is_partial_record')
    [void]$allowed.Add('display_name')
    [void]$allowed.Add('description')
    [void]$allowed.Add('strategy_tip')
    [void]$allowed.Add('technical_notes')
    [void]$allowed.Add('how_to_acquire')
    [void]$allowed.Add('sort_order')
    [void]$allowed.Add('copy_from')
    [void]$allowed.Add('tl')
    foreach ($attribute in $Metadata.root.attribute) {
        if ($attribute.key) {
            [void]$allowed.Add([string]$attribute.key)
        }
    }

    return $allowed
}

$resolvedModRoot = (Resolve-Path $ModRoot).Path
$resolvedGameConfigRoot = (Resolve-Path $GameConfigRoot).Path
$errors = [System.Collections.Generic.List[string]]::new()
$metadataCache = @{}

foreach ($xmlFile in Get-ChildItem -Path $resolvedModRoot -Recurse -Filter '*.xml') {
    $relativeDirPath = $xmlFile.DirectoryName.Substring($resolvedModRoot.Length).TrimStart('\')
    if ([string]::IsNullOrWhiteSpace($relativeDirPath)) {
        continue
    }

    $relativeDir = $relativeDirPath.Split([char]'\')[0]
    $tableName = Get-TableNameFromFolder $relativeDir
    $metadataPath = Join-Path (Join-Path $resolvedGameConfigRoot $relativeDir) "_$tableName.metadata"
    if (!(Test-Path $metadataPath)) {
        continue
    }

    if (!$metadataCache.ContainsKey($metadataPath)) {
        [xml]$metadataXml = Get-Content -Path $metadataPath -Raw
        $metadataCache[$metadataPath] = @{
            NodeName = [string]$metadataXml.root.node_name
            Allowed = Get-AllowedAttributes $metadataXml
        }
    }

    $metadata = $metadataCache[$metadataPath]
    if ([string]::IsNullOrWhiteSpace($metadata.NodeName)) {
        continue
    }

    [xml]$xml = Get-Content -Path $xmlFile.FullName -Raw
    foreach ($node in $xml.SelectNodes("//$($metadata.NodeName)")) {
        foreach ($attr in $node.Attributes) {
            if (!$metadata.Allowed.Contains($attr.Name)) {
                $relativePath = $xmlFile.FullName.Substring($resolvedModRoot.Length).TrimStart('\')
                $errors.Add("${relativePath}: <$($metadata.NodeName) id=""$($node.id)""> has unsupported attribute '$($attr.Name)'")
            }
        }
    }
}

if ($errors.Count -gt 0) {
    $errors | ForEach-Object { Write-Host $_ }
    exit 1
}

Write-Host "Mod XML metadata validation passed."




