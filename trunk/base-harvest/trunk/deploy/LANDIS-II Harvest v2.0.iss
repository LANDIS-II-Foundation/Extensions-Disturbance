#define PackageName      "Base Harvest"
#define PackageNameLong  "Base Harvest Extension"
#define Version          "2.0"
#define ReleaseType      "official"
#define ReleaseNumber    "2"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section).iss"

#if ReleaseType != "official"
  #define Configuration  "debug"
#else
  #define Configuration  "release"
#endif

[Files]

; Base Harvest (v1.1)
Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Extension.BaseHarvest.dll; DestDir: {app}\bin; Flags: replacesameversion

Source: docs\LANDIS-II Base Harvest v2.0 User Guide.pdf; DestDir: {app}\docs
Source: examples\*; DestDir: {app}\examples\base-harvest; Flags: recursesubdirs

#define Harvest "Base Harvest 2.0.txt"
Source: {#Harvest}; DestDir: {#LandisPlugInDir}

; Harvest needs the succession library with planting (2.1+).  Until the
; the latest version of that library is released for the LANDIS-II main
; package, the library is included in this installer.  It's marked as
; uninstallable because if the package is uninstalled and this version
; of the Succession library is removed, then age-only succession will
; break
Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Library.Succession.dll; DestDir: {app}\bin; Flags: replacesameversion uninsneveruninstall

[Run]
;; Run plug-in admin tool to add an entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Base Harvest"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#Harvest}"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]
;; Run plug-in admin tool to remove the entry for the plug-in
; Filename: {#PlugInAdminTool}; Parameters: "remove ""Base Harvest"" "; WorkingDir: {#LandisPlugInDir}

;; Run plug-in admin tool to remove the entry for the plug-in
; Filename: {#PlugInAdminTool}; Parameters: "remove ""Base Harvest"" "; WorkingDir: {#LandisPlugInDir}

[Code]
#include AddBackslash(LandisDeployDir) + "package (Code section) v3.iss"

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
