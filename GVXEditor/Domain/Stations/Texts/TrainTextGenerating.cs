using GVXEditor.Domain.Setup;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Stations.Texts;

/// <summary>
/// Generovanie textov vlakov do sady TTVlaky.xml stanice ako texty na tabuliach v GVDEditore: cielova alebo vychodzia
/// stanica, stanice do smeru alebo zo smeru zo stanic hlasenia (<see cref="ReportStationSelection" />) bez cielovej
/// (vychodzej) stanice. Casti textu oddeluje <c>#</c>. Text pri vlaku ma u servera prednost pred textami podla trasy
/// a stanice, zaznam sa preto pise len vtedy, ked by server bez neho ukazal nieco ine.
/// </summary>
internal static class TrainTextGenerating
{
    // obsah udaja (inf v TCatalog.xml) -> co sa do sady generuje
    private static readonly Dictionary<string, GeneratedTextKind> Kinds = new(StringComparer.Ordinal)
    {
        ["CilNazev"] = GeneratedTextKind.Destination,
        ["CilNazevNastupiste"] = GeneratedTextKind.Destination,
        ["CilNazevPodchod"] = GeneratedTextKind.Destination,
        ["VychoziNazev"] = GeneratedTextKind.Origin,
        ["PoNazev"] = GeneratedTextKind.StationsTo,
        ["PoNazevNastupiste"] = GeneratedTextKind.StationsTo,
        ["PredNazev"] = GeneratedTextKind.StationsFrom
    };

    // zaklad nazvu novej sady ako v datach servera (server k nazvu pripaja cislo)
    private static readonly Dictionary<GeneratedTextKind, string> NewSetNames = new()
    {
        [GeneratedTextKind.Destination] = "CiloveStanice",
        [GeneratedTextKind.Origin] = "VychoziStanice",
        [GeneratedTextKind.StationsTo] = "TrasaDoSmeru",
        [GeneratedTextKind.StationsFrom] = "TrasaZeSmeru"
    };

    /// <summary>
    /// Prvy udaj sady, ktoreho obsah generovanie podporuje, a co sa pren generuje.
    /// </summary>
    /// <param name="targets">udaje sady</param>
    /// <param name="catalog">katalogove predlohy stanice (TCatalog.xml)</param>
    public static (TextTarget Target, GeneratedTextKind Kind)? KindOf(IEnumerable<TextTarget> targets, IReadOnlyList<CatalogTable> catalog)
    {
        ArgumentNullException.ThrowIfNull(targets);
        ArgumentNullException.ThrowIfNull(catalog);
        foreach (var target in targets)
        {
            var content = catalog.FirstOrDefault(table => table.Key == target.Catalog)?.Columns.FirstOrDefault(column => column.Key == target.Column)?.Content;
            if (content is not null && Kinds.TryGetValue(content, out var kind))
                return (target, kind);
        }

        return null;
    }

    /// <summary>
    /// Sady, do ktorych sa da generovat: sady z TTVlaky.xml, TTTrasy.xml a TTStanice.xml stanice (kazdy nazov raz) a nove sady
    /// pre udaje katalogu s podporovanym obsahom, pre ktore ziadna sada nie je.
    /// </summary>
    public static List<TextSetChoice> Choices(StationDocument station)
    {
        ArgumentNullException.ThrowIfNull(station);
        var catalog = station.Boards?.Catalog ?? [];
        var choices = new List<TextSetChoice>();

        void Add(string name, IReadOnlyList<TextTarget> targets, bool fonet)
        {
            if (name.Length == 0 || choices.Any(choice => choice.Name == name))
                return;
            var kind = KindOf(targets, catalog);
            choices.Add(new TextSetChoice(name, targets, kind?.Kind, false) { Fonet = fonet, Target = kind?.Target });
        }

        foreach (var set in station.Files?.TextSets ?? [])
            Add(set.Name, set.Targets, set.Fonet);
        foreach (var set in station.Setup?.RouteTexts ?? [])
            Add(set.Name, set.Targets, set.Fonet);
        foreach (var set in station.Setup?.StationTexts ?? [])
            Add(set.Name, set.Targets, set.Fonet);

        var used = choices.SelectMany(choice => choice.Targets).ToHashSet();
        foreach (var table in catalog)
        {
            foreach (var column in table.Columns)
            {
                var target = new TextTarget(table.Key, column.Key);
                if (!Kinds.TryGetValue(column.Content, out var kind) || !used.Add(target))
                    continue;
                choices.Add(new TextSetChoice(UniqueName(NewSetNames[kind], choices), [target], kind, true) { Target = target });
            }
        }

        return choices;
    }

    // nazov ako server: zaklad a cislo, ktore v stanici este nie je
    private static string UniqueName(string name, List<TextSetChoice> choices)
    {
        for (var i = 1; ; i++)
        {
            var candidate = name + i.ToString(System.Globalization.CultureInfo.InvariantCulture);
            if (!choices.Any(choice => choice.Name == candidate))
                return candidate;
        }
    }

    /// <summary>
    /// Texty vlakov grafikonu, ktore v stanici zastavuju (kluc <c>Z</c> bez <c>P</c> a cas), podla cisla pri odchode. Ak sa
    /// texty variant s rovnakym cislom lisia, kazda varianta dostane text so svojou datumovou poznamkou (poznamky pred
    /// textom bez poznamky, ako ich server hlada).
    /// </summary>
    /// <param name="group">grafikon</param>
    /// <param name="stationIndex">poradie stanice v grafikone</param>
    /// <param name="kind">co sa generuje</param>
    /// <param name="target">udaj tabule (porovnanie s textom servera bez zaznamu); <see langword="null" /> = bez porovnania</param>
    /// <param name="longReport">stanice z dlheho hlasenia, inak z kratkeho</param>
    /// <param name="defaultsFile">grafikon ma Default.Stations.xml</param>
    /// <param name="stationName">nazov stanice podla cisla</param>
    public static List<GeneratedText> Generate(GroupDocument group, int stationIndex, GeneratedTextKind kind, TextTarget? target, bool longReport,
        bool defaultsFile, Func<int, string> stationName)
    {
        ArgumentNullException.ThrowIfNull(group);
        ArgumentNullException.ThrowIfNull(stationName);
        var station = group.Stations[stationIndex];
        var source = new ReportStationSource(station.Files?.ReportStations ?? [], station.Setup?.ShortPatterns ?? [], station.Setup?.LongPatterns ?? [],
            group.Setup?.DefaultStations ?? [], defaultsFile);
        var interest = group.InterestStations;

        var texts = new List<(string Number, string Validity, string Text, string? Server, bool FromFiles)>();
        foreach (var train in group.Timetable?.Trains ?? [])
        {
            foreach (var variant in TrainStations.ServerView(train, interest).Variants)
            {
                var index = StopIndex(variant, station.Station.Station);
                if (index < 0 || variant.NumberSource(index, false)?.Number is not { Length: > 0 } number)
                    continue;
                var (server, fromFiles) = target is null ? (null, false) : ServerText(variant, index, kind, target, station.Setup, stationName);
                texts.Add((number, variant.Validity, Text(variant, index, number, kind, source, longReport, stationName), server, fromFiles));
            }
        }

        var result = new List<GeneratedText>();
        foreach (var byNumber in texts.GroupBy(text => text.Number, StringComparer.Ordinal))
        {
            var distinct = byNumber.Select(text => text.Text).Distinct(StringComparer.Ordinal).ToList();
            if (distinct.Count == 1)
            {
                result.Add(new GeneratedText(byNumber.Key, "", distinct[0])
                {
                    ServerText = byNumber.First().Server,
                    IsRedundant = target is not null && byNumber.All(text => text.Server == text.Text),
                    HasFileText = byNumber.Any(text => text.FromFiles)
                });
                continue;
            }

            result.AddRange(byNumber.Select(text => new GeneratedText(text.Number, text.Validity, text.Text)
                {
                    ServerText = text.Server,
                    IsRedundant = target is not null && text.Server == text.Text,
                    HasFileText = text.FromFiles
                })
                .Distinct().OrderBy(text => text.Validity.Length == 0));
        }

        return result;
    }

    // zastavenie v stanici tak, ako stanici patri u servera
    private static int StopIndex(GvVariant variant, int station)
    {
        for (var i = 0; i < variant.Stops.Count; i++)
        {
            if (variant.Stops[i].Station == station && variant.Stops[i].IsInterest && variant.Stops[i].HasTime)
                return i;
        }

        return -1;
    }

    private static string Text(GvVariant variant, int index, string number, GeneratedTextKind kind, ReportStationSource source, bool longReport,
        Func<int, string> stationName)
    {
        var stops = variant.Stops;
        switch (kind)
        {
            case GeneratedTextKind.Destination:
                return stationName(stops[^1].Station);
            case GeneratedTextKind.Origin:
                return stationName(stops[0].Station);
        }

        var (from, to) = ReportStationSelection.Of(variant, index, number, source, longReport);
        // cielova (vychodzia) stanica je na tabuli v inom udaji
        IEnumerable<int> stations = kind == GeneratedTextKind.StationsTo
            ? to.Count > 0 && to[^1] == stops[^1].Station ? to.Take(to.Count - 1) : to
            : from.Count > 0 && from[0] == stops[0].Station ? from.Skip(1) : from;
        return string.Join("#", stations.Select(stationName));
    }

    /// <summary>
    /// Text udaja bez zaznamu v TTVlaky.xml: prva sada TTTrasy.xml s udajom (trasa bez bodov <c>P</c> od tejto stanice po
    /// cielovu, pri prichodovom udaji od vychodzej po tuto; bez zhody prazdny), inak prva sada TTStanice.xml s udajom
    /// (cielova, pri prichodovom vychodzia stanica), inak hodnota, ktoru server vypocita - nazov stanice, pri staniciach
    /// smeru nic.
    /// </summary>
    /// <returns>text (<see langword="null" />, ak ho server nevyplni) a ci je to neprazdny text zo suboru textov</returns>
    private static (string? Text, bool FromFiles) ServerText(GvVariant variant, int index, GeneratedTextKind kind, TextTarget target,
        StationSetup? setup, Func<int, string> stationName)
    {
        var stops = variant.Stops;
        var arrival = kind is GeneratedTextKind.Origin or GeneratedTextKind.StationsFrom;
        if (setup?.RouteTexts.FirstOrDefault(set => set.Targets.Contains(target)) is { } routes)
        {
            var route = arrival ? ReportStationSelection.Route(stops, 0, index) : ReportStationSelection.Route(stops, index, stops.Count - 1);
            var text = routes.Routes.FirstOrDefault(item => item.Stations.Select(stop => stop.Station).SequenceEqual(route))?.Text ?? "";
            return (text, text.Length > 0);
        }

        var end = arrival ? stops[0].Station : stops[^1].Station;
        if (setup?.StationTexts.FirstOrDefault(set => set.Targets.Contains(target)) is { } stations &&
            stations.Stations.FirstOrDefault(item => item.Station == end) is { } station)
            return (station.Text, station.Text.Length > 0);

        return (kind is GeneratedTextKind.Destination or GeneratedTextKind.Origin ? stationName(end) : null, false);
    }

    /// <summary>
    /// Text sa zapise: nie je prazdny (zaznam s prazdnym textom by prekryl texty podla trasy a stanice) a server by bez neho
    /// ukazal nieco ine; pri doplneni len vlaku, ktory v sade text nema a nema ani text podla trasy alebo stanice.
    /// </summary>
    /// <param name="text">vygenerovany text</param>
    /// <param name="hasRecord">vlak (cislo) uz ma v sade zaznam</param>
    /// <param name="all">nahradit aj existujuce texty</param>
    public static bool WillWrite(GeneratedText text, bool hasRecord, bool all)
    {
        ArgumentNullException.ThrowIfNull(text);
        return text.Text.Length > 0 && !text.IsRedundant && (all || !hasRecord && !text.HasFileText);
    }

    /// <summary>
    /// Nahlad generovania do sady: vygenerovane texty vlakov stanice so sucasnym textom a zmenou, ako ju urobi
    /// <see cref="Apply" /> (okno Generovat texty tabul aj agent).
    /// </summary>
    /// <param name="generated">texty z <see cref="Generate" /></param>
    /// <param name="sets">sady TTVlaky.xml stanice</param>
    /// <param name="set">nazov sady, do ktorej sa generuje</param>
    /// <param name="all">nahradit aj existujuce texty</param>
    public static List<TextPreviewLine> Preview(IReadOnlyList<GeneratedText> generated, IReadOnlyList<TrainTextSet> sets, string set, bool all)
    {
        ArgumentNullException.ThrowIfNull(generated);
        ArgumentNullException.ThrowIfNull(sets);
        var existing = sets.FirstOrDefault(item => item.Name == set)?.Trains ?? [];
        return
        [
            .. generated.Select(text =>
            {
                var current = existing.Where(record => record.Number == text.Number && !record.EditedForDay).Select(record => record.Text).ToList();
                var (change, changes) = ChangeOf(text, current, all);
                return new TextPreviewLine(text, current.Count > 0 ? string.Join(" | ", current) : text.ServerText ?? "", change, changes);
            })
        ];
    }

    // co sa so zaznamom vlaku stane (rovnako ako Apply) a ci to meni sadu
    private static (GeneratedTextChange Change, bool Changes) ChangeOf(GeneratedText text, List<string> current, bool all)
    {
        if (text.Text.Length == 0)
            return (GeneratedTextChange.Empty, false);
        if (WillWrite(text, current.Count > 0, all))
            return current.Count > 0 ? (GeneratedTextChange.Replace, current.Any(item => item != text.Text)) : (GeneratedTextChange.Add, true);
        // text, ktory server ukaze aj bez zaznamu: pri nahradeni vsetkych sa zaznam zmaze
        if (all && current.Count > 0)
            return (GeneratedTextChange.Replace, true);
        if (current.Count == 0 && text.IsRedundant)
            return (GeneratedTextChange.SameWithout, false);
        return current.Count == 0 && text.HasFileText ? (GeneratedTextChange.RouteOrStationText, false) : (GeneratedTextChange.Keep, false);
    }

    /// <summary>
    /// Zapise vygenerovane texty do sady. Pri <paramref name="all" /> nahradi texty vlakov stanice s vygenerovanym textom (okrem
    /// zaznamov upravenych na den; zaznam s textom, ktory server ukaze aj bez neho, sa len zmaze), inak prida len vlakom, ktore
    /// v sade text nemaju (<see cref="WillWrite" />). Vlak s prazdnym vygenerovanym textom si svoj zaznam ponecha.
    /// </summary>
    /// <param name="sets">sady TTVlaky.xml stanice</param>
    /// <param name="choice">sada, do ktorej sa generuje</param>
    /// <param name="generated">texty vlakov stanice (<see cref="Generate" />)</param>
    /// <param name="all">nahradit aj existujuce texty</param>
    /// <returns>sady (rovnaky zoznam, ak sa nic nezmenilo) a pocet zapisanych textov</returns>
    public static (IReadOnlyList<TrainTextSet> Sets, int Written) Apply(IReadOnlyList<TrainTextSet> sets, TextSetChoice choice,
        IReadOnlyList<GeneratedText> generated, bool all)
    {
        ArgumentNullException.ThrowIfNull(sets);
        ArgumentNullException.ThrowIfNull(choice);
        ArgumentNullException.ThrowIfNull(generated);
        var index = -1;
        for (var i = 0; i < sets.Count && index < 0; i++)
        {
            if (sets[i].Name == choice.Name)
                index = i;
        }

        var set = index >= 0 ? sets[index] : new TrainTextSet(choice.Name) { Targets = choice.Targets, Fonet = choice.Fonet };
        var numbers = generated.Where(text => text.Text.Length > 0).Select(text => text.Number).ToHashSet(StringComparer.Ordinal);
        var existing = set.Trains.Select(text => text.Number).ToHashSet(StringComparer.Ordinal);
        var written = generated.Where(text => WillWrite(text, existing.Contains(text.Number), all))
            .Select(text => new TrainText(text.Number, text.Validity, text.Text, TrainTextSet.NoFont)).ToList();
        var kept = all ? set.Trains.Where(text => text.EditedForDay || !numbers.Contains(text.Number)) : set.Trains;
        List<TrainText> trains = [.. kept, .. written];

        // rovnake zaznamy (pri nahradeni rovnakym textom) nie su zmena
        if (trains.Count == set.Trains.Count && !trains.Except(set.Trains).Any())
            return (sets, 0);

        var updated = set with { Trains = trains };
        List<TrainTextSet> result = [.. sets];
        if (index >= 0)
            result[index] = updated;
        else
            result.Add(updated);
        return (result, written.Count);
    }
}
