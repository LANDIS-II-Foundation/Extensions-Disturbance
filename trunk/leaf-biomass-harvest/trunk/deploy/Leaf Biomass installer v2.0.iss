#define PackageName      "Leaf Biomass Harvest"
#define PackageNameLong  "Leaf Biomass Harvest Extension"
#define Version          "2.1"

#define ReleaseType      "official"
#define ReleaseNumber    "2"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include "J:\Scheller\LANDIS-II\deploy\package (Setup section) v6.0.iss"
#define ExtDir "C:\Program Files\LANDIS-II\v6\bin\extensions"
#define AppDir "C:\Program Files\LANDIS-II\v6\"


[Files]
; Base Harvest
Source: ..\src\bin\debug\Landis.Extension.BaseHarvest.dll; DestDir: {#ExtDir}; Flags: replacesameversion uninsneveruninstall

; The extension's assembly
Source: ..\src\bin\debug\Landis.Extension.LeafBiomassHarvest.dll; DestDir: {#ExtDir}; Flags: replacesameversion
Source: ..\src\bin\debug\Landis.Library.Metadata.dll; DestDir: {#ExtDir}; Flags: replacesameversion uninsneveruninstall

Source: docs\LANDIS-II Leaf Biomass Harvest v2.0 User Guide.pdf; DestDir: {#AppDir}\docs
Source: examples\*.txt; DestDir: {#AppDir}\examples\leaf-biomass-harvest; Flags: recursesubdirs
Source: examples\ecoregions.gis; DestDir: {#AppDir}\examples\leaf-biomass-harvest; Flags: recursesubdirs
Source: examples\initial-communities.gis; DestDir: {#AppDir}\examples\leaf-biomass-harvest; Flags: recursesubdirs
Source: examples\management.gis; DestDir: {#AppDir}\examples\leaf-biomass-harvest; Flags: recursesubdirs
Source: examples\stand.gis; DestDir: {#AppDir}\examples\leaf-biomass-harvest; Flags: recursesubdirs
Source: examples\*.bat; DestDir: {#AppDir}\examples\leaf-biomass-harvest; Flags: recursesubdirs

; The extension's info file
#define ExtensionInfoFile "Leaf Biomass Harvest v2.1.txt"
Source: {#ExtensionInfoFile}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add an entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Leaf Biomass Harvest"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#ExtensionInfoFile}"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]
;; Run plug-in admin tool to remove the entry for the plug-in

[Code]
#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Code section) v3.iss"

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  Result := True
end;
