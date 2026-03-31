set shell := ["bash", "-cu"]

default:
    just -l

build:
    dotnet publish -c Release -r linux-x64 -o ./output
    
build-win:
    dotnet publish -c Release -r win-x64 -o ./output
    
update:
    dotnet tool install --global dotnet-outdated-tool
    dotnet outdated --version-lock major --upgrade

run: update build
    #!/usr/bin/env bash
    cd ./output
    ./OsuApi