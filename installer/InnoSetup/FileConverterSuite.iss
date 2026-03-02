#define MyAppName "File Converter Suite"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "Acme Software"
#define MyAppExeName "FileConverterSuite.Presentation.Wpf.exe"

[Setup]
AppId={{D4383A9B-94D7-4306-9E6D-B7AB03D1C8D1}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
DefaultDirName={autopf}\FileConverterSuite
DefaultGroupName=File Converter Suite
DisableProgramGroupPage=yes
OutputDir=..\..\artifacts\installer
OutputBaseFilename=FileConverterSuite-Setup-{#MyAppVersion}
Compression=lzma
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64
PrivilegesRequired=admin
SetupLogging=yes

[Languages]
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "Create a desktop icon"; GroupDescription: "Additional icons:"

[Files]
Source: "..\..\src\FileConverterSuite.Presentation.Wpf\bin\Release\net48\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs
Source: "..\..\third_party\tools\*"; DestDir: "{app}\tools"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\File Converter Suite"; Filename: "{app}\{#MyAppExeName}"
Name: "{autodesktop}\File Converter Suite"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "Launch File Converter Suite"; Flags: nowait postinstall skipifsilent
