using System.Globalization;

namespace GVXEditor.Domain.Stations.Drafts;

/// <summary>
/// Vyluka alebo odklon: kluc a obdobie, do ktoreho sa rozvija poznamka. Datumy su text v kulture UI, prazdny = bez datumu.
/// </summary>
internal abstract class NoticeDraft : RecordDraft
{
    /// <summary>Kluc (funkcia LogText alebo cislo hlasenia ConstReports).</summary>
    public string Key { get; set; } = "";

    /// <summary>Prvy den obdobia.</summary>
    public string From { get; set; } = "";

    /// <summary>Posledny den obdobia; prazdny = bez konca.</summary>
    public string To { get; set; } = "";

    /// <summary>Datum ako text pre tabulku.</summary>
    protected static string Text(DateOnly date) => date == DateOnly.MinValue ? "" : date.ToString("d", CultureInfo.CurrentCulture);

    /// <summary>
    /// Datum z textu; prazdny text = <see cref="DateOnly.MinValue" /> (v subore bez atributu), <see langword="null" />, ak text
    /// nie je datum.
    /// </summary>
    public static DateOnly? Date(string text) =>
        text.Trim().Length == 0 ? DateOnly.MinValue :
        DateOnly.TryParse(text.Trim(), CultureInfo.CurrentCulture, DateTimeStyles.None, out var date) ? date : null;

    /// <summary>Oba datumy sa daju precitat.</summary>
    public bool HasValidDates => Date(From) is not null && Date(To) is not null;
}
