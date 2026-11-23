; SDI Converter 2 setup (Inno Setup 7). Built by installer\build-setup.ps1, which publishes the program
; self-contained (no .NET needed on the target PC), adds the VC++ runtime next to the engine and passes:
;   ISCC /DMyAppVersion=2.0.0.1 /DPayload=<folder> sdi_converter2.iss
; The finished installer lands in ..\publish\.
;
; Layout on the target machine:
;   C:\Program Files\SDI Converter 2          program, engine, OMT libraries, .NET runtime (read only)
;   %APPDATA%\SDI Converter 2\channels.json   the channel settings of each user (kept on uninstall)
; Not included, installed by the user: Blackmagic Desktop Video (the card driver) and, only for NDI, NDI Tools.

#ifndef MyAppVersion
  #error Build with build-setup.ps1 (it passes MyAppVersion and Payload)
#endif
#ifndef Payload
  #error Build with build-setup.ps1 (it passes MyAppVersion and Payload)
#endif

#define MyAppName "SDI Converter 2"
#define MyAppExe "SdiConverter2.exe"
#define EngineExe "sdi_converter_engine.exe"

[Setup]
AppId={{B3E61F0A-8C2D-4E57-9A41-6F2D0C7E5B19}
AppName={#MyAppName}
AppVersion={#MyAppVersion}
VersionInfoVersion={#MyAppVersion}
AppVerName={#MyAppName} {#MyAppVersion}
AppPublisher=Peter Aellig
DefaultDirName={autopf}\{#MyAppName}
DisableProgramGroupPage=yes
PrivilegesRequired=admin
ArchitecturesAllowed=x64compatible
ArchitecturesInstallIn64BitMode=x64compatible
OutputDir=..\publish
OutputBaseFilename=SDI-Converter2-Setup-{#MyAppVersion}
SetupIconFile=..\gui\Branding\sdi_converter.ico
UninstallDisplayIcon={app}\{#MyAppExe}
UninstallDisplayName={#MyAppName}
LicenseFile=..\LICENSE
Compression=lzma2/max
SolidCompression=yes
WizardStyle=modern
; A running converter is closed before its files are replaced (its engines end with it).
CloseApplications=yes
RestartApplications=no

[Languages]
Name: "en"; MessagesFile: "compiler:Default.isl"
Name: "de"; MessagesFile: "compiler:Languages\German.isl"

[CustomMessages]
en.TaskDesktop=Desktop shortcut
de.TaskDesktop=Verknüpfung auf dem Desktop
en.TaskFirewall=Windows Firewall: allow OMT and NDI for the converter engine
de.TaskFirewall=Windows-Firewall: OMT und NDI für die Converter-Engine erlauben
en.TaskLogon=Start SDI Converter 2 when a user logs in (channels marked «Start when the program opens» then start by themselves)
de.TaskLogon=SDI Converter 2 bei der Anmeldung starten (Kanäle mit «Start when the program opens» starten dann von selbst)
en.GroupOther=Other:
de.GroupOther=Weiteres:
en.StartNow=Start SDI Converter 2
de.StartNow=SDI Converter 2 starten
en.NoDriver=The Blackmagic Desktop Video driver was not found on this PC.%n%nSDI Converter 2 needs it for the DeckLink card. It can be installed afterwards (blackmagicdesign.com, Desktop Video); the converter then finds the card when the ports are refreshed.%n%nContinue anyway?
de.NoDriver=Der Treiber Blackmagic Desktop Video wurde auf diesem PC nicht gefunden.%n%nSDI Converter 2 braucht ihn für die DeckLink-Karte. Er kann auch später installiert werden (blackmagicdesign.com, Desktop Video); der Converter findet die Karte dann beim Aktualisieren der Ports.%n%nTrotzdem weiter?
en.NoNdi=NDI Tools are not installed. OMT works without them; for NDI install NDI Tools from ndi.video/tools.
de.NoNdi=NDI Tools sind nicht installiert. OMT geht auch ohne; für NDI bitte NDI Tools von ndi.video/tools installieren.

[Tasks]
Name: "desktopicon"; Description: "{cm:TaskDesktop}"; GroupDescription: "{cm:GroupOther}"
Name: "firewall"; Description: "{cm:TaskFirewall}"; GroupDescription: "{cm:GroupOther}"
Name: "logon"; Description: "{cm:TaskLogon}"; GroupDescription: "{cm:GroupOther}"; Flags: unchecked

[Files]
Source: "{#Payload}\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{autoprograms}\{#MyAppName}"; Filename: "{app}\{#MyAppExe}"; WorkingDir: "{app}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExe}"; WorkingDir: "{app}"; Tasks: desktopicon

[Registry]
Root: HKLM; Subkey: "Software\Microsoft\Windows\CurrentVersion\Run"; ValueType: string; ValueName: "{#MyAppName}"; ValueData: """{app}\{#MyAppExe}"""; Flags: uninsdeletevalue; Tasks: logon

[Run]
; The engine sends and receives OMT/NDI; one program rule covers both (and their discovery).
Filename: "{sys}\netsh.exe"; Parameters: "advfirewall firewall delete rule name=""{#MyAppName} engine"""; Flags: runhidden; Tasks: firewall
Filename: "{sys}\netsh.exe"; Parameters: "advfirewall firewall add rule name=""{#MyAppName} engine"" dir=in action=allow program=""{app}\{#EngineExe}"" profile=any"; Flags: runhidden; Tasks: firewall
Filename: "{app}\{#MyAppExe}"; Description: "{cm:StartNow}"; WorkingDir: "{app}"; Flags: nowait postinstall skipifsilent runasoriginaluser

[UninstallRun]
Filename: "{sys}\netsh.exe"; Parameters: "advfirewall firewall delete rule name=""{#MyAppName} engine"""; Flags: runhidden; RunOnceId: "RemoveFirewallRule"

[Code]
function DriverInstalled: Boolean;
begin
  Result := FileExists(ExpandConstant('{commonpf64}\Blackmagic Design\Blackmagic Desktop Video\DeckLinkAPI64.dll')) or
            FileExists(ExpandConstant('{sys}\DeckLinkAPI64.dll'));
end;

function NdiInstalled: Boolean;
begin
  Result := (GetEnv('NDI_RUNTIME_DIR_V6') <> '') or (GetEnv('NDI_RUNTIME_DIR_V5') <> '') or
            FileExists(ExpandConstant('{commonpf64}\NDI\NDI 6 Tools\Runtime\Processing.NDI.Lib.x64.dll'));
end;

function InitializeSetup: Boolean;
begin
  Result := True;
  if not DriverInstalled and not WizardSilent then
    Result := MsgBox(CustomMessage('NoDriver'), mbConfirmation, MB_YESNO) = IDYES;
end;

procedure CurStepChanged(CurStep: TSetupStep);
var
  Code: Integer;
begin
  { Engines left over from a crashed program would keep their files locked. }
  if CurStep = ssInstall then
    Exec(ExpandConstant('{sys}\taskkill.exe'), '/F /IM {#EngineExe}', '', SW_HIDE, ewWaitUntilTerminated, Code);
  if (CurStep = ssPostInstall) and not NdiInstalled and not WizardSilent then
    MsgBox(CustomMessage('NoNdi'), mbInformation, MB_OK);
end;
