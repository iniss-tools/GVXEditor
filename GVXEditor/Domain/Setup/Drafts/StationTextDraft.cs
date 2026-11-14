using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Setup.Drafts;

/// <summary>Text stanice na tabuli (TTStanice.xml).</summary>
internal sealed class StationTextDraft
{
    /// <summary>Cislo stanice ako text v tabulke.</summary>
    public string Station { get; set; } = "";

    /// <summary>Text.</summary>
    public string Text { get; set; } = "";

    /// <summary>Cislo pisma; prazdne = pismo predlohy.</summary>
    public string Font { get; set; } = "";

    /// <summary>Cislo stanice; <see langword="null" />, ak text nie je cislo.</summary>
    public int? Number => SetupText.Station(Station);

    /// <summary>Koncept textu.</summary>
    public static StationTextDraft Of(StationText text)
    {
        ArgumentNullException.ThrowIfNull(text);
        return new StationTextDraft { Station = SetupText.Number(text.Station), Text = text.Text, Font = SetupText.Font(text.Font) };
    }

    /// <summary>Text suboru.</summary>
    public StationText To() => new(Number ?? 0, Text, SetupText.Font(Font) ?? TrainTextSet.NoFont);
}
