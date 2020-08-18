@echo off
TITLE Uninstall Service
:: You have to run this script as Administrator.
setlocal enableextensions
cd /d "%~dp0"
FOR /f %%X IN ('dir /b *Server.exe') DO CALL:S1 "%%X"
pause
GOTO:EOF

:S1
"%~1" /UnInstallService
echo.
GOTO:EOF
