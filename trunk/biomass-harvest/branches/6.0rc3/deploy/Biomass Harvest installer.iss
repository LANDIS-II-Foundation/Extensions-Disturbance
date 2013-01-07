#define PackageName      "Biomass Harvest"
#define PackageNameLong  "Biomass Harvest Extension"
#define Version          "2.0.4"

#define ReleaseType      "official"
#define ReleaseNumber    "2"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section) v6.0.iss"


[Files]
#define BuildDir "C:\Program Files\LANDIS-II\6.0\bin"

; Base Harvest
Source: {#BuildDir}\Landis.Extension.BaseHarvest.dll; DestDir: {app}\bin; Flags: replacesameversion

; Cohort and Succession Libraries
Source: {#BuildDir}\Landis.Library.BiomassCohorts.dll; DestDir: {app}\bin; Flags: replacesameversion uninsneveruninstall

; The extension's assembly
Source: {#BuildDir}\Landis.Extension.BiomassHarvest.dll; DestDir: {app}\bin; Flags: replacesameversion

; The user guide
Source: docs\LANDIS-II Biomass Harvest v2.0.4 User Guide.pdf; DestDir: {app}\docs; DestName: {#PackageName} {#Version}{#ReleaseAbbr} User Guide.html

; Sample input file
Source: examples\*; DestDir: {app}\examples\biomass-harvest

; The extension's info file
#define ExtensionInfoFile PackageName + " " + Version + ReleaseAbbr+ ".txt"
Source: Biomass Harvest.txt; DestDir: {#LandisPlugInDir}; DestName: {#ExtensionInfoFile}

[Run]
;; Run plug-in admin tool to add an entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"
Filename: {#PlugInAdminTool}; Parameters: "remove ""Biomass Harvest"" "; WorkingDir: {#LandisPlugInDir}
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
