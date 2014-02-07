#define PackageName      "Biomass Harvest"
#define PackageNameLong  "Biomass Harvest Extension"
#define Version          "2.2"
#define ReleaseType      "official"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include "J:\Scheller\LANDIS-II\deploy\package (Setup section) v6.0.iss"
#define ExtDir "C:\Program Files\LANDIS-II\v6\bin\extensions"
#define AppDir "C:\Program Files\LANDIS-II\v6\"



[Files]
; Base Harvest
Source: {#ExtDir}\Landis.Extension.BaseHarvest.dll; DestDir: {#ExtDir}; Flags: replacesameversion

; The extension's assembly
Source: ..\src\bin\Debug\Landis.Extension.BiomassHarvest.dll; DestDir: {#ExtDir}; Flags: replacesameversion

; The user guide
; Source: docs\LANDIS-II Base Wind v2.1 User Guide.pdf; DestDir: {#AppDir}\docs
Source: examples\*; DestDir: {#AppDir}\examples\biomass-harvest

; The extension's info file
; #define ExtensionInfo  ExtensionName + " " + MajorMinor + ".txt"
; Source: {#ExtInfoFile}; DestDir: {#LandisExtInfoDir}; DestName: {#ExtensionInfo}

#define BiomassHarvest "Biomass Harvest.txt"
Source: {#BiomassHarvest}; DestDir: {#LandisPlugInDir}

[Run]
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Biomass Harvest"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#BiomassHarvest}"" "; WorkingDir: {#LandisPlugInDir}

[Code]
{ Check for other prerequisites during the setup initialization }
#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Code section) v3.iss"

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  Result := True
end;
