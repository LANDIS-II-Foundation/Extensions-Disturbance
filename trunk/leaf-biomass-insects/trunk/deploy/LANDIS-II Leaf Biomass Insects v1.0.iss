#define PackageName      "Leaf Biomass Insects"
#define PackageNameLong  "Leaf Biomass Insects"
#define Version          "1.1"
#define ReleaseType      "official"
#define ReleaseNumber    "1"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include "J:\Scheller\LANDIS-II\deploy\package (Setup section) v6.0.iss"
#define ExtDir "C:\Program Files\LANDIS-II\v6\bin\extensions"
#define AppDir "C:\Program Files\LANDIS-II\v6\"

[Files]

Source: ..\src\bin\debug\Landis.Extension.LeafBiomassInsects.dll; DestDir: {#ExtDir}; Flags: replacesameversion
Source: ..\src\bin\debug\Landis.Library.Metadata.dll; DestDir: {#ExtDir}; Flags: replacesameversion

[Files]

Source: docs\LANDIS-II Leaf Biomass Insect Defoliation v1.1 User Guide.pdf; DestDir: {#AppDir}\doc
Source: examples\*.txt; DestDir: {#AppDir}\examples\biomass-insects; Flags: recursesubdirs
Source: examples\*.gis; DestDir: {#AppDir}\examples\biomass-insects; Flags: recursesubdirs
Source: examples\*.bat; DestDir: {#AppDir}\examples\biomass-insects; Flags: recursesubdirs

#define BioBugs "Leaf Biomass Insects 1.1.txt"
Source: {#BioBugs}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add entries for each plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Biomass Insects"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#BioBugs}"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]
;; Run plug-in admin tool to remove entries for each plug-in

[Code]
#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Code section) v3.iss"

//-----------------------------------------------------------------------------

function CurrentVersion_PostUninstall(currentVersion: TInstalledVersion): Integer;
begin
end;

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  CurrVers_PostUninstall := @CurrentVersion_PostUninstall
  Result := True
end;
