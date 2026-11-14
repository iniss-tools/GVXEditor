namespace GVXEditor.Domain.Stations.Drafts;

/// <summary>
/// Zaznam suboru stanice v tabulke okna Upravit vlak: datumova poznamka a priznak upravy na den. Cislo vlaku dostane az pri
/// prevode na zaznam - urcuje ho stanica, v ktorej sa zaznamy upravuju.
/// </summary>
internal abstract class RecordDraft
{
    /// <summary>Datumova poznamka (<c>jd</c>); prazdna = vzdy.</summary>
    public string Validity { get; set; } = "";

    /// <summary>Zaznam upravila obsluha pre konkretny den (<c>de</c>) - server ho po case zmaze.</summary>
    public bool EditedForDay { get; set; }

    /// <summary>Medzerou oddelene polozky textu.</summary>
    protected static List<string> Items(string text) => [.. text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)];
}
