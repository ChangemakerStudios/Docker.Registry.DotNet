param(
    [string] $version
)

If ($version -eq '') {
    Write-Output "Please specify a version: E.g. 1.0"
    exit
}

Write-Output "build: Build started"

Push-Location $PSScriptRoot

if (Test-Path .\artifacts) {
    Write-Output "build: Cleaning .\artifacts"
    Remove-Item .\artifacts -Force -Recurse
}

& dotnet restore --no-cache
if ($LASTEXITCODE -ne 0) { exit 1 }    

Write-Output "build: Version is $version"

foreach ($src in ls src/Docker.Registry.DotNet) {
    Push-Location $src

    Write-Output "build: Packaging project in $src"

    & dotnet pack -c Release -o ..\..\artifacts /p:Version="$version"

    if ($LASTEXITCODE -ne 0) { exit 1 }    

    Pop-Location
}

foreach ($test in ls test/*.Tests) {
    Push-Location $test

    Write-Output "build: Testing project in $test"

    & dotnet test -c Release
    if ($LASTEXITCODE -ne 0) { exit 3 }

    Pop-Location
}

Pop-Location