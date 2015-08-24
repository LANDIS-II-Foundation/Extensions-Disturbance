#define PackageName      "Biomass Development"
#define PackageNameLong  "Biomass Development Extension"
#define Version          "0.1"

#define ReleaseType      "beta"
#define ReleaseNumber    "4"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section) v6.0.iss"


[Files]
#define BuildDir "C:\Program Files\LANDIS-II\v6\bin\extensions"

; Base Development is distributed separately
;XXX; Source: {#BuildDir}\Landis.Extension.BaseHarvest.dll; DestDir: {app}\bin; Flags: replacesameversion

; The extension's assembly
Source: {#BuildDir}\Landis.Extension.BiomassDevelopment.dll; DestDir: {app}\bin; Flags: replacesameversion

;TO DO: update -- ; The user guide
;TO DO:        -- Source: docs\LANDIS-II Biomass Harvest v2.0.4 User Guide.pdf; DestDir: {app}\docs; DestName: {#PackageName} {#Version}{#ReleaseAbbr} User Guide.html

;TO DO: create -- ; Sample input file
;TO DO:        -- Source: examples\*; DestDir: {app}\examples\biomass-harvest

; The extension's info file
#define ExtensionInfoFile PackageName + " " + Version + ReleaseAbbr+ ".txt"
Source: Biomass Development.txt; DestDir: {#LandisPlugInDir}; DestName: {#ExtensionInfoFile}

[Run]
;; Run plug-in admin tool to add an entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"
Filename: {#PlugInAdminTool}; Parameters: "remove ""Biomass Development"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#ExtensionInfoFile}"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]

[Code]
#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Code section) v3.iss"


//-----------------------------------------------------------------------------

function CurrentVersion_PostUninstall(currentVersion: TInstalledVersion): Integer;
begin
    Result := 0;
end;
//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  Result := True
end;
