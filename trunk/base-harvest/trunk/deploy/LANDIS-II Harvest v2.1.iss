#define PackageName      "Base Harvest"
#define PackageNameLong  "Base Harvest Extension"
#define Version          "2.1.1"
#define ReleaseType      "official"
#define ReleaseNumber    "2"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section) v6.0.iss"

[Files]

; Base Harvest
Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Extension.BaseHarvest.dll; DestDir: {app}\bin; Flags: replacesameversion

Source: docs\LANDIS-II Base Harvest v2.1.1 User Guide.pdf; DestDir: {app}\docs
Source: examples\*; DestDir: {app}\examples\base-harvest; Flags: recursesubdirs

#define Harvest "Base Harvest 2.1.txt"
Source: {#Harvest}; DestDir: {#LandisPlugInDir}

; Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Library.Succession.dll; DestDir: {app}\bin; Flags: replacesameversion uninsneveruninstall

[Run]
;; Run plug-in admin tool to add an entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Base Harvest"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#Harvest}"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]

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
