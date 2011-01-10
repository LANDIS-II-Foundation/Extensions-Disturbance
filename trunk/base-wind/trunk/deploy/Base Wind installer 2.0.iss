#define PackageName      "Base Wind"
#define PackageNameLong  "Base Wind Extension"
#define Version          "2.0"
#define ReleaseType      "official"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section).iss"

#if ReleaseType != "official"
  #define Configuration  "debug"
#else
  #define Configuration  "release"
#endif


[Files]

Source: C:\Program Files\LANDIS-II\6.0\bin\Landis.Extension.BaseWind.dll; DestDir: {app}\bin; Flags: replacesameversion

; Base Wind
Source: docs\LANDIS-II Base Wind v2.0 User Guide.pdf; DestDir: {app}\docs
Source: examples\*; DestDir: {app}\examples\BaseWind

#define BaseWind "Base Wind 2.0.txt"
Source: {#BaseWind}; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add the entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"

Filename: {#PlugInAdminTool}; Parameters: "remove ""Base Wind"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#BaseWind}"" "; WorkingDir: {#LandisPlugInDir}

[Code]
{ Check for other prerequisites during the setup initialization }
#include AddBackslash(LandisDeployDir) + "package (Code section) v3.iss"

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  Result := True
end;
