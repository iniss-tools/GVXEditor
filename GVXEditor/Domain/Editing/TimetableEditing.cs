using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Editing;

/// <summary>
/// Zmeny vlakov grafikonu. Kazda vrati novy <see cref="GvFile" />, povodny sa nemeni.
/// </summary>
internal static class TimetableEditing
{
    /// <summary>
    /// Nahradi vlak na danom mieste.
    /// </summary>
    public static GvFile Replace(GvFile timetable, int index, GvTrain train) => With(timetable, trains => trains[index] = train);

    /// <summary>
    /// Prida vlak na koniec - ako server pri ulozeni vlaku z klienta.
    /// </summary>
    public static GvFile Add(GvFile timetable, GvTrain train) => With(timetable, trains => trains.Add(train));

    /// <summary>
    /// Vlozi kopiu vlaku hned za neho.
    /// </summary>
    public static GvFile Copy(GvFile timetable, int index) => With(timetable, trains => trains.Insert(index + 1, trains[index]));

    /// <summary>
    /// Vlozi kopiu kazdeho z vlakov hned za neho (vyber viacerych vlakov). Kopia vlaku s k-tym najmensim poradim je potom na
    /// mieste <c>index + k + 1</c>.
    /// </summary>
    public static GvFile Copy(GvFile timetable, IEnumerable<int> indexes) => With(timetable, trains =>
    {
        foreach (var index in indexes.Distinct().OrderDescending())
            trains.Insert(index + 1, trains[index]);
    });

    /// <summary>
    /// Odstrani vlak.
    /// </summary>
    public static GvFile Remove(GvFile timetable, int index) => With(timetable, trains => trains.RemoveAt(index));

    /// <summary>
    /// Odstrani vlaky (vyber viacerych vlakov).
    /// </summary>
    public static GvFile Remove(GvFile timetable, IEnumerable<int> indexes) => With(timetable, trains =>
    {
        foreach (var index in indexes.Distinct().OrderDescending())
            trains.RemoveAt(index);
    });

    /// <summary>
    /// Posunie obdobie platnosti vlakov o <paramref name="days" /> dni. Datumove poznamky ostanu - su to pravidla
    /// (dni v tyzdni, sviatky, datumy bez roku), ktore server rozvinie do noveho obdobia. Vlak bez zaciatku alebo konca ho
    /// nedostane.
    /// </summary>
    /// <param name="timetable">grafikon</param>
    /// <param name="days">pocet dni, zaporny posunie spat</param>
    /// <param name="only">poradie vlakov, ktore sa posunu (vyber viacerych vlakov); <see langword="null" /> = vsetky</param>
    public static GvFile ShiftValidity(GvFile timetable, int days, IReadOnlySet<int>? only = null)
    {
        ArgumentNullException.ThrowIfNull(timetable);
        return timetable with
        {
            Trains = [.. timetable.Trains.Select((train, index) => only is not null && !only.Contains(index) ? train : train with
            {
                From = train.From == DateOnly.MinValue ? train.From : train.From.AddDays(days),
                To = train.To == DateOnly.MaxValue ? train.To : train.To.AddDays(days)
            })]
        };
    }

    /// <summary>
    /// Vlaky sa zhoduju vo vsetkom, co sa zapisuje do GV.XML (okno Upravit vlak potvrdene bez zmeny nie je zmena).
    /// </summary>
    public static bool SameTrain(GvTrain a, GvTrain b)
    {
        ArgumentNullException.ThrowIfNull(a);
        ArgumentNullException.ThrowIfNull(b);
        return a.From == b.From && a.To == b.To && a.Variants.Count == b.Variants.Count &&
               a.Variants.Zip(b.Variants).All(pair => SameVariant(pair.First, pair.Second));
    }

    private static bool SameVariant(GvVariant a, GvVariant b) =>
        a.Validity == b.Validity && a.Operator == b.Operator && a.Flags == b.Flags && a.Note == b.Note && a.Stops.SequenceEqual(b.Stops);

    private static GvFile With(GvFile timetable, Action<List<GvTrain>> change)
    {
        ArgumentNullException.ThrowIfNull(timetable);
        var trains = timetable.Trains.ToList();
        change(trains);
        return timetable with { Trains = trains };
    }
}
