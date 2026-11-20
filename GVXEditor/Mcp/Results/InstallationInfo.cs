using System.ComponentModel;

namespace GVXEditor.Mcp.Results;

/// <summary>Vysledok get_installation.</summary>
internal sealed record InstallationInfo(
    [property: Description("Data folder of the INISS2 server installation.")] string DataDirectory,
    [property: Description("Server of this installation: running, stopped or stoppedOtherRunning (a server of another installation runs). GVXEditor saves only while it is stopped.")] string Server,
    [property: Description("The editor has changes that are not saved yet.")] bool UnsavedChanges,
    [property: Description("Data files (relative to the data folder) that saving would write.")] IReadOnlyList<string> UnsavedFiles,
    [property: Description("Last step the user can undo; null = none.")] string? LastStep,
    [property: Description("verify_with_server is available (the installation has IniSrv.exe).")] bool ServerVerification,
    [property: Description("Timetable groups in DirList.xml order.")] IReadOnlyList<GroupInfo> Groups);
