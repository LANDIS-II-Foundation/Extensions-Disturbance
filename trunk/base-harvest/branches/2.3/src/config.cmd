@echo off

set SCRIPT_DIR=%~dp0
rem trailing backslash
set SCRIPT_DIR=%SCRIPT_DIR:~,-1%

for /d %%K in ("%SCRIPT_DIR%\libs\Harvest\*") do set HARVEST_LIB_VER=%%~nxK
echo Harvest Library version  : %HARVEST_LIB_VER%

set HARVEST_VER_DIR=libs\Harvest\%HARVEST_LIB_VER%
for %%K in ("%HARVEST_VER_DIR%\Landis.Library.Harvest-v*.dll") do set HARVEST_LIB_NAME=%%~nxK
set HARVEST_LIB_PATH=%HARVEST_VER_DIR%\%HARVEST_LIB_NAME%

echo Harvest Library assembly : %HARVEST_LIB_PATH%

rem  Stage the Harvest library if it is not already in the build directory
set BUILD_DIR=C:\Program Files\LANDIS-II\v6\bin\build
set HARVEST_LIB_STAGED=%BUILD_DIR%\%HARVEST_LIB_NAME%
if exist "%HARVEST_LIB_STAGED%" (
  echo Harvest lib already staged at: %HARVEST_LIB_STAGED%
) else (
  call "%LANDIS_SDK%\staging\stage-assembly.cmd" "%HARVEST_LIB_PATH%"
)
