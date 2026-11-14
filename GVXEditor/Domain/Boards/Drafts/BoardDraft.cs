using System.ComponentModel;
using System.Globalization;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Boards.Drafts;

/// <summary>
/// Tabula stanice v okne. Adresa a jas su text ako v subore, chybu hlasia pravidla okna.
/// </summary>
internal sealed class BoardDraft
{
    /// <summary>Tabula zo suboru; <see langword="null" /> = nova tabula.</summary>
    public TableDefinition? Original { get; private init; }

    /// <summary>Kluc, v stanici jedinecny.</summary>
    public string Key { get; set; } = "";

    /// <summary>Nazov.</summary>
    public string Name { get; set; } = "";

    /// <summary>Kluc linky z CommLines.xml; prazdny = prva linka s protokolom druhu tabule.</summary>
    public string Line { get; set; } = "";

    /// <summary>Podriadena linka pre ovladac zariadenia.</summary>
    public string SubLine { get; set; } = "";

    /// <summary>Adresa na linke ako text; -1 = tabula nekomunikuje.</summary>
    public string Address { get; set; } = "-1";

    /// <summary>Subor v priecinku LOG s obsahom tabule.</summary>
    public string SaveFile { get; set; } = "";

    /// <summary>Cisla zaznamov s otocenymi sipkami.</summary>
    public string ReverseArrows { get; set; } = "";

    /// <summary>Zariadenie nema rolovat zaznamy.</summary>
    public bool NoScroll { get; set; }

    /// <summary>Zaciatok denneho obdobia jasu.</summary>
    public string LightTime0 { get; set; } = "";

    /// <summary>Koniec denneho obdobia jasu.</summary>
    public string LightTime1 { get; set; } = "";

    /// <summary>Jas v dennom obdobi.</summary>
    public string Light0 { get; set; } = "";

    /// <summary>Jas mimo denneho obdobia.</summary>
    public string Light1 { get; set; } = "";

    /// <summary>Jas prazdnej tabule v dennom obdobi.</summary>
    public string LightOff0 { get; set; } = "";

    /// <summary>Jas prazdnej tabule mimo denneho obdobia.</summary>
    public string LightOff1 { get; set; } = "";

    /// <summary>Prazdna tabula svieti v dennom obdobi (true/false, prazdne = z linky).</summary>
    public string LightIfEmpty0 { get; set; } = "";

    /// <summary>Prazdna tabula svieti mimo denneho obdobia.</summary>
    public string LightIfEmpty1 { get; set; } = "";

    /// <summary>Najkratsi cas v ms po zmene jasu.</summary>
    public string LightOnTime { get; set; } = "";

    /// <summary>Casti tabule v poradi suboru.</summary>
    public BindingList<PartDraft> Parts { get; } = [];

    /// <summary>Adresa; <see langword="null" />, ak text nie je cislo.</summary>
    public int? AddressNumber =>
        int.TryParse(Address.Trim(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var address) ? address : null;

    /// <summary>Text v strome okna.</summary>
    public override string ToString() => Name.Length > 0 && Name != Key ? $"{Key} ({Name})" : Key;

    /// <summary>Koncept tabule.</summary>
    public static BoardDraft Of(TableDefinition table)
    {
        ArgumentNullException.ThrowIfNull(table);
        var draft = new BoardDraft
        {
            Original = table,
            Key = table.Key,
            Name = table.Name,
            Line = table.Line,
            SubLine = table.SubLine,
            Address = table.Address.ToString(CultureInfo.InvariantCulture),
            SaveFile = table.SaveFile,
            ReverseArrows = table.ReverseArrows,
            NoScroll = table.NoScroll,
            LightTime0 = table.LightTime0,
            LightTime1 = table.LightTime1,
            Light0 = table.Light0,
            Light1 = table.Light1,
            LightOff0 = table.LightOff0,
            LightOff1 = table.LightOff1,
            LightIfEmpty0 = table.LightIfEmpty0,
            LightIfEmpty1 = table.LightIfEmpty1,
            LightOnTime = table.LightOnTime
        };
        foreach (var part in table.Parts)
            draft.Parts.Add(PartDraft.Of(part));
        return draft;
    }

    /// <summary>Tabula suboru; bez zmeny vrati povodnu tabulu (ten isty objekt).</summary>
    public TableDefinition To()
    {
        var table = new TableDefinition(Key, Name, Line, AddressNumber ?? TableDefinition.NoAddress, SaveFile, [.. Parts.Select(part => part.To())])
        {
            ReverseArrows = ReverseArrows,
            SubLine = SubLine,
            NoScroll = NoScroll,
            LightTime0 = LightTime0,
            LightTime1 = LightTime1,
            Light0 = Light0,
            Light1 = Light1,
            LightOff0 = LightOff0,
            LightOff1 = LightOff1,
            LightIfEmpty0 = LightIfEmpty0,
            LightIfEmpty1 = LightIfEmpty1,
            LightOnTime = LightOnTime
        };
        return Original is { } original && BoardEquality.Same(original, table) ? original : table;
    }
}
