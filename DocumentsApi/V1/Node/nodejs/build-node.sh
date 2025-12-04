#!/bin/bash

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
