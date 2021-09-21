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
Write-Host "Setting Nex ver to:"
Write-Host $node.Name
Write-Host $value
        $node.InnerText = $value
Write-Host "Value set to:"
Write-Host $node.InnerText
Write-Host "Saving file..."
        $fileXml.Save($filePath)
Write-Host "Saved!"
    }
}
$csproj = Resolve-Path $csproj
Write-Host "Csproj:"
Write-Host $csproj
$xpath = "/Project/PropertyGroup/BuildNumber"
[int]$currentVer = 0
$currentVer = xmlPeek -filePath $csproj -xpath $xpath
Write-Host "Found:"
Write-Host $currentVer
$nextVer = $($currentVer + 1)
Write-Host "Next ver will be:"
Write-Host $nextVer

xmlPoke -filePath $csproj -xpath $xpath -value $nextVer