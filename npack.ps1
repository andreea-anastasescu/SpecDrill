del .\NuGetPackages\*.nupkg
dotnet pack .\SpecDrill.Adapters.WebDriver\SpecDrill.SecondaryPorts.Adapters.WebDriver.csproj -o NuGetPackages
dotnet pack .\SpecDrill\SpecDrill.csproj -o NuGetPackages
dotnet pack .\SpecDrill.MsTest\SpecDrill.MsTest.csproj -o NuGetPackages
dotnet pack .\SpecDrill.NUnit3\SpecDrill.NUnit3.csproj -o NuGetPackages
dotnet pack .\SpecDrill.SpecFlow\SpecDrill.SpecFlow.csproj -o NuGetPackages
