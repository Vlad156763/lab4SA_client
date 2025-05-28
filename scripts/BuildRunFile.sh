#!/bin/bash
dotnet publish -c Release -r linux-x64 --self-contained true -p:PublishSingleFile=true -p:DebugType=None -p:DebugSymbols=false -p:IncludeNativeLibrariesForSelfExtract=true -o ./publish
