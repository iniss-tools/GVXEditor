using System.Globalization;

namespace GVXEditor.Domain.Views;

/// <summary>
/// Cas v minutach od polnoci ako text pre obsluhu.
/// </summary>
internal static class TimeText
{
    /// <summary>
    /// Cas <c>HH:mm</c>, pri dalsom dni s poctom dni (<c>00:10 +1</c>). Bez casu prazdny text.
    /// </summary>
    /// <param name="minutes">minuty od polnoci; <see langword="null" /> = bez casu</param>
    /// <param name="day">o kolko dni neskor</param>
    public static string Format(int? minutes, int day = 0)
    {
        if (minutes is not { } value)
            return "";

        var text = string.Create(CultureInfo.InvariantCulture, $"{value / 60:00}:{value % 60:00}");
        return day > 0 ? string.Create(CultureInfo.InvariantCulture, $"{text} +{day}") : text;
    }

    /// <summary>
    /// Cas zadany obsluhou: <c>8:15</c>, <c>08:15</c>, <c>815</c> alebo <c>0815</c>. Prazdny text je bez casu.
    /// </summary>
    /// <param name="text">text</param>
    /// <param name="minutes">minuty od polnoci; <see langword="null" /> = bez casu</param>
    /// <returns><see langword="false" />, ak text nie je cas.</returns>
    public static bool TryParse(string? text, out int? minutes)
    {
        minutes = null;
        var value = text?.Trim() ?? "";
        if (value.Length == 0)
            return true;

        string hours, rest;
        var colon = value.IndexOf(':', StringComparison.Ordinal);
        if (colon >= 0)
        {
            hours = value[..colon];
            rest = value[(colon + 1)..];
        }
        else if (value.Length is 3 or 4)
        {
            hours = value[..^2];
            rest = value[^2..];
        }
        else
        {
            return false;
        }

        if (rest.Length != 2 || hours.Length is 0 or > 2 ||
            !int.TryParse(hours, NumberStyles.None, CultureInfo.InvariantCulture, out var h) ||
            !int.TryParse(rest, NumberStyles.None, CultureInfo.InvariantCulture, out var m) || h > 23 || m > 59)
            return false;

        minutes = h * 60 + m;
        return true;
    }
}
