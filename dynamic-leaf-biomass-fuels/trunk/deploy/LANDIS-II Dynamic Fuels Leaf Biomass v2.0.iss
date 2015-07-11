#define PackageName      "Dynamic Fuels Leaf Biomass"
#define PackageNameLong  "Dynamic Fuels Leaf Biomass"
#define Version          "2.1"
#define ReleaseType      "official"
#define ReleaseNumber    "2"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include "J:\Scheller\LANDIS-II\deploy\package (Setup section) v6.0.iss"
#define ExtDir "C:\Program Files\LANDIS-II\v6\bin\extensions"
#define AppDir "C:\Program Files\LANDIS-II\v6"

[Files]
Source: ..\src\bin\debug\Landis.Extension.LeafBiomassFuels.dll; DestDir: {#ExtDir}; Flags: replacesameversion

Source: docs\LANDIS-II Dynamic Fuels - Leaf Biomass v2.0 User Guide.pdf; DestDir: {#AppDir}\docs
Source: examples\ecoregions.gis; DestDir: {#AppDir}\examples\dynamic-leaf-biomass-fuels
Source: examples\initial-communities.gis; DestDir: {#AppDir}\examples\dynamic-leaf-biomass-fuels
Source: examples\*.txt; DestDir: {#AppDir}\examples\dynamic-leaf-biomass-fuels
Source: examples\*.bat; DestDir: {#AppDir}\examples\dynamic-leaf-biomass-fuels

#define DynFuelSys "Dynamic Fuels Leaf Biomass 2.0.txt"
Source: {#DynFuelSys}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add entries for each plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Dynamic Fuels Leaf Biomass"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#DynFuelSys}"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]
;; Run plug-in admin tool to remove entries for each plug-in

[Code]
#include "J:\Scheller\LANDIS-II\deploy\package (Code section) v3.iss"

//-----------------------------------------------------------------------------

function CurrentVersion_PostUninstall(currentVersion: TInstalledVersion): Integer;
begin
    Result := 0;
end;

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  CurrVers_PostUninstall := @CurrentVersion_PostUninstall
  Result := True
end;
