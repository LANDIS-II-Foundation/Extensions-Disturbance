#define PackageName      "Leaf Biomass Harvest"
#define PackageNameLong  "Leaf Biomass Harvest Extension"
#define Version          "1.0"

#define ReleaseType      "official"
#define ReleaseNumber    "1"

; #include "build/release-info.iss"

#define CoreVersion      "5.1"
#define CoreReleaseAbbr  ""

#define LandisSDK        GetEnv("LANDIS_SDK")
#include AddBackslash(LandisSDK) + "deployment\windows\package (Setup section).iss"

#if ReleaseType != "official"
  #define Configuration  "debug"
#else
  #define Configuration  "release"
#endif

[Files]
; The extension's assembly
Source: ..\build\{#Configuration}\Landis.Extension.LeafBiomassHarvest.dll; DestDir: {app}\bin

; The user guide
Source: ..\docs\LANDIS-II Biomass Harvest v1.0 User Guide.pdf; DestDir: {app}\doc; DestName: {#PackageName} {#Version}{#ReleaseAbbr} User Guide.html

; Sample input file
Source: leaf-biomass-harvest.txt; DestDir: {app}\examples

; The extension's info file
#define ExtensionInfoFile PackageName + " " + Version + ReleaseAbbr+ ".txt"
Source: extension info.txt; DestDir: {#LandisPlugInDir}; DestName: {#ExtensionInfoFile}


[Run]
;; Run plug-in admin tool to add an entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "add ""{#ExtensionInfoFile}"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]
;; Run plug-in admin tool to remove the entry for the plug-in
Filename: {#PlugInAdminTool}; Parameters: "remove ""Leaf Biomass Harvest"" "; WorkingDir: {#LandisPlugInDir}

[Code]
#include AddBackslash(LandisSDK) + "deployment\windows\package (Code section) v2.iss"

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  Result := True
end;
