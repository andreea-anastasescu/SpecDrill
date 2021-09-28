param ([Parameter(Mandatory)]$csproj)
function xmlPeek($filePath, $xpath) {
    [xml] $fileXml = Get-Content $filePath
    return $fileXml.SelectSingleNode($xpath).InnerText
}

function xmlPoke($filePath, $xpath, $value) {
    
    [xml] $fileXml = New-Object XML
    $fileXml.Load($filePath)
    $node = $fileXml.SelectSingleNode($xpath)
    if ($node) {
        Write-Host "Setting Next ver: $($node.Name) <- $($value)"
        $node.InnerText = $value
        Write-Host "Value set to: $($node.InnerText). Saving..."
        
        $fileXml.Save($filePath)
        Write-Host "Saved!"
    }
}

$csproj = Resolve-Path $csproj
Write-Host "Csproj: $($csproj)"
$xpath = "/Project/PropertyGroup/BuildNumber"

[int]$currentVer = 0
$currentVer = xmlPeek -filePath $csproj -xpath $xpath
Write-Host "Found ver: $($currentVer)"
$nextVer = $($currentVer + 1)
Write-Host "Next ver will be: $($nextVer)"

xmlPoke -filePath $csproj -xpath $xpath -value $nextVer