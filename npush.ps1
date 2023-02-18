# param([string]$version)
$path = "c:\nuget\*.nupkg";
$path1 = "c:\nuget\*.nupkg";

if ([string]::IsNullOrEmpty($env:NuGetApiKey))
{
    Write-Host "Please set NuGetApiKey environment variable!"
    Exit 2
}

Get-ChildItem -Path $path | Foreach { dotnet nuget push $_.fullname -k $env:NuGetApiKey -s https://api.nuget.org/v3/index.json --skip-duplicate }

Get-ChildItem -Path $path1 | Foreach { dotnet nuget push $_.fullname -k $env:NuGetApiKey -s https://api.nuget.org/v3/index.json --skip-duplicate }
