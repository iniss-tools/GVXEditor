using System.ComponentModel;
using GVXEditor.Domain.Stations.Drafts;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Stations;

/// <summary>
/// Zaznamy vlaku v jednej stanici (podla cisla pri odchode) v okne Upravit vlak. Zaznamy inych cisel sa nemenia.
/// </summary>
internal sealed class StationEdit
{
    private StationEdit(TrainStationKey key, StationDocument station)
    {
        Key = key;
        Station = station;
        TextSetNames = station.Files?.TextSets.Select(set => set.Name).Distinct(StringComparer.Ordinal).ToList() ?? [];
    }

    /// <summary>Stanica a cislo vlaku v nej.</summary>
    public TrainStationKey Key { get; }

    /// <summary>Stanica so subormi pred upravou.</summary>
    public StationDocument Station { get; }

    /// <summary>Nazvy sad textov tabul stanice - text vlaku patri do niektorej z nich.</summary>
    public IReadOnlyList<string> TextSetNames { get; }

    /// <summary>Kolaje.</summary>
    public BindingList<PositionDraft> Positions { get; } = [];

    /// <summary>Dodatky.</summary>
    public BindingList<AdditionDraft> Additions { get; } = [];

    /// <summary>Radenie.</summary>
    public BindingList<CompositionDraft> Compositions { get; } = [];

    /// <summary>Jazyky hlasenia.</summary>
    public BindingList<LanguagesDraft> Languages { get; } = [];

    /// <summary>Stanice v hlaseni.</summary>
    public BindingList<ReportStationsDraft> ReportStations { get; } = [];

    /// <summary>Texty tabul.</summary>
    public BindingList<TextDraft> Texts { get; } = [];

    /// <summary>Motorovy osobny vlak.</summary>
    public BindingList<MosDraft> Mos { get; } = [];

    /// <summary>
    /// Zaznamy vlaku v stanici; stanica, ktorej subory sa nedaju nacitat, nema ziadne.
    /// </summary>
    public static StationEdit Of(TrainStationKey key, StationDocument station)
    {
        ArgumentNullException.ThrowIfNull(key);
        ArgumentNullException.ThrowIfNull(station);

        var edit = new StationEdit(key, station);
        if (station.Files is not { } files)
            return edit;

        var number = key.Number;
        Fill(edit.Positions, files.Positions.Where(record => record.Train == number).Select(PositionDraft.Of));
        Fill(edit.Additions, files.Additions.Where(record => record.Number == number).Select(AdditionDraft.Of));
        Fill(edit.Compositions, files.Compositions.Where(record => record.Number == number).Select(CompositionDraft.Of));
        Fill(edit.Languages, files.Languages.Where(record => record.Number == number).Select(LanguagesDraft.Of));
        Fill(edit.ReportStations, files.ReportStations.Where(record => record.Number == number).Select(ReportStationsDraft.Of));
        Fill(edit.Texts, files.TextSets.SelectMany(set => set.Trains.Where(text => text.Number == number).Select(text => TextDraft.Of(set.Name, text))));
        Fill(edit.Mos, files.Mos.Where(record => record.Number == number).Select(MosDraft.Of));
        return edit;
    }

    private static void Fill<T>(BindingList<T> list, IEnumerable<T> items)
    {
        foreach (var item in items)
            list.Add(item);
    }

    /// <summary>
    /// Subory stanice so zaznamami vlaku z okna. Nezmenene zoznamy ostanu tie iste objekty, bez zmeny vrati
    /// <paramref name="files" />.
    /// </summary>
    public StationFiles Apply(StationFiles files)
    {
        ArgumentNullException.ThrowIfNull(files);
        var number = Key.Number;

        var updated = files with
        {
            Positions = RecordLists.Replace(files.Positions, record => record.Train == number, [.. Positions.Select(draft => draft.To(number))],
                RecordEquality.Same),
            Additions = RecordLists.Replace(files.Additions, record => record.Number == number, [.. Additions.Select(draft => draft.To(number))],
                RecordEquality.Same),
            Compositions = RecordLists.Replace(files.Compositions, record => record.Number == number,
                [.. Compositions.Select(draft => draft.To(number))], RecordEquality.Same),
            Languages = RecordLists.Replace(files.Languages, record => record.Number == number, [.. Languages.Select(draft => draft.To(number))],
                RecordEquality.Same),
            ReportStations = RecordLists.Replace(files.ReportStations, record => record.Number == number,
                [.. ReportStations.Select(draft => draft.To(number))], RecordEquality.Same),
            TextSets = ApplyTexts(files.TextSets, number),
            Mos = RecordLists.Replace(files.Mos, record => record.Number == number, [.. Mos.Select(draft => draft.To(number))], RecordEquality.Same)
        };

        return ReferenceEquals(updated.Positions, files.Positions) && ReferenceEquals(updated.Additions, files.Additions) &&
               ReferenceEquals(updated.Compositions, files.Compositions) && ReferenceEquals(updated.Languages, files.Languages) &&
               ReferenceEquals(updated.ReportStations, files.ReportStations) && ReferenceEquals(updated.TextSets, files.TextSets) &&
               ReferenceEquals(updated.Mos, files.Mos)
            ? files
            : updated;
    }

    // texty vlaku po sadach: v kazdej sade sa nahradia texty s cislom vlaku textami z okna pre tuto sadu
    private IReadOnlyList<TrainTextSet> ApplyTexts(IReadOnlyList<TrainTextSet> sets, string number)
    {
        var changed = false;
        var result = sets.Select(set =>
        {
            var texts = Texts.Where(draft => draft.Set == set.Name).Select(draft => draft.To(number)).ToList();
            var trains = RecordLists.Replace(set.Trains, text => text.Number == number, texts, RecordEquality.Same);
            if (ReferenceEquals(trains, set.Trains))
                return set;
            changed = true;
            return set with { Trains = trains };
        }).ToList();

        return changed ? result : sets;
    }
}
