using System.Collections;
using System.ComponentModel;
using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Stations;
using GVXEditor.Domain.Views;
using GVXEditor.Mcp.Results;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Mcp;

/// <summary>Nastroje MCP nad vlakmi grafikonu (GV.XML) a ich zaznamami v staniciach.</summary>
[McpServerToolType]
internal static class TrainTools
{
    private const int MaxLimit = 500;

    private const string GroupDescription = "Timetable group from get_installation; empty = the first group.";

    [McpServerTool(Name = "list_trains", Title = "Trains", ReadOnly = true, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Trains of a timetable group (GV.XML with unsaved changes) in file order. Filters are optional and combine: text in the "
                 + "numbers or name, train type key, station on the route and a day the train runs on.")]
    public static Task<TrainList> ListTrains(
        EditorAccess editor,
        [Description(GroupDescription)] string? group = null,
        [Description("Text in the train numbers or name.")] string? text = null,
        [Description("Train type key, e.g. Os, R, Ex.")] string? type = null,
        [Description("Station on the route: SR70 number or text in the name.")] string? station = null,
        [Description("Only trains running on this day.")] DateOnly? day = null,
        [Description("Trains to skip.")] int offset = 0,
        [Description("Most trains to return, 1-500.")] int limit = 100) =>
        editor.ReadAsync(context =>
        {
            if (limit is < 1 or > MaxLimit)
                throw new McpException($"trains.limit: The limit must be 1 to {MaxLimit}.");
            var workspace = McpGuards.Workspace(context);
            var (_, document) = McpGuards.Group(workspace, group);
            var trains = McpGuards.Timetable(document).Trains;
            var matching = trains
                .Select((train, index) => (Train: train, Row: TrainRow.Create(train, workspace.StationName, index)))
                .Where(item => text is null || Contains(item.Row.Numbers, text) || item.Train.Variants.Any(v => v.Stops.Any(s => Contains(s.TrainName, text))))
                .Where(item => type is null || item.Train.Variants.Any(v => v.Stops.Any(s => string.Equals(s.TrainType, type, StringComparison.OrdinalIgnoreCase))))
                .Where(item => station is null || item.Train.Variants.Any(v => v.Stops.Any(s => IsStation(s.Station, station, workspace))))
                .Where(item => day is null || RunsOn(item.Train, day.Value))
                .ToList();
            var rows = matching.Skip(Math.Max(0, offset)).Take(limit)
                .Select(item => new TrainListItem(item.Row.Index, item.Row.Numbers, item.Row.Type, item.Row.Name, item.Row.From, item.Row.Departure,
                    item.Row.To, item.Row.Arrival, item.Row.Validity, item.Row.Variants, item.Row.Operator))
                .ToList();
            return new TrainList(matching.Count, rows);
        });

    [McpServerTool(Name = "get_train", Title = "Train", ReadOnly = true, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("One train with everything that belongs to it: validity period, variants with date remarks and routes, its records in the "
                 + "station files (tracks, supplements, composition, languages, stations in announcements, board texts, MOs) looked up by "
                 + "the number when leaving each station, and its lockouts and diversions.")]
    public static Task<TrainDetail> GetTrain(
        EditorAccess editor,
        [Description("Train index in GV.XML from list_trains.")] int train,
        [Description(GroupDescription)] string? group = null) =>
        editor.ReadAsync(context =>
        {
            var workspace = McpGuards.Workspace(context);
            var (_, document) = McpGuards.Group(workspace, group);
            var item = McpGuards.Train(document, train);
            var row = TrainRow.Create(item, workspace.StationName, train);
            var variants = VariantRow.Create(item).Select(variant => new VariantDetail(variant.Index, variant.Variant.Validity, variant.Days,
                variant.Operator, GvFile.FlagText(variant.Variant.Flags), variant.Variant.Note, Route(variant.Variant, workspace))).ToList();

            var keys = TrainStations.Keys(item, document.Group);
            var stations = keys.Select(key => Records(document.Stations[key.StationIndex], key)).ToList();
            var lockoutNumber = TrainStations.LockoutNumber(item, document.InterestStations);
            var lockouts = (document.Lockouts ?? []).Where(lockout => lockout.Number == lockoutNumber).ToList();
            var diversions = (document.Diversions ?? [])
                .Where(diversion => keys.Any(key => key.Station.Station == diversion.Station && key.Number == diversion.Number))
                .ToList();
            return new TrainDetail(document.Group.Name, train, row.Numbers, item.From, item.To == DateOnly.MaxValue ? null : item.To, variants,
                stations, lockoutNumber, lockouts, diversions);
        });

    [McpServerTool(Name = "explain_validity", Title = "Explain validity text", ReadOnly = true, Idempotent = true, OpenWorld = false,
        UseStructuredContent = true)]
    [Description("Expands a date remark of INISS2 (Czech, e.g. 'jede v 6,7', 'nejede 24.XII.'; Slovak words are read too) to the days "
                 + "it means in a period, the way the server does. Holidays are always Czech. Without a period the period of the train "
                 + "is used, without a train the period of all trains of the group.")]
    public static Task<ValidityDays> ExplainValidity(
        EditorAccess editor,
        [Description("Date remark; empty = every day.")] string text,
        [Description("First day of the period.")] DateOnly? from = null,
        [Description("Last day of the period.")] DateOnly? to = null,
        [Description("Train index in GV.XML whose period is used.")] int? train = null,
        [Description(GroupDescription)] string? group = null) =>
        editor.ReadAsync(context =>
        {
            var (start, end) = (from, to);
            if (start is null || end is null)
            {
                var workspace = McpGuards.Workspace(context);
                var (_, document) = McpGuards.Group(workspace, group);
                var trains = train is { } index ? [McpGuards.Train(document, index)] : McpGuards.Timetable(document).Trains;
                if (trains.Count == 0)
                    throw new McpException("validity.period: The group has no train; give from and to.");
                start ??= trains.Min(item => item.From);
                end ??= trains.Max(item => item.To == DateOnly.MaxValue ? item.From.AddDays(364) : item.To);
            }

            if (end < start)
                throw new McpException("validity.period: The period ends before it starts.");
            var days = GvValidity.Days(start.Value, end.Value, text);
            var problem = GvValidity.Check(start.Value, end.Value, text) switch
            {
                GvValidityProblem.PeriodTooLong => "The server expands a text remark only in periods of at most 400 days; in this period the variant runs on no day.",
                GvValidityProblem.Invalid => "The server cannot expand this remark; a variant with it runs on no day.",
                _ => null
            };
            return new ValidityDays(text, start.Value, end.Value, Count(days), Ranges(start.Value, days), problem);
        });

    [McpServerTool(Name = "station_day", Title = "Station on a day", ReadOnly = true, Idempotent = true, OpenWorld = false, UseStructuredContent = true)]
    [Description("Trains a station has on a day as the server sees them: the variant that applies that day, train numbers on arrival and "
                 + "departure and the track from Positions.xml. A night train appears at a station after midnight on the next day.")]
    public static Task<StationDay> StationDayTool(
        EditorAccess editor,
        [Description("Station: SR70 number, folder or name.")] string station,
        [Description("Day.")] DateOnly day,
        [Description(GroupDescription)] string? group = null) =>
        editor.ReadAsync(context =>
        {
            var workspace = McpGuards.Workspace(context);
            var (_, document) = McpGuards.Group(workspace, group);
            McpGuards.Timetable(document);
            var (_, item) = McpGuards.Station(workspace, document, station);
            var rows = StationDayRow.Create(workspace.StationTimetable(document, item.Station).On(day), workspace.StationName);
            return new StationDay(document.Group.Name, item.Station.Station, workspace.StationName(item.Station.Station), day,
                rows.Select(row => new StationDayItem(row.Arrival, row.Departure, row.ArrivalNumber, row.DepartureNumber, row.Type, row.Name,
                    row.From, row.To, row.Track, row.Variant)).ToList());
        });

    private static List<StopDetail> Route(GvVariant variant, Iniss2Workspace workspace) =>
        StopRow.Create(variant, workspace.StationName)
            .Select((row, index) => new StopDetail(variant.Stops[index].Station, row.Station, row.Arrival, row.Departure, GvFile.FlagText(variant.Stops[index].Flags), row.Number,
                row.DisplayNumber, row.Type, row.Name, row.Line, row.ReplacementLine))
            .ToList();

    private static TrainStationRecords Records(StationDocument station, TrainStationKey key)
    {
        var files = station.Files;
        var number = key.Number;
        return new TrainStationRecords(key.Station.Station, key.Station.Directory, number,
            [.. files?.Positions.Where(record => record.Train == number) ?? []],
            [.. files?.Additions.Where(record => record.Number == number) ?? []],
            [.. files?.Compositions.Where(record => record.Number == number) ?? []],
            [.. files?.Languages.Where(record => record.Number == number) ?? []],
            [.. files?.ReportStations.Where(record => record.Number == number) ?? []],
            [
                .. files?.TextSets.SelectMany(set => set.Trains.Where(record => record.Number == number)
                    .Select(record => new TrainTextInfo(set.Name, record.Validity, record.Text, record.Font))) ?? []
            ],
            [.. files?.Mos.Where(record => record.Number == number) ?? []]);
    }

    private static bool Contains(string value, string text) => value.Contains(text.Trim(), StringComparison.CurrentCultureIgnoreCase);

    private static bool IsStation(int number, string station, Iniss2Workspace workspace) =>
        number.ToString(CultureInfo.InvariantCulture) == station.Trim() || Contains(workspace.StationName(number), station);

    // vlak ide v den, ked v nom ide niektora varianta
    private static bool RunsOn(GvTrain train, DateOnly day)
    {
        var index = day.DayNumber - train.From.DayNumber;
        if (index < 0 || day > train.To)
            return false;
        return train.Variants.Any(variant => train.ActiveDaysOf(variant) is var days && index < days.Length && days[index]);
    }

    private static int Count(BitArray days)
    {
        var count = 0;
        for (var i = 0; i < days.Length; i++)
        {
            if (days[i])
                count++;
        }

        return count;
    }

    private static List<DayRange> Ranges(DateOnly from, BitArray days)
    {
        var ranges = new List<DayRange>();
        for (var i = 0; i < days.Length; i++)
        {
            if (!days[i])
                continue;
            var start = i;
            while (i + 1 < days.Length && days[i + 1])
                i++;
            ranges.Add(new DayRange(from.AddDays(start), from.AddDays(i)));
        }

        return ranges;
    }
}
