#!/bin/bash

#install zip on debian OS, since microsoft/dotnet container doesn't have zip by default
if [ -f /etc/debian_version ]
then
  apt -qq update
  apt -qq -y install zip
fi

cd ./DocumentsApi/python
rm -rf build
mkdir build
cp lambda-orchestrator.py build
pip3 install -r requirements.txt --target build

zip -rv lambda-orchestrator.zip ./build/*