# Package Validator
PackageValidator for .NET Application. This helps to check runtime dependencies for deployment package and to avoid any dll mising errors

## Run this utility using below command

  PackageValidator.exe -output:"<OUTPUT_PATH>" -package:"<PACKAGE_PATH>" -entry:"<Entry_DLL>"
  i.e 
  PackageValidator.exe -output:"C:\Projects\output" -package:"C:\Projects\temp2\WebApplication1\bin" -entry:"WebApplication1.dll"
