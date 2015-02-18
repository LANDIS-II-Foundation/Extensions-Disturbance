@echo off
setlocal

set SCRIPT_DIR=%~dp0
rem trailing backslash
set SCRIPT_DIR=%SCRIPT_DIR:~,-1%

set INSTALL_LIB=%SCRIPT_DIR%\libs\install-lib.cmd

call "%INSTALL_LIB%" SiteHarvest 0.5
call "%INSTALL_LIB%" HarvestManagement 0.5
