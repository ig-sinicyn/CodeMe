$ErrorActionPreference = 'Stop'
$PSNativeCommandUseErrorActionPreference = $true

dotnet restore
dotnet build -c Release --no-restore
dotnet pack -c Release -o .artifacts\packages --no-restore --no-build /p:Version="0.0.1-local"