param([string]$version)
$prefix = "NuGetPackages\SpecDrill.";
$suffix = ".nupkg";
$specDrill = $prefix + "" + $version + $suffix;
$specDrillWDAdapter = $prefix + "SecondaryPorts.Adapters.WebDriver." + $version + $suffix;
$specDrillMsTest = $prefix + "MsTest." + $version + $suffix;
$specDrillSpecFlow = $prefix + "SpecFlow." + $version + $suffix;

if ([string]::IsNullOrEmpty($version))
{
    Write-Host "Please provide package(s) version"
    Exit 1
}

if ([string]::IsNullOrEmpty($env:NuGetApiKey))
{
    Write-Host "Please set NuGetApiKey environment variable!"
    Exit 2
}
dotnet nuget push $specDrill -k $env:NuGetApiKey -s https://api.nuget.org/v3/index.json
dotnet nuget push $specDrillWDAdapter -k $env:NuGetApiKey -s https://api.nuget.org/v3/index.json
dotnet nuget push $specDrillMsTest -k $env:NuGetApiKey -s https://api.nuget.org/v3/index.json
dotnet nuget push $specDrillSpecFlow -k $env:NuGetApiKey -s https://api.nuget.org/v3/index.json