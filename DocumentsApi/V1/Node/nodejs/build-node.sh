#!/bin/bash

# Create bin directory
mkdir bin

# Download Node v22 (Linux x64)
curl -sL https://deb.nodesource.com/setup_22.x | bash -
apt-get update && apt-get install -y nodejs

# Extract contents
#tar -xjf node-v22.21.1-linux-x64.tar.xz

# Move binary to bin folder
#mv node-v22.22.1-linux-x64/bin/node bin/

# Cleanup
#rm -rf node-v22.22.1-linux-x64*
