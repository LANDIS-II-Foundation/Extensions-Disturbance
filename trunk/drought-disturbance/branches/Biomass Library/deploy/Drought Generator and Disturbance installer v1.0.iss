#include GetEnv("LANDIS_SDK") + '\packaging\initialize.iss'

#define ExtInfoFile "Drought Disturbance v1.0.txt"

#include LandisSDK + '\packaging\read-ext-info.iss'
#include LandisSDK + '\packaging\Landis-vars.iss'

[Setup]
#include LandisSDK + '\packaging\Setup-directives.iss'
LicenseFile={#LandisSDK}\licenses\LANDIS-II_Binary_license.rtf

[Files]
Source: {#LandisExtDir}\{#ExtensionAssembly}.dll; DestDir: {app}\bin\extensions; Flags: replacesameversion
Source: {#LandisExtDir}\Landis.Extension.BaseHarvest.dll; DestDir: {app}\bin\extensions; Flags: replacesameversion

#define UserGuideSrc "LANDIS-II " + ExtensionName + " vX.Y User Guide.pdf"
#define UserGuide    StringChange(UserGuideSrc, "X.Y", MajorMinor)
Source: docs\{#UserGuide}; DestDir: {app}\docs; DestName: {#UserGuide}; Flags: replacesameversion

Source: examples\*; DestDir: {app}\examples\{#ExtensionName}; Flags: recursesubdirs replacesameversion

#define ExtensionInfo  ExtensionName + " " + MajorMinor + ".txt"
Source: {#ExtInfoFile}; DestDir: {#LandisExtInfoDir}; DestName: {#ExtensionInfo}; Flags: replacesameversion


[Run]
Filename: {#ExtAdminTool}; Parameters: "remove ""{#ExtensionName}"" "; WorkingDir: {#LandisExtInfoDir}
Filename: {#ExtAdminTool}; Parameters: "add ""{#ExtensionInfo}"" "; WorkingDir: {#LandisExtInfoDir}

[UninstallRun]
Filename: {#ExtAdminTool}; Parameters: "remove ""{#ExtensionName}"" "; WorkingDir: {#LandisExtInfoDir}

[Code]
#include LandisSDK + '\packaging\Pascal-code.iss'

//-----------------------------------------------------------------------------

function InitializeSetup_FirstPhase(): Boolean;
begin
  Result := True
end;




























#define PackageName      "Drought Generator and Disturbance"
#define PackageNameLong  "Drought Generator and Disturbance Extensions"
#define Version          "1.0"
#define ReleaseType      "official"
#define ReleaseNumber    "1"

#define CoreVersion      "6.0"
#define CoreReleaseAbbr  ""

#include AddBackslash(GetEnv("LANDIS_DEPLOY")) + "package (Setup section) v6.0.iss"


[Files]
#define BuildDir "C:\Program Files\LANDIS-II\6.0\bin"

; The extension's assembly
Source: C:\Program Files\LANDIS-II\6.0\bin\\Landis.Extension.DroughtDisturbance.dll; DestDir: {app}\bin; Flags: replacesameversion
Source: C:\Program Files\LANDIS-II\6.0\bin\\Landis.Extension.DroughtGenerator.dll; DestDir: {app}\bin; Flags: replacesameversion

; The user guide
Source: docs\LANDIS-II Drought Disturbance v1.0 User Guide.pdf; DestDir: {app}\docs
Source: docs\LANDIS-II Drought Generator v1.0 User Guide.pdf; DestDir: {app}\docs

; Sample input file
Source: examples\*; DestDir: {app}\examples\drought-disturbance

; The extension's info file
Source: Drought Disturbance v1.0.txt; DestDir: {#LandisPlugInDir}
Source: Drought Generator v1.0.txt; DestDir: {#LandisPlugInDir}

[Run]
;; Run plug-in admin tool to add an entry for the plug-in
#define PlugInAdminTool  CoreBinDir + "\Landis.PlugIns.Admin.exe"
Filename: {#PlugInAdminTool}; Parameters: "remove ""Drought Disturbance"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""Drought Disturbance v1.0.txt"" "; WorkingDir: {#LandisPlugInDir}

Filename: {#PlugInAdminTool}; Parameters: "remove ""Drought Generator"" "; WorkingDir: {#LandisPlugInDir}
Filename: {#PlugInAdminTool}; Parameters: "add ""Drought Generator v1.0.txt"" "; WorkingDir: {#LandisPlugInDir}

[UninstallRun]

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
  Result := True
end;
