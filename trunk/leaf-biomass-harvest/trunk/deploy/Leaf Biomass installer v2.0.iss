#define PackageName      "Leaf Biomass Harvest"
#define PackageNameLong  "Leaf Biomass Harvest Extension"
#define Version          "2.0.3"

#define ReleaseType      "official"
#define ReleaseNumber    "2"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

; #include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section) v6.0.iss"
#include "j:\scheller\landis-ii\deploy\package (Setup section) v6.0.iss"

#if ReleaseType != "official"
  #define Configuration  "debug"
#else
  #define Configuration  "release"
#endif

[Files]
#define BuildDir "C:\Program Files\LANDIS-II\v6\bin\extensions"
#define AppDir "C:\Program Files\LANDIS-II\v6"

; Base Harvest
Source: {#BuildDir}\Landis.Extension.BaseHarvest.dll; DestDir: {app}\bin; Flags: replacesameversion

; The extension's assembly
Source: {#BuildDir}\Landis.Extension.LeafBiomassHarvest.dll; DestDir: {app}\bin; Flags: replacesameversion

Source: docs\LANDIS-II Leaf Biomass Harvest v2.0 User Guide.pdf; DestDir: {#AppDir}\docs
Source: examples\*; DestDir: {#AppDir}\examples\leaf-biomass-harvest; Flags: recursesubdirs

; The extension's info file
#define ExtensionInfoFile "Leaf Biomass Harvest v2.0.txt"
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
