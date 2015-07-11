#include GetEnv("LANDIS_SDK") + '\packaging\initialize.iss'

#define ExtInfoFile "Biomass Harvest.txt"

#include LandisSDK + '\packaging\read-ext-info.iss'
#include LandisSDK + '\packaging\Landis-vars.iss'

[Setup]
#include LandisSDK + '\packaging\Setup-directives.iss'
LicenseFile={#LandisSDK}\licenses\LANDIS-II_Binary_license.rtf

[Files]
; The extension's assembly
Source: {#LandisBuildDir}\{#ExtensionAssembly}.dll; DestDir: {app}\bin\extensions

; Harvest libraries
; Note: Since they are used by other extensions, they are not uninstalled.
Source: {#LandisBuildDir}\Landis.Library.BiomassCohorts.dll;       DestDir: {app}\bin\extensions; Flags: uninsneveruninstall
Source: {#LandisBuildDir}\Landis.Library.Biomass-v1.dll;       DestDir: {app}\bin\extensions; Flags: uninsneveruninstall
Source: {#LandisBuildDir}\Landis.Library.BiomassHarvest-v1.dll;    DestDir: {app}\bin\extensions; Flags: uninsneveruninstall
Source: {#LandisBuildDir}\Landis.Library.HarvestManagement-v1.dll; DestDir: {app}\bin\extensions; Flags: uninsneveruninstall
Source: {#LandisBuildDir}\Landis.Library.SiteHarvest-v1.dll;       DestDir: {app}\bin\extensions; Flags: uninsneveruninstall

; The user guide
#define UserGuideSrc "LANDIS-II " + ExtensionName + " vX.Y User Guide.pdf"
#define UserGuide    StringChange(UserGuideSrc, "X.Y", MajorMinor)
Source: docs\{#UserGuideSrc}; DestDir: {app}\docs; DestName: {#UserGuide}

; Sample input files
Source: examples\*; DestDir: {app}\examples\{#ExtensionName}\{#MajorMinor}; Flags: recursesubdirs

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