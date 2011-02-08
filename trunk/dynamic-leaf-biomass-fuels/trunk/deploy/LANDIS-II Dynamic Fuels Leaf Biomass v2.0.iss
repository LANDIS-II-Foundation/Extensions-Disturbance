#define PackageName      "Dynamic Fuels Leaf Biomass"
#define PackageNameLong  "Dynamic Fuels Leaf Biomass"
#define Version          "2.0"
#define ReleaseType      "official"
#define ReleaseNumber    "2"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section).iss"

;#include "..\package (Setup section).iss"


[Files]

; Dynamic Fire Fuel System v1.0 plug-in and auxiliary libs (Troschuetz Random)
Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Extension.LeafBiomassFuels.dll; DestDir: {app}\bin

Source: docs\LANDIS-II Dynamic Fuels - Leaf Biomass v2.0 User Guide.pdf; DestDir: {app}\doc
Source: examples\*; DestDir: {app}\examples\leaf-biomass-fuels

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
#include AddBackslash(LandisDeployDir) + "package (Code section) v3.iss"

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
