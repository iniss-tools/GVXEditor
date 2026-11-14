using System.Globalization;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Setup.Drafts;

/// <summary>Hlasenie obsluhy (ConstReports.xml): vzor z LogText so stanicami.</summary>
internal sealed class ConstReportDraft
{
    /// <summary>Cislo, na ktore sa odkazuju dodatky, vyluky a odklony, ako text v tabulke.</summary>
    public string Key { get; set; } = "";

    /// <summary>Nazov pre obsluhu.</summary>
    public string Name { get; set; } = "";

    /// <summary>Nazov vzoru v LogText.</summary>
    public string Function { get; set; } = "";

    /// <summary>Cisla stanic oddelene medzerou.</summary>
    public string Stations { get; set; } = "";

    /// <summary>Cislo hlasenia; <see langword="null" />, ak text nie je cislo.</summary>
    public int? Number =>
        int.TryParse(Key.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var number) ? number : null;

    /// <summary>Cisla stanic; <see langword="null" />, ak niektora polozka nie je cislo.</summary>
    public IReadOnlyList<int>? StationNumbers
    {
        get
        {
            var numbers = SetupText.Items(Stations).Select(SetupText.Station).ToList();
            return numbers.Any(number => number is null) ? null : [.. numbers.Select(number => number!.Value)];
        }
    }

    /// <summary>Koncept hlasenia.</summary>
    public static ConstReportDraft Of(ConstReport report)
    {
        ArgumentNullException.ThrowIfNull(report);
        return new ConstReportDraft
        {
            Key = SetupText.Number(report.Key), Name = report.Name, Function = report.Function, Stations = SetupText.Stations(report.Stations)
        };
    }

    /// <summary>Hlasenie suboru.</summary>
    public ConstReport To() => new(Number ?? 0, Name, Function, StationNumbers ?? []);
}
