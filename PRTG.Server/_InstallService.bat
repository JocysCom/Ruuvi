@echo off
TITLE Install Service
setlocal enableextensions
:: Change disk and go to local folder.
:: Line is needed if script was started as Administrator.
cd /D "%~d0%~p0"
:: You have to run this script as Administrator.
FOR /f %%X IN ('dir /b *Server.exe') DO CALL:S1 "%%X"
pause
GOTO:EOF

:S1
:: Install with default NETWORK SERVICE user.
"%~1" /InstallService
echo.
GOTO:EOF
