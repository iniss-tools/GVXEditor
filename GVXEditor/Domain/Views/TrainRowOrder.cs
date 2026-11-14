namespace GVXEditor.Domain.Views;

/// <summary>
/// Poradie riadkov zoznamu vlakov: bez triedenia ako v GV.XML, inak podla stlpca. Rovnake hodnoty ostavaju v poradi suboru
/// v oboch smeroch.
/// </summary>
internal static class TrainRowOrder
{
    /// <summary>
    /// Zoradene riadky.
    /// </summary>
    /// <param name="rows">riadky vlakov</param>
    /// <param name="property">vlastnost riadka (stlpec), <see langword="null" /> = poradie GV.XML</param>
    /// <param name="descending">zostupne</param>
    public static List<TrainRow> Sort(IEnumerable<TrainRow> rows, string? property, bool descending)
    {
        ArgumentNullException.ThrowIfNull(rows);
        if (property is null || Comparer(property) is not { } comparer)
            return [.. rows.OrderBy(row => row.Index)];

        var ordered = descending ? rows.OrderByDescending(row => row, comparer) : rows.OrderBy(row => row, comparer);
        return [.. ordered.ThenBy(row => row.Index)];
    }

    /// <summary>
    /// Stlpec sa da triedit.
    /// </summary>
    public static bool CanSort(string? property) => property is not null && Comparer(property) is not null;

    private static Comparer<TrainRow>? Comparer(string property) => property switch
    {
        nameof(TrainRow.Numbers) => Text(row => row.Numbers),
        nameof(TrainRow.Type) => Text(row => row.Type),
        nameof(TrainRow.Name) => Text(row => row.Name),
        nameof(TrainRow.From) => Text(row => row.From),
        nameof(TrainRow.To) => Text(row => row.To),
        nameof(TrainRow.Operator) => Text(row => row.Operator),
        nameof(TrainRow.Departure) => Value(row => row.DepartureMinutes),
        nameof(TrainRow.Arrival) => Value(row => row.ArrivalMinutes),
        nameof(TrainRow.Validity) => Comparer<TrainRow>.Create((a, b) =>
            a.Train.From != b.Train.From ? a.Train.From.CompareTo(b.Train.From) : a.Train.To.CompareTo(b.Train.To)),
        nameof(TrainRow.Variants) => Value(row => (int?)row.Variants),
        _ => null
    };

    private static Comparer<TrainRow> Text(Func<TrainRow, string> value) =>
        Comparer<TrainRow>.Create((a, b) => NaturalText.Compare(value(a), value(b)));

    // bez hodnoty (napr. vlak bez casu odchodu) pred hodnotou
    private static Comparer<TrainRow> Value(Func<TrainRow, int?> value) =>
        Comparer<TrainRow>.Create((a, b) => Nullable.Compare(value(a), value(b)));
}
