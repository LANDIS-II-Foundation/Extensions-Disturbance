#define PackageName      "Biomass Harvest"
#define PackageNameLong  "Biomass Harvest Extension"
#define Version          "2.1"

#define ReleaseType      "beta"
#define ReleaseNumber    "1"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section) v6.0.iss"

#define CustomRevision "2"

#define OutputBaseFileName       SetupSetting("OutputBaseFileName")
#define OutputBaseFileNameCustom StringChange(OutputBaseFileName, "-setup", " (EventLog-r"+CustomRevision+")-setup")
#expr SetSetupSetting("OutputBaseFileName", OutputBaseFileNameCustom)


[Files]
#define BuildDir "C:\Program Files\LANDIS-II\v6\bin\extensions"

; Base Harvest
Source: {#BuildDir}\Landis.Extension.BaseHarvest.dll; DestDir: {app}\bin;

; The extension's assembly
Source: {#BuildDir}\Landis.Extension.BiomassHarvest.dll; DestDir: {app}\bin;

; The user guide
#define UserGuideSrc PackageName + " vX.Y User Guide.pdf"
#define UserGuide    StringChange(UserGuideSrc, "X.Y", MajorMinor)
Source: docs\{#UserGuideSrc}; DestDir: {app}\docs; DestName: {#UserGuide}

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
