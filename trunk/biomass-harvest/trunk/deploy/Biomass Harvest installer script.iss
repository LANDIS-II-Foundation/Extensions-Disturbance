#define PackageName      "Biomass Harvest"
#define PackageNameLong  "Biomass Harvest Extension"
#define Version          "1.3.1"

#define ReleaseType      "official"
#define ReleaseNumber    "1"

; #include "build/release-info.iss"

#define CoreVersion      "5.1"
#define CoreReleaseAbbr  ""

; #define LandisSDK        GetEnv("LANDIS_SDK")
; #include AddBackslash(LandisSDK) + "deployment\windows\package (Setup section).iss"
#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section).iss"


#if ReleaseType != "official"
  #define Configuration  "debug"
#else
  #define Configuration  "release"
#endif

[Files]
; Base Harvest
Source: C:\Program Files\LANDIS-II\5.1\bin\Landis.Extension.BaseHarvest.dll; DestDir: {app}\bin; Flags: replacesameversion

; Cohort and Succession Libraries
; Source: {#LandisBuildDir}\libraries\biomass-cohort\build\release\Landis.Library.Cohorts.Biomass.dll; DestDir: {app}\bin; Flags: replacesameversion uninsneveruninstall

; The extension's assembly
Source: C:\Program Files\LANDIS-II\5.1\bin\Landis.Extension.BiomassHarvest.dll; DestDir: {app}\bin; Flags: replacesameversion

; The extension's program database file (for debugging) - only if not official release
; #if ReleaseType != "official"
;  ; Source: ..\src\build\{#Configuration}\Landis.Extensions.BiomassHarvest.pdb; DestDir: {app}\bin
;#endif

; The user guide
Source: docs\LANDIS-II Biomass Harvest v1.3.1 User Guide.pdf; DestDir: {app}\docs; DestName: {#PackageName} {#Version}{#ReleaseAbbr} User Guide.html

; Sample input file
Source: examples\BiomassHarvest-v1.2-Sample-Input.txt; DestDir: {app}\examples

; The extension's info file
#define ExtensionInfoFile PackageName + " " + Version + ReleaseAbbr+ ".txt"
Source: extension info.txt; DestDir: {#LandisPlugInDir}; DestName: {#ExtensionInfoFile}


[Run]
;; Run plug-in admin tool to add an entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"
Filename: {#PlugInAdminTool}; Parameters: "remove ""Biomass Harvest"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""{#ExtensionInfoFile}"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]
;; Run plug-in admin tool to remove the entry for the plug-in
; Filename: {#PlugInAdminTool}; Parameters: "remove ""Biomass Harvest"" "; WorkingDir: {#LandisPlugInDir}

[Code]
#include AddBackslash(LandisDeployDir) + "package (Code section) v3.iss"


//-----------------------------------------------------------------------------

function CurrentVersion_PostUninstall(currentVersion: TInstalledVersion): Integer;
begin
  // Alpha and beta releases of version 1.0 don't remove the plug-in name from
  // database
  //if StartsWith(currentVersion.Version, '1') then
  //  begin
  //    Exec('{#PlugInAdminTool}', 'remove "Biomass Harvest"',
  //         ExtractFilePath('{#PlugInAdminTool}'),
//		   SW_HIDE, ewWaitUntilTerminated, Result);
	//end
  //else
    Result := 0;
end;
//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  Result := True
end;
