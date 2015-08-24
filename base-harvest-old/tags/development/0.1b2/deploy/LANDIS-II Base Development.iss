#define PackageName      "Base Development"
#define PackageNameLong  "Base Development Extension"
#define Version          "0.1"
#define ReleaseType      "beta"
#define ReleaseNumber    "2"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section) v6.0.iss"

[Files]

; Base Development
Source: C:\Program Files\LANDIS-II\v6\bin\extensions\Landis.Extension.BaseDevelopment.dll; DestDir: {app}\bin; Flags: replacesameversion

;Source: docs\LANDIS-II Base Harvest v2.1.1 User Guide.pdf; DestDir: {app}\docs
;Source: examples\*; DestDir: {app}\examples\base-harvest; Flags: recursesubdirs

#define Development "Base Development 0.1.txt"
Source: {#Development}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add an entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Base Development"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#Development}"" "; WorkingDir: {#LandisPlugInDir}

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
