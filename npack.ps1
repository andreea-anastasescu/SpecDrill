del c:\nuget\*.nupkg
del c:\nuget\*.snupkg
dotnet pack .\SpecDrill.Infrastructure\SpecDrill.Infrastructure.csproj -o C:\nuget -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg
dotnet pack .\SpecDrill\SpecDrill.csproj -o C:\nuget -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --source c:\nuget
dotnet pack .\SpecDrill.Secondary.Adapters.WebDriver\SpecDrill.Secondary.Adapters.WebDriver.csproj -o C:\nuget -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --source c:\nuget
dotnet pack .\SpecDrill.MsTest\SpecDrill.MsTest.csproj -o C:\nuget -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --source c:\nuget
dotnet pack .\SpecDrill.NUnit3\SpecDrill.NUnit3.csproj -o C:\nuget -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --source c:\nuget
dotnet pack .\SpecDrill.SpecFlow\SpecDrill.SpecFlow.csproj -o C:\nuget -p:IncludeSymbols=true -p:SymbolPackageFormat=snupkg --source c:\nuget

 
