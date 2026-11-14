; Instalator GVXEditora. Spolocny obsah instalatorov je v ToolsCore\Installer\AppSetup.iss.
; Pred kompilaciou treba zostavit Release: dotnet build GVXEditor\GVXEditor.csproj -c Release

#define MyAppName "GVXEditor"
#define MyAppExeName "GVXEditor.exe"
#define MyAppDir SourcePath + "GVXEditor\bin\Release\net10.0-windows"
#define MyOutputBaseFilename "gvxeditor-setup"

#include "..\ToolsCore\Installer\AppSetup.iss"
