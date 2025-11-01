; DownSort Installer Script
; Inno Setup 6.x required
; https://jrsoftware.org/isinfo.php

#define MyAppName "DownSort"
#define MyAppVersion "1.0.2"
#define MyAppPublisher "DownSort Team"
#define MyAppURL "https://github.com/BaeTab/downsort"
#define MyAppExeName "Downsort.exe"

[Setup]
; NOTE: The value of AppId uniquely identifies this application.
AppId={{8F5A9D3C-2B1E-4C7A-9F8D-5E3A1B4C6D9F}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
AllowNoIcons=yes
; LicenseFile=..\LICENSE.txt
; InfoBeforeFile=..\README.md
OutputDir=..\Installer
OutputBaseFilename=DownSort-Setup-{#MyAppVersion}
; SetupIconFile=..\Downsort\Resources\app.ico
Compression=lzma2/ultra64
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64compatible
ArchitecturesAllowed=x64compatible
MinVersion=10.0
PrivilegesRequired=admin
DisableProgramGroupPage=yes
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "korean"; MessagesFile: "compiler:Languages\Korean.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "startupicon"; Description: "�ý��� ���� �� �ڵ� ����"; GroupDescription: "�߰� �ɼ�:"; Flags: unchecked

[Files]
; Main Application - Use correct publish path
Source: "..\Downsort\bin\Release\net8.0-windows\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "{cm:LaunchProgram,{#StringChange(MyAppName, '&', '&&')}}"; Flags: nowait postinstall skipifsilent

[UninstallDelete]
Type: filesandordirs; Name: "{localappdata}\DownSort"

[Registry]
; Startup registry entry
Root: HKCU; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "DownSort"; ValueData: "{app}\{#MyAppExeName}"; Flags: uninsdeletevalue; Tasks: startupicon

; Application settings
Root: HKCU; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "InstallPath"; ValueData: "{app}"; Flags: uninsdeletekey
Root: HKCU; Subkey: "Software\{#MyAppPublisher}\{#MyAppName}"; ValueType: string; ValueName: "Version"; ValueData: "{#MyAppVersion}"

[Code]
procedure CurStepChanged(CurStep: TSetupStep);
begin
  if CurStep = ssPostInstall then
  begin
    // Create default configuration directory
    CreateDir(ExpandConstant('{localappdata}\DownSort'));
  end;
end;

procedure CurUninstallStepChanged(CurUninstallStep: TUninstallStep);
var
  DialogResult: Integer;
begin
  if CurUninstallStep = usUninstall then
  begin
    DialogResult := MsgBox('����� ������(����, ��Ģ, �α�)�� �����Ͻðڽ��ϱ�?' + #13#10 + 
                           '�ƴϿ��� �����ϸ� �缳ġ �� �����Ͱ� �����˴ϴ�.', 
                           mbConfirmation, MB_YESNO);
    if (DialogResult = IDYES) then
    begin
      DelTree(ExpandConstant('{localappdata}\DownSort'), True, True, True);
    end;
  end;
end;

[CustomMessages]
korean.LaunchProgram=DownSort ����
korean.CreateDesktopIcon=����ȭ�� ������ �����
korean.AdditionalIcons=�߰� ������:
korean.UninstallProgram=����


