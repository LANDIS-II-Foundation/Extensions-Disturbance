#include GetEnv("LANDIS_SDK") + '\packaging\initialize.iss'

#define ExtInfoFile "Biomass Harvest.txt"

#include LandisSDK + '\packaging\read-ext-info.iss'
#include LandisSDK + '\packaging\Landis-vars.iss'

[Setup]
#include LandisSDK + '\packaging\Setup-directives.iss'
LicenseFile=..\Apache-License-2.0.txt

[Files]
#define ConfigOutDir '..\src\bin\Debug'

; The extension's assembly
Source: {#ConfigOutDir}\{#ExtensionAssembly}.dll; DestDir: {app}\bin\extensions

; Supporting libraries
; Note: Since they are used by other extensions, they are not uninstalled.
Source: {#ConfigOutDir}\Landis.Library.BiomassHarvest-v0.dll;    DestDir: {app}\bin\extensions; Flags: uninsneveruninstall
Source: {#ConfigOutDir}\Landis.Library.HarvestManagement-v0.dll; DestDir: {app}\bin\extensions; Flags: uninsneveruninstall
Source: {#ConfigOutDir}\Landis.Library.SiteHarvest-v0.dll;       DestDir: {app}\bin\extensions; Flags: uninsneveruninstall

; The user guide
#define UserGuideSrc ExtensionName + " vX.Y User Guide.pdf"
#define UserGuide    StringChange(UserGuideSrc, "X.Y", MajorMinor)
Source: docs\{#UserGuideSrc}; DestDir: {app}\docs; DestName: {#UserGuide}

; Sample input files
Source: examples\*; DestDir: {app}\examples\{#ExtensionName}; Flags: recursesubdirs

; The extension's info file
#define ExtensionInfo  ExtensionName + " " + MajorMinor + ".txt"
Source: {#ExtInfoFile}; DestDir: {#LandisExtInfoDir}; DestName: {#ExtensionInfo}


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
