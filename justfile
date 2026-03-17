set shell := ["bash", "-cu"]

default:
    just -l

release:
    dotnet publish -c Release -r linux-x64 -p:PublishSingleFile=true --self-contained false

