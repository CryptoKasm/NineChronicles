#!/usr/bin/env bash

#-----------------------------------------
# Tasks
#   - Check for Unity executable
#   - Clean old build
#   - Configure hooks & update
#   - Build player
#   - Copy player to ./dist
#-----------------------------

if [[ "$#" != "1" ]]; then
  {
    echo "error: too few arguments"
    echo "usage: $0 --<buildTarget>"
  } > /dev/stderr
  exit 1
fi

buildTarget="$1"
buildDir="./nekoyume/Build"
editorDir="$HOME/Unity/Hub/Editor"
unity="$editorDir/2020.3.4f1/Editor/Unity"

if [ -x "$(command -v Unity)" ]; then
  unity="Unity"
elif [ -x "$UNITYPATH" ]; then
  unity="$UNITYPATH"
elif [ ! -x "$unity" ]; then
  echo "Unable to find Unity: 1) export UNITYPATH='path/to/Unity' or 2) use script: install-unity.sh --<buildTarget>"
  exit 1
fi

if [ "$buildTarget" == "--mac" ]; then
  buildTarget="MacOS"
elif [ "$buildTarget" == "--linux" ]; then
  buildTarget="Linux"
fi

currentBuild="$buildDir/$buildTarget"

#-----------------------------

rm -rf $currentBuild

git config core.hooksPath hooks
git submodule update --init --recursive

$unity \
-quit \
-batchmode \
-nographics \
-projectPath=./nekoyume/ \
-executeMethod "Editor.Builder.Build""$buildTarget"

if [ ! -d ./dist ]; then
    mkdir ./dist
fi

if [ -d $currentBuild ]; then
    cp -R $currentBuild/* ./dist/
fi

#-----------------------------------------