using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IniSrvBridge;

/// <summary>
/// Vysledok overenia kodom servera.
/// </summary>
internal sealed class BridgeResult
{
    /// <summary>Server by sa spustil: ziadna chyba nacitania ani prekladu.</summary>
    public bool Ok => Fatal is null && CompileErrors.Count == 0;

    /// <summary>Chyba, pre ktoru by sa server nespustil; <see langword="null" /> = ziadna.</summary>
    public string? Fatal { get; set; }

    /// <summary>Krok, v ktorom nastala <see cref="Fatal" /> (subor alebo preklad).</summary>
    public string? Stage { get; set; }

    /// <summary>Chyby prekladu kodu.</summary>
    public List<CompileError> CompileErrors { get; } = [];

    /// <summary>Zaznamy, ktore server pri nacitani zapisal do Error.log.</summary>
    public List<string> Log { get; } = [];

    /// <summary>Kroky, ktore prebehli, v poradi.</summary>
    public List<string> Stages { get; } = [];

    /// <summary>Trvanie v ms.</summary>
    public long Milliseconds { get; set; }

    /// <summary>
    /// JSON pre GVXEditor (rucne - .NET Framework nema System.Text.Json).
    /// </summary>
    public string ToJson()
    {
        var json = new StringBuilder();
        json.Append('{');
        json.Append("\"ok\":").Append(Ok ? "true" : "false");
        json.Append(",\"fatal\":").Append(Text(Fatal));
        json.Append(",\"stage\":").Append(Text(Stage));
        json.Append(",\"milliseconds\":").Append(Milliseconds.ToString(CultureInfo.InvariantCulture));
        json.Append(",\"stages\":[").Append(string.Join(",", Stages.ConvertAll(Text))).Append(']');
        json.Append(",\"log\":[").Append(string.Join(",", Log.ConvertAll(Text))).Append(']');
        json.Append(",\"compileErrors\":[");
        for (var i = 0; i < CompileErrors.Count; i++)
        {
            var error = CompileErrors[i];
            if (i > 0)
                json.Append(',');
            json.Append("{\"source\":").Append(Text(error.Source))
                .Append(",\"line\":").Append(error.Line.ToString(CultureInfo.InvariantCulture))
                .Append(",\"column\":").Append(error.Column.ToString(CultureInfo.InvariantCulture))
                .Append(",\"number\":").Append(Text(error.Number))
                .Append(",\"message\":").Append(Text(error.Message))
                .Append(",\"description\":").Append(Text(error.Description))
                .Append(",\"code\":").Append(Text(error.Code))
                .Append('}');
        }

        json.Append("]}");
        return json.ToString();
    }

    private static string Text(string? value)
    {
        if (value is null)
            return "null";

        var text = new StringBuilder(value.Length + 2);
        text.Append('"');
        foreach (var c in value)
        {
            switch (c)
            {
                case '"':
                    text.Append("\\\"");
                    break;
                case '\\':
                    text.Append("\\\\");
                    break;
                case '\n':
                    text.Append("\\n");
                    break;
                case '\r':
                    text.Append("\\r");
                    break;
                case '\t':
                    text.Append("\\t");
                    break;
                default:
                    if (c < ' ')
                        text.Append("\\u").Append(((int)c).ToString("x4", CultureInfo.InvariantCulture));
                    else
                        text.Append(c);
                    break;
            }
        }

        return text.Append('"').ToString();
    }
}
