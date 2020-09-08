copy .\bin\Debug\netcoreapp3.1\allure-reports\history -Destination .\bin\Debug\netcoreapp3.1\allure-results -Recurse -Force
allure generate  .\bin\Debug\netcoreapp3.1\allure-results\ -o  .\bin\Debug\netcoreapp3.1\allure-reports\ --clean
allure open .\bin\Debug\netcoreapp3.1\allure-reports\