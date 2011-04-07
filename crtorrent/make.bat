@echo off
IF EXIST %WINDIR%\Microsoft.NET\Framework64\v4.0.30319\csc.exe GOTO SETX64

set PATH=%PATH%;%WINDIR%\Microsoft.NET\Framework\v4.0.30319\
pause

IF NOT EXIST %WINDIR%\Microsoft.NET\Framework\v4.0.30319\ goto NO_NET_FRAMEWORK


:Compile
del /F bin
mkdir bin

Echo building:
Echo ---------
csc /optimize /reference:system.dll /reference:system.core.dll /main:crtorrent.Program /target:exe /out:bin/crtorrent.exe *.cs Bencode\*.cs
Echo ---------
GOTO END

:SETX64
Echo "Selecting x64..."
Echo.
Echo.
set PATH=%PATH%;C:\Windows\Microsoft.NET\Framework64\v4.0.30319\
GOTO COMPILE

:NO_NET_FRAMEWORK
ECHO "NO .NET Framework Found or wrong version"
ECHO "Check if .NET v4.0.30319 is installed and try again"
Echo. 
Echo.
Goto END

:END
Echo.
Echo.
pause
