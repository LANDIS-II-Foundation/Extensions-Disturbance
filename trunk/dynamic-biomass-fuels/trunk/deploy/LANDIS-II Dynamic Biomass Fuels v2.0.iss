#define PackageName      "Dynamic Biomass Fuel System"
#define PackageNameLong  "Dynamic Biomass Fuel System"
#define Version          "2.0"
#define ReleaseType      "official"
#define ReleaseNumber    "2"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section) v6.0.iss"

[Files]
; Cohort and Succession Libraries
Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Library.BiomassCohorts.dll; DestDir: {app}\bin; Flags: replacesameversion uninsneveruninstall

; Dynamic Fire Fuel System v1.0 plug-in and auxiliary libs (Troschuetz Random)
Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Extension.BiomassFuels.dll; DestDir: {app}\bin; Flags: replacesameversion

Source: docs\LANDIS-II Dynamic Biomass Fuel System v2.0 User Guide.pdf; DestDir: {app}\docs
Source: examples\*; DestDir: {app}\examples\dynamic-biomass-fuels

#define DynFuelSys "Dynamic Biomass Fuels 2.0.txt"
Source: {#DynFuelSys}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add entries for each plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"
Filename: {#PlugInAdminTool}; Parameters: "remove ""Dynamic Biomass Fuel System"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#DynFuelSys}"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]
;; Run plug-in admin tool to remove entries for each plug-in
; Filename: {#PlugInAdminTool}; Parameters: "remove ""Dynamic Biomass Fuel System"" "; WorkingDir: {#LandisPlugInDir}

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
  CurrVers_PostUninstall := @CurrentVersion_PostUninstall
  Result := True
end;
