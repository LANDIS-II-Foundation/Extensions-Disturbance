#define PackageName      "Base Harvest"
#define PackageNameLong  "Base Harvest Extension"
#define Version          "2.2"
#define ReleaseType      "candidate"
#define ReleaseNumber    "1"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section) v6.0.iss"

[Files]

; Base Harvest
Source: C:\Program Files\LANDIS-II\v6\bin\extensions\Landis.Extension.BaseHarvest.dll; DestDir: {app}\bin;

#define UserGuideSrc PackageName + " vX.Y User Guide.pdf"
#define UserGuide    StringChange(UserGuideSrc, "X.Y", MajorMinor)
Source: docs\{#UserGuideSrc}; DestDir: {app}\docs; DestName: {#UserGuide}

Source: examples\*; DestDir: {app}\examples\base-harvest; Flags: recursesubdirs

; Extension info
#define ExtInfoSrc PackageName + ".txt"
#define ExtInfo    PackageName + " " + MajorMinor + ".txt"
Source: {#ExtInfoSrc}; DestDir: {#LandisPlugInDir}; DestName: {#ExtInfo}

; Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Library.Succession.dll; DestDir: {app}\bin; Flags: replacesameversion uninsneveruninstall

[Run]
;; Run plug-in admin tool to add an entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""{#PackageName}"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#ExtInfo}"" "; WorkingDir: {#LandisPlugInDir}

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
