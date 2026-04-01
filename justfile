set shell := ["bash", "-cu"]

default:
    just -l

build:
    dotnet publish -c Release -r linux-x64   
    mkdir -p output
    mv bin/Release/net9.0/linux-x64/publish/OsuApi ./output
    
build-win:
    dotnet publish -c Release -r win-x64
    mkdir -p output
    mv bin/Release/net9.0/win-x64/publish/OsuApi.exe ./output
    
update:
    dotnet tool install --global dotnet-outdated-tool
    dotnet outdated --version-lock major --upgrade

run: update build
    #!/usr/bin/env bash
    cd ./output
    ./OsuApi