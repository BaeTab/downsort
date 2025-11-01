; DownSort Installer Script
; Inno Setup 6.x required
; https://jrsoftware.org/isinfo.php

#define MyAppName "DownSort"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "DownSort Team"
#define MyAppURL "https://github.com/yourusername/downsort"
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
ArchitecturesInstallIn64BitMode=x64
ArchitecturesAllowed=x64
MinVersion=10.0
PrivilegesRequired=admin
DisableProgramGroupPage=yes
UninstallDisplayIcon={app}\{#MyAppExeName}

[Languages]
Name: "korean"; MessagesFile: "compiler:Languages\Korean.isl"
Name: "english"; MessagesFile: "compiler:Default.isl"

[Tasks]
Name: "desktopicon"; Description: "{cm:CreateDesktopIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked
Name: "quicklaunchicon"; Description: "{cm:CreateQuickLaunchIcon}"; GroupDescription: "{cm:AdditionalIcons}"; Flags: unchecked; OnlyBelowVersion: 6.1; Check: not IsAdminInstallMode
Name: "startupicon"; Description: "시스템 시작 시 자동 실행"; GroupDescription: "추가 옵션:"; Flags: unchecked

[Files]
; Main Application - Adjust path to match your actual publish output
Source: "..\Downsort\bin\Release\net8.0-windows\win-x64\publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

; Documentation (optional - uncomment if files exist)
; Source: "..\README.md"; DestDir: "{app}"; Flags: ignoreversion isreadme
; Source: "..\USER_GUIDE.md"; DestDir: "{app}"; Flags: ignoreversion
; Source: "..\CHANGELOG.md"; DestDir: "{app}"; Flags: ignoreversion

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\{cm:UninstallProgram,{#MyAppName}}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon
Name: "{userappdata}\Microsoft\Internet Explorer\Quick Launch\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: quicklaunchicon

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
    DialogResult := MsgBox('사용자 데이터(설정, 규칙, 로그)를 삭제하시겠습니까?' + #13#10 + 
                           '아니오를 선택하면 재설치 시 데이터가 유지됩니다.', 
                           mbConfirmation, MB_YESNO);
    if DialogResult = IDYES then
    begin
      DelTree(ExpandConstant('{localappdata}\DownSort'), True, True, True);
    end;
  end;
end;

[CustomMessages]
korean.LaunchProgram=DownSort 실행
korean.CreateDesktopIcon=바탕화면 아이콘 만들기
korean.CreateQuickLaunchIcon=빠른 실행 아이콘 만들기
korean.AdditionalIcons=추가 아이콘:
korean.UninstallProgram=제거
