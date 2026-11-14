using System.Globalization;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Setup;

/// <summary>
/// Prevod cisel stanic a pisma medzi textom v tabulke okna a hodnotou suboru.
/// </summary>
internal static class SetupText
{
    /// <summary>
    /// Cislo stanice; <see langword="null" />, ak text nie je kladne cislo.
    /// </summary>
    public static int? Station(string text) =>
        int.TryParse(text.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var number) && number > 0 ? number : null;

    /// <summary>
    /// Text cisla pre tabulku.
    /// </summary>
    public static string Number(int number) => number.ToString(CultureInfo.InvariantCulture);

    /// <summary>
    /// Cislo pisma; prazdny text = pismo predlohy, <see langword="null" />, ak text nie je cislo.
    /// </summary>
    public static int? Font(string text) => text.Trim().Length == 0 ? TrainTextSet.NoFont :
        int.TryParse(text.Trim(), NumberStyles.None, CultureInfo.InvariantCulture, out var font) ? font : null;

    /// <summary>
    /// Text pisma pre tabulku; pismo predlohy je prazdny text.
    /// </summary>
    public static string Font(int font) => font == TrainTextSet.NoFont ? "" : Number(font);

    /// <summary>
    /// Cisla stanic oddelene medzerou.
    /// </summary>
    public static string Stations(IEnumerable<int> stations) => string.Join(" ", stations.Select(Number));

    /// <summary>
    /// Polozky textu oddelene medzerou.
    /// </summary>
    public static string[] Items(string text) => text.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
}
