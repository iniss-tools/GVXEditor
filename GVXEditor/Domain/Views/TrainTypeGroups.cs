using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Views;

/// <summary>
/// Zaradenie druhu vlaku INISS2 do skupiny farieb. Kluc druhu je v INISS2 lubovolny text, klient INISS2 preto farbi druhy
/// podla zabudovanych zoznamov kluca (predvolene styly druhov vlakov) - rovnake zoznamy pouziva aj tento program. Druh mimo
/// zoznamov sa zaradi podla kluca zvuku z TrTypes.xml (ako ho server hlasi, napr. <c>ALX</c> so zvukom <c>Ex</c>), inak je osobny.
/// </summary>
internal static class TrainTypeGroups
{
    // zoznamy klienta INISS2; kluc sa porovnava velkymi pismenami bez medzier ako v klientovi
    private static readonly Dictionary<string, TrainTypeGroup> Builtin = Build(
        (TrainTypeGroup.Os, ["Os", "TL", "MOs", "Sp", "Bus", "ZvlOs", "ZvlR", "ZvlSp", "ZvlEx"]),
        (TrainTypeGroup.R, ["R", "Rx", "Ex", "REx", "ER", "AEx", "TLX", "D", "S"]),
        (TrainTypeGroup.X, ["EC", "IC", "SC", "RJ", "RLJ", "LE", "ICE", "EN", "CNL", "DPN", "NZ", "TGV"]),
        (TrainTypeGroup.Sl, ["Sl", "Nákl", "Rn", "Rp"]));

    private static Dictionary<string, TrainTypeGroup> Build(params (TrainTypeGroup Group, string[] Keys)[] groups) =>
        groups.SelectMany(group => group.Keys.Select(key => (Key: Normalize(key), group.Group)))
            .ToDictionary(pair => pair.Key, pair => pair.Group, StringComparer.Ordinal);

    private static string Normalize(string key) => key.Replace(" ", "", StringComparison.Ordinal).ToUpperInvariant();

    /// <summary>
    /// Skupina druhu vlaku.
    /// </summary>
    /// <param name="type">druh vlaku z GV.XML</param>
    /// <param name="types">druhy z TrTypes.xml (kluc zvuku pre druh mimo zabudovanych zoznamov)</param>
    public static TrainTypeGroup Of(string type, IEnumerable<TrType> types)
    {
        ArgumentNullException.ThrowIfNull(type);
        ArgumentNullException.ThrowIfNull(types);
        if (Builtin.TryGetValue(Normalize(type), out var group))
            return group;

        var sound = types.FirstOrDefault(definition => string.Equals(definition.Key, type, StringComparison.Ordinal))?.Sound;
        return sound is { Length: > 0 } && Builtin.TryGetValue(Normalize(sound), out var bySound) ? bySound : TrainTypeGroup.Os;
    }
}
