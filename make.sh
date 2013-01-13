#!/bin/sh
mkdir bin

dmcs -out:bin/crtorrent.exe -main:crtorrent.Program -reference:System.dll,System.Core.dll *.cs Bencode/*.cs