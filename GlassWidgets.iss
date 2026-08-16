; GlassWidgets 安装脚本 (Inno Setup 7)
; 用法：ISCC.exe GlassWidgets.iss   （在项目根目录执行，输出到 dist\GlassWidgets-setup.exe）
#define MyAppName "GlassWidgets"
#define MyAppVersion "1.0.0"
#define MyAppPublisher "GlassWidgets"
#define MyAppURL "https://github.com/your-org/GlassWidgets"
#define MyAppExeName "GlassWidgets.exe"

[Setup]
; 注：AppId 为纯 GUID，不要用 { } 包裹，否则会被当作常量展开
AppId=8F3A1C2B-4D5E-6F7A-8B9C-0D1E2F3A4B5C
AppName={#MyAppName}
AppVersion={#MyAppVersion}
AppPublisher={#MyAppPublisher}
AppPublisherURL={#MyAppURL}
AppSupportURL={#MyAppURL}
AppUpdatesURL={#MyAppURL}
VersionInfoVersion=1.0.0.0
VersionInfoCompany={#MyAppPublisher}
VersionInfoDescription=Windows 桌面玻璃质感小组件
DefaultDirName={autopf}\{#MyAppName}
DefaultGroupName={#MyAppName}
OutputDir=dist
OutputBaseFilename={#MyAppName}-setup
SetupIconFile=Assets\LogoIcon.ico
UninstallDisplayIcon={app}\{#MyAppExeName}
Compression=lzma2
SolidCompression=yes
WizardStyle=modern
ArchitecturesInstallIn64BitMode=x64os
CloseApplications=yes
SetupLogging=yes
; 单一语言（简体中文），避免安装时弹出语言选择框
ShowLanguageDialog=no

[Languages]
Name: "chinese"; MessagesFile: "compiler:Languages\ChineseSimplified.isl"

[Tasks]
Name: "desktopicon"; Description: "创建桌面快捷方式"; GroupDescription: "附加任务:"; Flags: unchecked

[Files]
; 整个自包含发布目录（含 Avalonia 运行所需全部文件）
Source: "publish\*"; DestDir: "{app}"; Flags: ignoreversion recursesubdirs createallsubdirs

[Icons]
Name: "{group}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"
Name: "{group}\卸载 {#MyAppName}"; Filename: "{uninstallexe}"
Name: "{autodesktop}\{#MyAppName}"; Filename: "{app}\{#MyAppExeName}"; Tasks: desktopicon

[Run]
Filename: "{app}\{#MyAppExeName}"; Description: "启动 {#MyAppName}"; Flags: nowait postinstall skipifsilent
