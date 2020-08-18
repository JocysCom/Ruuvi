@ECHO off
SET wra="%ProgramFiles%\WinRAR\winrar.exe"
IF NOT EXIST %wra% SET wra="%ProgramFiles(x86)%\WinRAR\winrar.exe"
IF NOT EXIST %wra% SET wra="%ProgramW6432%\WinRAR\winrar.exe"
IF NOT EXIST %wra% SET wra="D:\Program Files\WinRAR\winrar.exe"
SET zip=%wra% a -ep
:: ---------------------------------------------------------------------------
IF NOT EXIST Files\nul MKDIR Files
::-------------------------------------------------------------
:: Archive MSIL Application
CALL:CRE ..\bin\Debug     JocysCom.Ruuvi.PRTG.Server
ECHO.
pause
GOTO:EOF

::-------------------------------------------------------------
:CRE :: Create Archive
::-------------------------------------------------------------
SET src=%~1
SET arc=Files\%~2.zip
ECHO.
IF NOT EXIST "%src%\%~2.exe" (
  ECHO "%src%\%~2" not exist. Skipping.
  GOTO:EOF
)
ECHO Creating: %arc%
:: Create Archive.
IF EXIST %arc% DEL %arc%
%zip% %arc% %src%\_InstallService.bat
%zip% %arc% %src%\_UnInstallService.bat
%zip% %arc% %src%\%~2.exe
%zip% %arc% %src%\%~2.exe.config
GOTO:EOF