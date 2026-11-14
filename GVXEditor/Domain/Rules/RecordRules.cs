using System.Globalization;
using GVXEditor.Domain.Stations;
using GVXEditor.Domain.Stations.Drafts;
using GVXEditor.Properties;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Rules;

/// <summary>
/// Kontrola zaznamov vlaku v suboroch stanic, vylukach a odklonoch: chyby (text, ktory nie je cislo alebo datum), pre ktore
/// sa zaznam neda zapisat, a upozornenia na zaznamy, ktore server nepouzije.
/// </summary>
internal static class RecordRules
{
    /// <summary>
    /// Zistenia zaznamov vlaku v stanici.
    /// </summary>
    /// <param name="edit">zaznamy vlaku v stanici</param>
    /// <param name="period">obdobie, ked vlak s cislom stanicou ide - do neho server rozvija poznamky; <see langword="null" /> = ziadne</param>
    /// <param name="keys">kluce dodatkov</param>
    /// <param name="stationName">nazov stanice pre hlasenia</param>
    public static List<TrainProblem> Check(StationEdit edit, (DateOnly From, DateOnly To)? period, KeyCatalog keys, string stationName)
    {
        ArgumentNullException.ThrowIfNull(edit);
        ArgumentNullException.ThrowIfNull(keys);

        var problems = new List<TrainProblem>();
        void Validity(string file, IEnumerable<RecordDraft> records)
        {
            if (period is not { } range)
                return;
            foreach (var (record, i) in records.Select((record, i) => (record, i)))
            {
                if (GvValidity.Check(range.From, range.To, record.Validity.Trim()) != GvValidityProblem.None)
                    problems.Add(Warning(Text(Resources.Rule_RecordValidity, file, stationName, i + 1, record.Validity)));
            }
        }

        Validity(Resources.File_Positions, edit.Positions);
        Validity(Resources.File_Additions, edit.Additions);
        Validity(Resources.File_Compositions, edit.Compositions);
        Validity(Resources.File_Languages, edit.Languages);
        Validity(Resources.File_ReportStations, edit.ReportStations);
        Validity(Resources.File_Texts, edit.Texts);
        Validity(Resources.File_Mos, edit.Mos);

        var tracks = edit.Station.Tracks.Select(track => track.Key).ToHashSet(StringComparer.Ordinal);
        for (var i = 0; i < edit.Positions.Count; i++)
        {
            foreach (var track in new[] { edit.Positions[i].Arrival.Trim(), edit.Positions[i].Departure.Trim() })
            {
                if (tracks.Count > 0 && track.Length > 0 && !tracks.Contains(track))
                    problems.Add(Warning(Text(Resources.Rule_RecordTrack, stationName, i + 1, track)));
            }
        }

        for (var i = 0; i < edit.Additions.Count; i++)
        {
            if (!keys.IsKnown(edit.Additions[i].Key))
                problems.Add(Warning(Text(Resources.Rule_RecordKey, Resources.File_Additions, i + 1, edit.Additions[i].Key)));
        }

        for (var i = 0; i < edit.Compositions.Count; i++)
        {
            if (edit.Compositions[i].EndStationNumber is null)
                problems.Add(Error(Text(Resources.Rule_RecordNumber, Resources.File_Compositions, stationName, i + 1, edit.Compositions[i].EndStation)));
        }

        for (var i = 0; i < edit.ReportStations.Count; i++)
        {
            if (!edit.ReportStations[i].IsValid)
                problems.Add(Error(Text(Resources.Rule_RecordNumber, Resources.File_ReportStations, stationName, i + 1, "")));
        }

        for (var i = 0; i < edit.Texts.Count; i++)
        {
            var text = edit.Texts[i];
            if (!edit.TextSetNames.Contains(text.Set))
                problems.Add(Error(Text(Resources.Rule_RecordSet, stationName, i + 1)));
            if (text.FontNumber is null)
                problems.Add(Error(Text(Resources.Rule_RecordNumber, Resources.File_Texts, stationName, i + 1, text.Font)));
        }

        return problems;
    }

    /// <summary>
    /// Zistenia vyluk a odklonov vlaku.
    /// </summary>
    public static List<TrainProblem> Check(NoticesEdit notices, KeyCatalog keys)
    {
        ArgumentNullException.ThrowIfNull(notices);
        ArgumentNullException.ThrowIfNull(keys);

        var problems = new List<TrainProblem>();
        void Notice(string file, NoticeDraft notice, int i)
        {
            if (!notice.HasValidDates)
            {
                problems.Add(Error(Text(Resources.Rule_NoticeDate, file, i + 1)));
                return;
            }

            // bez konca server pocita do 31. 12. 9999 - textova poznamka potom neplati
            var from = NoticeDraft.Date(notice.From)!.Value;
            var to = NoticeDraft.Date(notice.To)!.Value;
            if (GvValidity.Check(from, to == DateOnly.MinValue ? DateOnly.MaxValue : to, notice.Validity.Trim()) != GvValidityProblem.None)
                problems.Add(Warning(Text(Resources.Rule_NoticeValidity, file, i + 1, notice.Validity)));
            if (!keys.IsKnown(notice.Key))
                problems.Add(Warning(Text(Resources.Rule_RecordKey, file, i + 1, notice.Key)));
        }

        for (var i = 0; i < notices.Lockouts.Count; i++)
            Notice(Resources.File_Lockouts, notices.Lockouts[i], i);
        for (var i = 0; i < notices.Diversions.Count; i++)
        {
            if (notices.Diversions[i].Station is null)
                problems.Add(Error(Text(Resources.Rule_DiversionStation, i + 1)));
            Notice(Resources.File_Diversions, notices.Diversions[i], i);
        }

        return problems;
    }

    private static TrainProblem Error(string message) => new(true, -1, -1, message);

    private static TrainProblem Warning(string message) => new(false, -1, -1, message);

    private static string Text(string format, params object[] args) => string.Format(CultureInfo.CurrentCulture, format, args);
}
