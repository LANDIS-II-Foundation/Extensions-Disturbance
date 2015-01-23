@echo off
setlocal

set SCRIPT_DIR=%~dp0
rem trailing backslash
set SCRIPT_DIR=%SCRIPT_DIR:~,-1%

rem  -------------------------------------------------------------------------
rem  Install a library into the LANDIS-II build directory, so it's available
rem  for this project when the LANDIS-II model runs.

set LIB_NAME=%~1
set LIB_VER=%~2

echo Installing %LIB_NAME% library (version %LIB_VER%) ...

set LIB_DIR=%SCRIPT_DIR%\%LIB_NAME%\%LIB_VER%
if not exist "%LIB_DIR%" (
  echo Error: directory does not exist: %LIB_DIR:%
  exit /b 1
)

for /f %%V in ("%LIB_VER%") do (
  set MAJOR_VER=%%~nV
  set MINOR_VER=%%~xV
)
set MINOR_VER=%MINOR_VER:.=%

set ASSEMBLY_NAME=Landis.Library.%LIB_NAME%-v%MAJOR_VER%.dll
set ASSEMBLY_PATH=%LIB_DIR%\%ASSEMBLY_NAME%
if not exist "%ASSEMBLY_PATH%" (
  echo Error: file does not exist: %ASSEMBLY_PATH%
  exit /b 1
)

rem  Stage the library's assembly if it is not already in the build directory
set BUILD_DIR=C:\Program Files\LANDIS-II\v6\bin\build
set STAGED_LIB=%BUILD_DIR%\%ASSEMBLY_NAME%
if exist "%STAGED_LIB%" (
  echo Library already staged at: %STAGED_LIB%
) else (
  call "%LANDIS_SDK%\staging\stage-assembly.cmd" "%ASSEMBLY_PATH%"
  call :waitUntilFileExists "%STAGED_LIB%"
)
goto :eof

rem  -------------------------------------------------------------------------
rem  Wait until a file is created.  This is used since the staging task runs
rem  asynchronously, and uses a global file for argument-passing.

:waitUntilFileExists

if not exist %1 (
  rem  Wait a second (based on http://stackoverflow.com/a/735294/1258514)
  ping 127.0.0.1 -n 2 -w 1000 > nul
  goto :waitUntilFileExists
)
goto :eof
