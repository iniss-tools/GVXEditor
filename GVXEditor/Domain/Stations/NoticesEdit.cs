using System.ComponentModel;
using GVXEditor.Domain.Stations.Drafts;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Stations;

/// <summary>
/// Vyluky a odklony vlaku v okne Upravit vlak: vyluky pod cislom vlaku v grafikone, odklony pod cislom pri odchode zo
/// stanice.
/// </summary>
internal sealed class NoticesEdit
{
    private readonly IReadOnlyList<TrainStationKey> _keys;

    private NoticesEdit(string lockoutNumber, IReadOnlyList<TrainStationKey> keys)
    {
        LockoutNumber = lockoutNumber;
        _keys = keys;
    }

    /// <summary>Cislo, pod ktorym ma vlak vyluky; prazdne = vlak stanicou grafikonu nejde.</summary>
    public string LockoutNumber { get; }

    /// <summary>Stanice vlaku s cislom pri odchode.</summary>
    public IReadOnlyList<TrainStationKey> Keys => _keys;

    /// <summary>Vyluky.</summary>
    public BindingList<LockoutDraft> Lockouts { get; } = [];

    /// <summary>Odklony.</summary>
    public BindingList<DiversionDraft> Diversions { get; } = [];

    /// <summary>
    /// Vyluky a odklony vlaku z grafikonu.
    /// </summary>
    /// <param name="lockoutNumber">cislo vlaku pre vyluky</param>
    /// <param name="keys">stanice vlaku s cislom pri odchode</param>
    /// <param name="group">grafikon</param>
    public static NoticesEdit Of(string lockoutNumber, IReadOnlyList<TrainStationKey> keys, GroupDocument group)
    {
        ArgumentNullException.ThrowIfNull(group);
        var edit = new NoticesEdit(lockoutNumber, keys);
        if (lockoutNumber.Length > 0)
        {
            foreach (var lockout in group.Lockouts?.Where(record => record.Number == lockoutNumber) ?? [])
                edit.Lockouts.Add(LockoutDraft.Of(lockout));
        }

        foreach (var diversion in group.Diversions ?? [])
        {
            if (edit.KeyOf(diversion) is { } key)
                edit.Diversions.Add(DiversionDraft.Of(diversion, key));
        }

        return edit;
    }

    private TrainStationKey? KeyOf(TrainDiversion diversion) =>
        _keys.FirstOrDefault(key => key.Station.Station == diversion.Station && key.Number == diversion.Number);

    /// <summary>
    /// Vyluky so zaznamami vlaku z okna; bez zmeny vrati <paramref name="lockouts" />.
    /// </summary>
    public IReadOnlyList<TrainLockout> ApplyLockouts(IReadOnlyList<TrainLockout> lockouts) => LockoutNumber.Length == 0
        ? lockouts
        : RecordLists.Replace(lockouts, record => record.Number == LockoutNumber, [.. Lockouts.Select(draft => draft.ToRecord(LockoutNumber))],
            RecordEquality.Same);

    /// <summary>
    /// Odklony so zaznamami vlaku z okna; bez zmeny vrati <paramref name="diversions" />.
    /// </summary>
    public IReadOnlyList<TrainDiversion> ApplyDiversions(IReadOnlyList<TrainDiversion> diversions) =>
        RecordLists.Replace(diversions, record => KeyOf(record) is not null, [.. Diversions.Select(draft => draft.ToRecord()).OfType<TrainDiversion>()],
            RecordEquality.Same);
}
