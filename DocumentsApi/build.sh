#!/bin/bash

#install zip on debian OS, since microsoft/dotnet container doesn't have zip by default
if [ -f /etc/debian_version ]
then
  apt -qq update
  apt -qq -y install zip
fi

#dotnet restore
dotnet tool install --global Amazon.Lambda.Tools --version 5.12.4


# (for CI) ensure that the newly-installed tools are on PATH
if [ -f /etc/debian_version ]
then
  export PATH="$PATH:/$(whoami)/.dotnet/tools"
fi

dotnet restore
dotnet lambda package --configuration release --framework net8.0 --output-package ./bin/release/net8.0/documents-api.zip

# --- Setup Node Layer ---
cd DocumentsApi/V1/Node/nodejs

# Create bin directory
mkdir -p bin

# Download Node v22 (Linux x64)
curl -O https://nodejs.org/dist/v22.13.0/node-v22.13.0-linux-x64.tar.xz

# Extract contents
tar -xf node-v22.13.0-linux-x64.tar.xz

# Move binary to bin folder
mv node-v22.13.0-linux-x64/bin/node bin/

# Cleanup
rm -rf node-v22.13.0-linux-x64*

cd python && zip -rv ../lambda-orchestrator.zip ./*
