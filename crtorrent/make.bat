@echo off
set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework\v4.0.30319\


Echo building:
csc /optimize /reference:system.dll /reference:system.core.dll /main:crtorrent.Program /target:exe /out:crtorrent.exe *.cs Bencode\*.cs


pause