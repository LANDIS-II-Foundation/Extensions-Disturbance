#define PackageName      "Biomass Harvest"
#define PackageNameLong  "Biomass Harvest Extension"
#define Version          "1.0"
#define ReleaseType      "alpha"
#define ReleaseNumber    "1"

#define CoreVersion      "5.1"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section).iss"

#if ReleaseType != "official"
  #define Configuration  "debug"
#else
  #define Configuration  "release"
#endif

[Files]
; The extension's assembly
Source: ..\src\build\{#Configuration}\Landis.Extensions.BiomassHarvest.dll; DestDir: {app}\bin

; The extension's program database file (for debugging) - only if not official release
#if ReleaseType != "official"
  ; Source: ..\src\build\{#Configuration}\Landis.Extensions.BiomassHarvest.pdb; DestDir: {app}\bin
#endif

; The user guide
Source: ..\doc\build\user-guide.html; DestDir: {app}\doc; DestName: Biomass Harvest 1.0a1 User Guide.html

; Sample input file
Source: biomass-harvest.txt; DestDir: {app}\examples

; The extension's info file
#define ExtensionInfoFile "Biomass Harvest 1.0a1.txt"
Source: extension info.txt; DestDir: {#LandisPlugInDir}; DestName: {#ExtensionInfoFile}


[Run]
;; Run plug-in admin tool to add an entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "add ""{#ExtensionInfoFile}"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]
;; Run plug-in admin tool to remove the entry for the plug-in
Filename: {#PlugInAdminTool}; Parameters: "remove ""Biomass Harvest"" "; WorkingDir: {#LandisPlugInDir}

[Code]
#include AddBackslash(LandisDeployDir) + "package (Code section).iss"

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  Result := True
end;
