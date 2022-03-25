#!/bin/bash

#install zip on debian OS, since microsoft/dotnet container doesn't have zip by default
if [ -f /etc/debian_version ]
then
  apt -qq update
  apt -qq -y install zip
  apt-get -qq -y install libmagic-dev
fi

#dotnet restore
dotnet tool install --global Amazon.Lambda.Tools --version 4.0.0


# (for CI) ensure that the newly-installed tools are on PATH
if [ -f /etc/debian_version ]
then
  export PATH="$PATH:/$(whoami)/.dotnet/tools"
fi

dotnet restore
dotnet lambda package --configuration release --framework netcoreapp3.1 --output-package ./bin/release/netcoreapp3.1/documents-api.zip

cd python && zip -rv ../lambda-orchestrator.zip ./*
