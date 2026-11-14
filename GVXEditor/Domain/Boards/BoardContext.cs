namespace GVXEditor.Domain.Boards;

/// <summary>
/// Okolie tabul stanice pre kontrolu: co server pri starte vyhladava mimo Tables.xml a TCatalog.xml.
/// </summary>
/// <param name="Station">nazov stanice pre text zisteni</param>
/// <param name="Lines">kluce liniek z CommLines.xml; <see langword="null" />, ak sa nedaju nacitat</param>
/// <param name="StationLine">linka tabul stanice z DirList.xml (<c>tl</c>) - server ju uprednostni pred <c>lin</c></param>
/// <param name="Tracks">kluce kolaji stanice z PositionsDef.xml; <see langword="null" />, ak sa nedaju nacitat</param>
/// <param name="TabTabs">nazvy tabuliek TabTab stanice a spolocnych; <see langword="null" />, ak sa nedaju nacitat</param>
/// <param name="FontFileExists">banka pisem ELEN existuje ako subor TableFonts\&lt;nazov&gt;.bin</param>
internal sealed record BoardContext(
    string Station,
    IReadOnlySet<string>? Lines,
    string StationLine,
    IReadOnlySet<string>? Tracks,
    IReadOnlySet<string>? TabTabs,
    Func<string, bool> FontFileExists)
{
    /// <summary>Priecinok binarnych pisem tabul v datach servera.</summary>
    public const string FontDirectory = "TableFonts";

    /// <summary>
    /// Okolie stanice v aktualnom stave instalacie.
    /// </summary>
    /// <param name="workspace">otvorena instalacia</param>
    /// <param name="group">grafikon stanice</param>
    /// <param name="station">poradie stanice v grafikone</param>
    public static BoardContext Of(Iniss2Workspace workspace, GroupDocument group, int station)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        ArgumentNullException.ThrowIfNull(group);
        var document = group.Stations[station];
        var number = document.Station.Station;
        // spolocne tabulky TabTab: prvy grafikon v poradi, ktory ich ma
        var global = workspace.Groups.Select(item => item.GlobalTabTabs ?? []).FirstOrDefault(tables => tables.Count > 0) ?? [];
        var fonts = Path.Combine(workspace.DataDir, FontDirectory);
        return new BoardContext(
            workspace.StationName(number),
            workspace.Lines.Select(line => line.Key).ToHashSet(StringComparer.Ordinal),
            StationLineOf(group, station),
            document.Setup is { } setup ? setup.Tracks.Select(track => track.Key).ToHashSet(StringComparer.Ordinal) : null,
            document.Code?.TabTabs is { } tables ? tables.Concat(global).Select(table => table.Name).ToHashSet(StringComparer.Ordinal) : null,
            // server meni priponu na .bin (aj ked nazov uz priponu ma)
            name => File.Exists(Path.ChangeExtension(Path.Combine(fonts, name), "bin")));
    }

    /// <summary>
    /// Linka tabul stanice z DirList.xml: server berie prvu polozku grafikonu s cislom stanice.
    /// </summary>
    public static string StationLineOf(GroupDocument group, int station)
    {
        ArgumentNullException.ThrowIfNull(group);
        var number = group.Stations[station].Station.Station;
        return group.Group.Items.FirstOrDefault(item => item.Station == number)?.TableLine ?? "";
    }

    /// <summary>
    /// Grafikon, z ktoreho server berie tabule stanice: prvy v poradi DirList, kde stanica je; <see langword="null" />, ak nie je nikde.
    /// </summary>
    public static int? FirstGroupOf(Iniss2Workspace workspace, int station)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        for (var i = 0; i < workspace.Groups.Count; i++)
        {
            if (workspace.Groups[i].Group.Items.Any(item => item.Station == station))
                return i;
        }

        return null;
    }
}
