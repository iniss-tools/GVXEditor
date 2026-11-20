using System.ComponentModel;
using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Editing;
using GVXEditor.Domain.Rules;
using GVXEditor.Mcp.Inputs;
using GVXEditor.Mcp.Results;
using GVXEditor.Properties;
using ModelContextProtocol;
using ModelContextProtocol.Server;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Mcp;

/// <summary>
/// Nastroje MCP so zmenou vlakov grafikonu: ako prikazy ponuky Upravy (okno Upravit vlak, Kopirovat, Zmazat, Premenovat
/// cislo, Posunut platnost) s rovnakymi pravidlami. Kazda zmena je jeden krok spat, nic sa neuklada.
/// </summary>
[McpServerToolType]
internal static class TrainEditTools
{
    private const string GroupDescription = "Timetable group from get_installation; empty = the first group.";

    // kluce priznakov zo suboru -> priznak (ako GvFile)
    private static readonly Dictionary<string, GvVariantFlags> VariantFlags = Enum.GetValues<GvVariantFlags>()
        .Where(flag => flag != 0 && GvFile.FlagText(flag).Length > 0).ToDictionary(flag => GvFile.FlagText(flag), StringComparer.OrdinalIgnoreCase);

    [McpServerTool(Name = "set_train", Title = "Set train", ReadOnly = false, Destructive = true, Idempotent = true, OpenWorld = false,
        UseStructuredContent = true)]
    [Description("Changes a train of GV.XML or adds a new one at the end, with the checks of the Edit train window: errors refuse the change, "
                 + "warnings are returned. Only the given values change; variants replace all variants of the train (get_train gives the "
                 + "current ones in the same shape). Key Z and days after midnight are computed like the INISS2 client does. Station "
                 + "records, lockouts and diversions of the train stay - records of a changed train number stay at the old number.")]
    public static async Task<TrainChangeResult> SetTrain(
        EditorAccess editor,
        [Description("Train index in GV.XML from list_trains; null = a new train.")] int? train = null,
        [Description("First day of the validity period; a new train without it takes the period of the first train of the group.")] DateOnly? from = null,
        [Description("Last day of the validity period.")] DateOnly? to = null,
        [Description("The train has no last day (the server uses 31.12.9999); a date remark then never applies.")] bool? noEnd = null,
        [Description("All variants in order - on a day the first variant running that day applies. Required for a new train.")]
        IReadOnlyList<VariantInput>? variants = null,
        [Description(GroupDescription)] string? group = null,
        [Description(EditorAccess.DryRunDescription)] bool dryRun = false)
    {
        var (result, change) = await editor.WriteAsync(dryRun, workspace =>
        {
            var (index, document) = McpGuards.Group(workspace, group);
            var timetable = McpGuards.Timetable(document);
            var original = train is { } existing ? McpGuards.Train(document, existing) : null;
            if (original is null && variants is null)
                throw new McpException("train.variants: A new train needs its variants with routes.");

            var draft = original is not null ? TrainDraft.Of(original) : NewDraft(timetable);
            if (from is { } start)
                draft.From = start;
            if (to is { } end)
                draft.To = end;
            if (noEnd is { } open)
                draft.NoEnd = open;
            if (variants is not null)
            {
                draft.Variants.Clear();
                draft.Variants.AddRange(variants.Select((variant, position) => Variant(variant, position + 1)));
            }

            var problems = TrainRules.Check(draft, document.InterestStations, workspace.StationName);
            if (problems.Any(problem => problem.IsError))
                throw new McpException("train.invalid: " + string.Join(" ", problems.Where(problem => problem.IsError).Select(problem => problem.Message)));

            var changed = draft.ToTrain(document.InterestStations);
            GvFile updated;
            int position;
            string label;
            if (original is null)
            {
                updated = TimetableEditing.Add(timetable, changed);
                position = timetable.Trains.Count;
                label = Label(Resources.History_New, changed);
            }
            else
            {
                updated = TimetableEditing.SameTrain(original, changed) ? timetable : TimetableEditing.Replace(timetable, train!.Value, changed);
                position = train!.Value;
                label = Label(Resources.History_Edit, changed);
            }

            var state = ReferenceEquals(updated, timetable) ? workspace.History.Current : workspace.History.Current.With(index, document with { Timetable = updated });
            var warnings = problems.Select(problem => new ProblemInfo("warning", problem.Message, problem.Variant >= 0 ? problem.Variant + 1 : null,
                problem.Stop >= 0 ? problem.Stop + 1 : null)).ToList();
            return (state, label, (document.Group.Name, position, string.Join(" / ", TrainNumbers.Of(changed)), warnings));
        }).ConfigureAwait(false);
        return new TrainChangeResult(result.Name, result.position, result.Item3, result.warnings, change);
    }

    [McpServerTool(Name = "copy_train", Title = "Copy trains", ReadOnly = false, Destructive = false, Idempotent = false, OpenWorld = false,
        UseStructuredContent = true)]
    [Description("Copies trains of GV.XML; each copy goes right after its original, like Copy train in GVXEditor. Station records stay "
                 + "shared through the train number, so change the number of the copy with set_train or rename_train_number.")]
    public static async Task<TrainsChangeResult> CopyTrain(
        EditorAccess editor,
        [Description("Train indexes in GV.XML from list_trains.")] IReadOnlyList<int> trains,
        [Description(GroupDescription)] string? group = null,
        [Description(EditorAccess.DryRunDescription)] bool dryRun = false)
    {
        var (result, change) = await editor.WriteAsync(dryRun, workspace =>
        {
            var (index, document) = McpGuards.Group(workspace, group);
            var indexes = Indexes(document, trains);
            var timetable = McpGuards.Timetable(document);
            var copies = indexes.Select((train, order) => train + order + 1).ToList();
            var label = indexes.Count == 1
                ? Label(Resources.History_Copy, timetable.Trains[indexes[0]])
                : string.Format(CultureInfo.CurrentCulture, Resources.History_CopyMany, indexes.Count);
            var state = workspace.History.Current.With(index, document with { Timetable = TimetableEditing.Copy(timetable, indexes) });
            return (state, label, (document.Group.Name, copies));
        }).ConfigureAwait(false);
        return new TrainsChangeResult(result.Name, result.copies, 0, change);
    }

    [McpServerTool(Name = "delete_train", Title = "Delete trains", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false,
        UseStructuredContent = true)]
    [Description("Deletes trains from GV.XML like Delete train in GVXEditor. With removeUnusedRecords it also removes their station records, "
                 + "lockouts and diversions that no other train of the group uses any more. Indexes of later trains shift.")]
    public static async Task<TrainsChangeResult> DeleteTrain(
        EditorAccess editor,
        [Description("Train indexes in GV.XML from list_trains.")] IReadOnlyList<int> trains,
        [Description("Also remove records of the deleted trains that no other train uses.")] bool removeUnusedRecords = false,
        [Description(GroupDescription)] string? group = null,
        [Description(EditorAccess.DryRunDescription)] bool dryRun = false)
    {
        var (result, change) = await editor.WriteAsync(dryRun, workspace =>
        {
            var (index, document) = McpGuards.Group(workspace, group);
            var indexes = Indexes(document, trains);
            var timetable = McpGuards.Timetable(document);
            var deleted = indexes.Select(train => timetable.Trains[train]).ToList();
            var updated = document with { Timetable = TimetableEditing.Remove(timetable, indexes) };
            var removed = 0;
            if (removeUnusedRecords)
            {
                foreach (var train in deleted)
                {
                    (updated, var count) = TrainNumbers.RemoveUnused(updated, train);
                    removed += count;
                }
            }

            var label = deleted.Count == 1
                ? Label(Resources.History_Delete, deleted[0])
                : string.Format(CultureInfo.CurrentCulture, Resources.History_DeleteMany, deleted.Count);
            return (workspace.History.Current.With(index, updated), label, (document.Group.Name, removed));
        }).ConfigureAwait(false);
        return new TrainsChangeResult(result.Name, [], result.removed, change);
    }

    [McpServerTool(Name = "rename_train_number", Title = "Rename train number", ReadOnly = false, Destructive = true, Idempotent = false,
        OpenWorld = false, UseStructuredContent = true)]
    [Description("Renames a number of a train in GV.XML (number and announced number) and in all records of the group - station files, "
                 + "lockouts and diversions - like Rename train number in GVXEditor. Other trains with the old number keep their records "
                 + "and the renamed train gets a copy, unless allTrains renames them too.")]
    public static async Task<RenameResult> RenameTrainNumber(
        EditorAccess editor,
        [Description("Train index in GV.XML from list_trains.")] int train,
        [Description("Old number - one of the numbers of the train.")] string from,
        [Description("New number.")] string to,
        [Description("Also rename the other trains of the group that use the old number.")] bool allTrains = false,
        [Description("Allow a new number another train already has - station records of both trains then merge.")] bool mergeExisting = false,
        [Description(GroupDescription)] string? group = null,
        [Description(EditorAccess.DryRunDescription)] bool dryRun = false)
    {
        var (result, change) = await editor.WriteAsync(dryRun, workspace =>
        {
            var (index, document) = McpGuards.Group(workspace, group);
            var original = McpGuards.Train(document, train);
            var timetable = McpGuards.Timetable(document);
            var (old, number) = (from.Trim(), to.Trim());
            if (!TrainNumbers.Of(original).Contains(old))
                throw new McpException($"rename.number: Train {train} has no number '{old}'; its numbers: {string.Join(", ", TrainNumbers.Of(original))}.");
            if (number.Length == 0 || number == old)
                throw new McpException("rename.to: The new number must be different and not empty.");
            if (!mergeExisting && TrainNumbers.OtherTrains(timetable, train, number).Count > 0)
                throw new McpException($"rename.exists: Another train already has number {number}; station records of both trains would merge. Set mergeExisting to rename anyway.");

            var others = TrainNumbers.OtherTrains(timetable, train, old);
            var records = TrainNumbers.RecordCount(document, old);
            var label = string.Format(CultureInfo.CurrentCulture, Resources.History_Rename, old, number);
            var state = workspace.History.Current.With(index, TrainNumbers.Rename(document, train, old, number, allTrains));
            List<int> renamed = allTrains ? [train, .. others] : [train];
            return (state, label, (document.Group.Name, old, number, renamed.Order().ToList(), records));
        }).ConfigureAwait(false);
        return new RenameResult(result.Name, result.old, result.number, result.Item4, result.records, change);
    }

    [McpServerTool(Name = "shift_validity", Title = "Shift validity", ReadOnly = false, Destructive = true, Idempotent = false, OpenWorld = false,
        UseStructuredContent = true)]
    [Description("Shifts the validity periods of all trains of the group (or the given trains) by days, like Shift timetable validity in "
                 + "GVXEditor; 364 days = 52 weeks keeps weekdays. Date remarks stay - weekdays, holidays and dates without a year move "
                 + "with the period, a date with a year must be changed by hand. A train without end keeps no end.")]
    public static async Task<TrainsChangeResult> ShiftValidity(
        EditorAccess editor,
        [Description("Days to shift, negative = back.")] int days,
        [Description("Train indexes in GV.XML; empty = all trains of the group.")] IReadOnlyList<int>? trains = null,
        [Description(GroupDescription)] string? group = null,
        [Description(EditorAccess.DryRunDescription)] bool dryRun = false)
    {
        var (result, change) = await editor.WriteAsync(dryRun, workspace =>
        {
            var (index, document) = McpGuards.Group(workspace, group);
            var timetable = McpGuards.Timetable(document);
            var only = trains is { Count: > 0 } ? Indexes(document, trains).ToHashSet() : null;
            if (days == 0)
                return (workspace.History.Current, "", (document.Group.Name, new List<int>()));

            var label = only is null
                ? string.Format(CultureInfo.CurrentCulture, Resources.History_Shift, days)
                : string.Format(CultureInfo.CurrentCulture, Resources.History_ShiftSelected, only.Count, days);
            var state = workspace.History.Current.With(index, document with { Timetable = TimetableEditing.ShiftValidity(timetable, days, only) });
            return (state, label, (document.Group.Name, only?.Order().ToList() ?? [.. Enumerable.Range(0, timetable.Trains.Count)]));
        }).ConfigureAwait(false);
        return new TrainsChangeResult(result.Name, result.Item2, 0, change);
    }

    // novy vlak ma obdobie prveho vlaku grafikonu (vlaky grafikonu ho maju spravidla rovnake), inak rok od dneska
    private static TrainDraft NewDraft(GvFile timetable)
    {
        var source = timetable.Trains.Count > 0 ? timetable.Trains[0] : null;
        var from = source is null || source.From == DateOnly.MinValue ? DateOnly.FromDateTime(DateTime.Today) : source.From;
        var noEnd = source?.To == DateOnly.MaxValue;
        var draft = TrainDraft.New(from, source is null || noEnd ? from.AddDays(364) : source.To);
        draft.NoEnd = noEnd;
        return draft;
    }

    private static VariantDraft Variant(VariantInput input, int position)
    {
        var flags = default(GvVariantFlags);
        foreach (var key in Keys(input.Flags))
            flags |= VariantFlags.TryGetValue(key, out var flag)
                ? flag
                : throw new McpException($"train.flag: Variant {position} has unknown flag '{key}'; flags: {string.Join(", ", VariantFlags.Keys)}.");
        var variant = new VariantDraft { Validity = input.Validity.Trim(), Operator = input.Operator.Trim(), Flags = flags, Note = input.Note };
        foreach (var stop in input.Route)
            variant.Stops.Add(Stop(stop, position));
        return variant;
    }

    private static StopDraft Stop(StopInput input, int position)
    {
        var keys = Keys(input.Flags).Select(key => key.ToUpperInvariant()).ToList();
        foreach (var key in keys.Where(key => key is not ("P" or "PR" or "DB" or "X" or "Z")))
            throw new McpException($"train.flag: Variant {position} has unknown point flag '{key}'; flags: P, PR, DB, X.");
        return new StopDraft
        {
            Station = input.Station.ToString(CultureInfo.InvariantCulture),
            Arrival = Time(input.Arrival),
            Departure = Time(input.Departure),
            Passing = keys.Contains("P"),
            Transfer = keys.Contains("PR"),
            OperatingPoint = keys.Contains("DB"),
            OnRequest = keys.Contains("X"),
            Number = input.Number.Trim(),
            DisplayNumber = input.ReportedNumber.Trim(),
            TrainType = input.Type.Trim(),
            TrainName = input.TrainName.Trim(),
            Line = input.IdsLine.Trim(),
            ReplacementLine = input.NadLine.Trim()
        };
    }

    // cas z get_train moze mat pocet dni (08:15 +1) - dni sa pri zapise dopocitaju z casov
    private static string Time(string text)
    {
        var value = text.Trim();
        var plus = value.IndexOf(" +", StringComparison.Ordinal);
        return plus >= 0 ? value[..plus].Trim() : value;
    }

    private static string[] Keys(string text) => text.Split([' ', ','], StringSplitOptions.RemoveEmptyEntries);

    private static List<int> Indexes(GroupDocument document, IReadOnlyList<int> trains)
    {
        if (trains.Count == 0)
            throw new McpException("train.none: Give at least one train index.");
        foreach (var train in trains)
            McpGuards.Train(document, train);
        return [.. trains.Distinct().Order()];
    }

    private static string Label(string format, GvTrain train) =>
        string.Format(CultureInfo.CurrentCulture, format, TrainNumbers.Of(train).FirstOrDefault() ?? "?");
}
