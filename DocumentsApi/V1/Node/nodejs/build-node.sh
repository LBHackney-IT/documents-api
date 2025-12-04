#!/bin/bash

if [ -f /etc/debian_version ]
then
  apt -qq update
  apt -qq -y install xz-utils
fi

# Create bin directory
mkdir bin


# Download Node v22 (Linux x64)
curl -O https://nodejs.org/dist/v22.22.1/node-v22.21.1-linux-x64.tar.xz

# Extract contents
tar -xjf node-v22.21.1-linux-x64.tar.xz

# Move binary to bin folder
mv node-v22.22.1-linux-x64/bin/node bin/

# Cleanup
rm -rf node-v22.22.1-linux-x64*
