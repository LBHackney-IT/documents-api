#!/bin/bash

if [ -f /etc/debian_version ]
then
  apt -qq update
  apt -qq -y install xz-utils
fi

mkdir bin

curl -O https://nodejs.org/dist/v22.21.1/node-v22.21.1-linux-x64.tar.xz

unxz node-v22.21.1-linux-x64.tar.xz
tar -xf node-v22.21.1-linux-x64.tar

mv node-v22.21.1-linux-x64/bin/node bin/

rm -rf node-v22.21.1-linux-x64*
