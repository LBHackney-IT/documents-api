#!/bin/bash

#install zip on debian OS, since microsoft/dotnet container doesn't have zip by default
if [ -f /etc/debian_version ]
then
  apt -qq update
  apt -qq -y install zip
fi

rm -rf build
mkdir build
cp lambda-orchestrator.py build
apt-get install -y python3-pip
pip3 install -r ../requirements.txt --target build

zip -rv ../lambda-orchestrator.zip ./*