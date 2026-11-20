using System.ComponentModel;
using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Stations;
using GVXEditor.Domain.Stations.Texts;
using GVXEditor.Mcp.Results;
using GVXEditor.Properties;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Mcp;

/// <summary>Nastroje MCP nad textami tabul pri vlakoch (TTVlaky.xml) - nahlad a generovanie ako okno Generovat texty tabul.</summary>
[McpServerToolType]
internal static class TextTools
{
    private const string StationDescription = "Station: SR70 number, folder or name.";
    private const string SetDescription = "Text set name from preview_board_texts; empty = the first set whose board field supports generating.";
    private const string KindDescription = "destination, origin, stationsTo or stationsFrom; empty = what the field of the set shows.";
    private const string LongDescription = "Stations of the long announcement instead of the short one (stationsTo, stationsFrom).";
    private const string AllDescription = "Replace the texts of all trains of the station instead of adding missing ones.";
    private const string GroupDescription = "Timetable group from get_installation; empty = the first group.";

    private static readonly (GeneratedTextKind Kind, string Name)[] Kinds =
    [
        (GeneratedTextKind.Destination, "destination"), (GeneratedTextKind.Origin, "origin"),
        (GeneratedTextKind.StationsTo, "stationsTo"), (GeneratedTextKind.StationsFrom, "stationsFrom")
    ];

    /// <summary>Vypocet generovania: stanica, sady, vybrana sada, druh, vygenerovane texty a nahlad.</summary>
    private sealed record Generation(int GroupIndex, GroupDocument Group, int StationIndex, StationDocument Station, List<TextSetChoice> Choices,
        TextSetChoice? Set, GeneratedTextKind Kind, List<GeneratedText> Generated, List<TextPreviewLine> Lines);

    [McpServerTool(Name = "preview_board_texts", Title = "Preview generated board texts", ReadOnly = true, Idempotent = true, OpenWorld = false,
        UseStructuredContent = true)]
    [Description("Preview of generating board texts at the trains of a station into a TTVlaky.xml set, as the window Generate board texts "
                 + "does: destination or origin station, or stations to or from the direction taken from the announcement stations the "
                 + "way the server selects them. A text at the train overrides route and station texts on the server, so only texts the "
                 + "server would show differently are written. Nothing changes.")]
    public static Task<TextPreview> PreviewBoardTexts(
        EditorAccess editor,
        [Description(StationDescription)] string station,
        [Description(SetDescription)] string? set = null,
        [Description(KindDescription)] string? kind = null,
        [Description(LongDescription)] bool longReport = false,
        [Description(AllDescription)] bool all = false,
        [Description(GroupDescription)] string? group = null) =>
        editor.ReadAsync(context =>
        {
            var workspace = McpGuards.Workspace(context);
            var generation = Generate(workspace, group, station, set, kind, longReport, all);
            var sets = generation.Choices.Select(choice => new TextSetInfo(choice.Name, [.. choice.Targets.Select(target => $"{target.Catalog} / {target.Column}")],
                choice.Kind is { } setKind ? Name(setKind) : null, choice.IsNew)).ToList();
            return new TextPreview(generation.Group.Group.Name, generation.Station.Station.Station, sets, generation.Set?.Name, Name(generation.Kind),
                generation.Lines.Count(line => line.Changes),
                generation.Lines.Select(line => new TextPreviewItem(line.Text.Number, line.Text.Validity, line.Text.Text, line.Current, Change(line.Change))).ToList());
        });

    [McpServerTool(Name = "generate_board_texts", Title = "Generate board texts", ReadOnly = false, Destructive = true, Idempotent = true,
        OpenWorld = false, UseStructuredContent = true)]
    [Description("Generates board texts at the trains of a station into a TTVlaky.xml set exactly as preview_board_texts shows them "
                 + "(call it first) and as the window Generate board texts writes them: a new set for a board field is created, empty "
                 + "texts are never written, records edited for one day stay.")]
    public static async Task<GenerateTextsResult> GenerateBoardTexts(
        EditorAccess editor,
        [Description(StationDescription)] string station,
        [Description(SetDescription)] string? set = null,
        [Description(KindDescription)] string? kind = null,
        [Description(LongDescription)] bool longReport = false,
        [Description(AllDescription)] bool all = false,
        [Description(GroupDescription)] string? group = null,
        [Description(EditorAccess.DryRunDescription)] bool dryRun = false)
    {
        var (result, change) = await editor.WriteAsync(dryRun, workspace =>
        {
            var generation = Generate(workspace, group, station, set, kind, longReport, all);
            if (generation.Set is not { } chosen || generation.Station.Files is not { } files)
                throw new McpException($"texts.noSet: Station {generation.Station.Station.Directory} has no text set or board field to generate into.");

            var (sets, written) = TrainTextGenerating.Apply(files.TextSets, chosen, generation.Generated, all);
            var state = workspace.History.Current;
            if (!ReferenceEquals(sets, files.TextSets))
            {
                List<StationDocument> stations = [.. generation.Group.Stations];
                stations[generation.StationIndex] = generation.Station with { Files = files with { TextSets = sets } };
                state = state.With(generation.GroupIndex, generation.Group with { Stations = stations });
            }

            var label = string.Format(CultureInfo.CurrentCulture, Resources.History_GenerateTexts, generation.Station.Station.Directory, chosen.Name);
            return (state, label, new GenerateTextsResult(generation.Group.Group.Name, generation.Station.Station.Station, chosen.Name, Name(generation.Kind),
                written, new ChangeInfo(false, false, null)));
        }).ConfigureAwait(false);
        return result with { Change = change };
    }

    private static Generation Generate(Iniss2Workspace workspace, string? group, string station, string? set, string? kind, bool longReport, bool all)
    {
        var (groupIndex, document) = McpGuards.Group(workspace, group);
        McpGuards.Timetable(document);
        var (index, item) = McpGuards.Station(workspace, document, station);
        var choices = TrainTextGenerating.Choices(item);
        var chosen = string.IsNullOrWhiteSpace(set)
            ? choices.FirstOrDefault(choice => choice.Kind is not null) ?? choices.FirstOrDefault()
            : choices.FirstOrDefault(choice => string.Equals(choice.Name, set.Trim(), StringComparison.OrdinalIgnoreCase))
              ?? throw new McpException($"texts.setNotFound: Set '{set}' is not a set of station {item.Station.Directory}; sets: {string.Join(", ", choices.Select(c => c.Name))}.");
        var generatedKind = kind is { Length: > 0 }
            ? Kinds.FirstOrDefault(pair => string.Equals(pair.Name, kind.Trim(), StringComparison.OrdinalIgnoreCase)) is { Name: not null } match
                ? match.Kind
                : throw new McpException("texts.kind: The kind must be destination, origin, stationsTo or stationsFrom.")
            : chosen?.Kind ?? GeneratedTextKind.StationsTo;
        if (chosen is null)
            return new Generation(groupIndex, document, index, item, choices, null, generatedKind, [], []);

        // porovnanie s textom servera len pri udaji, ktoreho obsah zodpoveda generovanemu textu
        var target = chosen.Kind == generatedKind ? chosen.Target : null;
        var defaultsFile = File.Exists(Path.Combine(workspace.DataDir, document.Group.Name, DefaultStationsFile.FileName));
        var generated = TrainTextGenerating.Generate(document, index, generatedKind, target, longReport, defaultsFile, workspace.StationName);
        var lines = TrainTextGenerating.Preview(generated, item.Files?.TextSets ?? [], chosen.Name, all);
        return new Generation(groupIndex, document, index, item, choices, chosen, generatedKind, generated, lines);
    }

    private static string Name(GeneratedTextKind kind) => Kinds.First(pair => pair.Kind == kind).Name;

    private static string Change(GeneratedTextChange change) => change switch
    {
        GeneratedTextChange.Add => "add",
        GeneratedTextChange.Replace => "replace",
        GeneratedTextChange.RouteOrStationText => "routeOrStationText",
        GeneratedTextChange.SameWithout => "sameWithout",
        GeneratedTextChange.Empty => "empty",
        _ => "keep"
    };
}
