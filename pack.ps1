$csprojFile = "$pwd\$((Get-ChildItem *.csproj | Select-Object -first 1).Name)"
$csproj = [xml] (Get-Content $csprojFile)
Write-Host "Version"
([System.String]$csproj.Project.PropertyGroup.Version)
$vparts = ([System.String]$csproj.Project.PropertyGroup.Version).Split(new char ".",[System.StringSplitOptions]::RemoveEmptyEntries)
Write-Host "Parts (Count = $($vParts.Length)"
$vparts
$vparts[$vparts.Length-1] = ([System.Int32]::Parse($vparts[$vparts.Length-1])+1).ToString();
$vNext = $([System.String]::Join('.',$vparts))
Write-Host "vCurrent"
$vparts
Write-Host "vNext: $vNext"
Select-Xml -xml $csproj -XPath //Project/PropertyGroup/Version | % { $_.Node.'#text' = $vNext }
$csproj.Project.PropertyGroup.Version
Write-Host "Project file to be changed: $csprojFile"
$csproj.Save($csprojFile)

dotnet pack $csprojFile -o c:\nuget
