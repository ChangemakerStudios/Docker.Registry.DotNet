param(
    [string] $version
)

If ($version -eq '') {
    Write-Output "Please specify a version: E.g. 1.0"
    exit
}

Write-Output "build: Build started"

if (Test-Path .\artifacts) {
    Write-Output "build: Cleaning .\artifacts"
    Remove-Item .\artifacts -Force -Recurse
}

Write-Output "build: Version is $version"

& dotnet pack .\src\Docker.Registry.DotNet\Docker.Registry.DotNet.csproj -c Release -o .\artifacts /p:Version="$version"

if ($LASTEXITCODE -ne 0) { exit 1 }    

# foreach ($test in Get-ChildItem test/*.Tests) {
#     Push-Location $test
#     Write-Output "build: Testing project in $test"
#     & dotnet test -c Release
#     Pop-Location
#     if ($LASTEXITCODE -ne 0) { exit 3 }
# }