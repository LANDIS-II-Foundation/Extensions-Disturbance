#define PackageName      "Dynamic Fuels Leaf Biomass"
#define PackageNameLong  "Dynamic Fuels Leaf Biomass"
#define Version          "1.0"
#define ReleaseType      "official"
#define ReleaseNumber    "1"

#define CoreVersion      "5.1"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section).iss"

;#include "..\package (Setup section).iss"


[Files]

; Dynamic Fire Fuel System v1.0 plug-in and auxiliary libs (Troschuetz Random)
Source: {#LandisBuildDir}\disturbanceextensions\dynamic-leaf-biomass-fuels\build\release\Landis.Extension.Fuels.LeafBiomass.dll; DestDir: {app}\bin

; Source: docs\LANDIS-II Dynamic Biomass Fuel System v1.0 User Guide.pdf; DestDir: {app}\doc

#define DynFuelSys "Dynamic Fuels Leaf Biomass 1.0.txt"
Source: {#DynFuelSys}; DestDir: {#LandisPlugInDir}

; All the example input-files for the in examples\dynamic-fire-fuel-system
; Source: {#LandisBuildDir}\dynamic-biomass-fuels\deploy\examples\*; DestDir: {app}\examples\dynamic-fire-fuel-system; Flags: recursesubdirs

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
  // Remove the plug-in name from database
  if StartsWith(currentVersion.Version, '1.0') then
    begin
      Exec('{#PlugInAdminTool}', 'remove "Dynamic Biomass Fuel System"',
           ExtractFilePath('{#PlugInAdminTool}'),
		   SW_HIDE, ewWaitUntilTerminated, Result);
	end
  else
    Result := 0;
end;

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  CurrVers_PostUninstall := @CurrentVersion_PostUninstall
  Result := True
end;
