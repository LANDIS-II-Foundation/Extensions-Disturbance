#define PackageName      "Base Fire"
#define PackageNameLong  "Base Fire Extension"
#define Version          "3.0.1"
#define ReleaseType      "official"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section) v6.0.iss"

[Files]

Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Extension.BaseFire.dll; DestDir: {app}\bin; Flags: replacesameversion

; Base Fire
Source: docs\LANDIS-II Base Fire v3.0 User Guide.pdf; DestDir: {app}\docs
Source: examples\*; DestDir: {app}\examples\base-fire

#define BaseFire "Base Fire 3.0.txt"
Source: {#BaseFire}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add the entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Base Fire"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#BaseFire}"" "; WorkingDir: {#LandisPlugInDir}


[Code]
{ Check for other prerequisites during the setup initialization }
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
