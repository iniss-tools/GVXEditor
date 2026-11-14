namespace GVXEditor.Domain;

/// <summary>
/// Nemenny stav dat instalacie - jeden krok historie spat a znova.
/// </summary>
/// <param name="Groups">grafikony v poradi DirList.xml</param>
internal sealed record WorkspaceState(IReadOnlyList<GroupDocument> Groups)
{
    /// <summary>Hlasenia obsluhy z ConstReports.xml (spolocne pre instalaciu); <see langword="null" />, ak sa nedaju nacitat.</summary>
    public IReadOnlyList<ToolsCore.Iniss2.Grafikon.ConstReport>? ConstReports { get; init; } = [];

    /// <summary>Text LogText.vb (skript hlaseni); <see langword="null" />, ak subor chyba alebo sa neda nacitat.</summary>
    public string? LogText { get; init; }

    /// <summary>
    /// Stav s nahradenym grafikonom.
    /// </summary>
    /// <param name="index">poradie grafikonu</param>
    /// <param name="group">novy grafikon</param>
    public WorkspaceState With(int index, GroupDocument group)
    {
        var groups = Groups.ToArray();
        groups[index] = group;
        return this with { Groups = groups };
    }
}
