using System.Globalization;
using GVXEditor.Properties;
using Serilog;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss2.RawBank;

namespace GVXEditor.Domain;

/// <summary>
/// Zvukova banka instalacie (priecinok RawBank vedla servera, subory FyzBank.xml) - len na citanie pre vyber jazykov
/// a zvukov. Cita sa az pri prvom pouziti; banka, ktora sa neda nacitat, je prazdna.
/// </summary>
/// <param name="path">priecinok banky</param>
internal sealed class SoundBank(string path)
{
    /// <summary>Priecinok banky v priecinku servera.</summary>
    public const string DirectoryName = "RawBank";

    private List<FyzLanguage>? _languages;

    /// <summary>Priecinok banky.</summary>
    public string Path { get; } = path;

    /// <summary>Jazyky banky zoradene podla priecinka; zvuky jazyka nacita <see cref="Sounds" />.</summary>
    public IReadOnlyList<FyzLanguage> Languages => _languages ??= ReadLanguages();

    private List<FyzLanguage> ReadLanguages()
    {
        if (!Directory.Exists(Path))
            return [];
        try
        {
            return FyzBankXmlReader.ReadLanguages(Path);
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or System.Xml.XmlException or FormatException)
        {
            Log.Warning(e, "Zvukovu banku {Path} sa nepodarilo nacitat", Path);
            return [];
        }
    }

    /// <summary>
    /// Zvuk podla cesty radenia: <c>jazyk/skupina/zvuk</c>, alebo <c>skupina/zvuk</c> v prvom jazyku banky;
    /// <see langword="null" />, ak v banke nie je.
    /// </summary>
    public FyzSound? Find(string path)
    {
        ArgumentNullException.ThrowIfNull(path);
        var parts = path.Split('/');
        var (language, group, key) = parts.Length switch
        {
            3 => (parts[0], parts[1], parts[2]),
            2 => (Languages.Count > 0 ? Languages[0].Key : "", parts[0], parts[1]),
            _ => ("", "", "")
        };
        return Sounds(language)?.Groups.FirstOrDefault(item => string.Equals(item.Key, group, StringComparison.OrdinalIgnoreCase))?.Sounds
            .FirstOrDefault(sound => string.Equals(sound.Key, key, StringComparison.OrdinalIgnoreCase));
    }

    /// <summary>Text zvuku radenia pre obsluhu: cesta a prepis nahravky, ak zvuk v banke je.</summary>
    public string Describe(string path) =>
        Find(path) is { Text.Length: > 0 } sound ? string.Format(CultureInfo.CurrentCulture, Resources.Key_Choice, path, sound.Text) : path;

    /// <summary>
    /// Text zvuku kolaje podla kluca: prepis prveho zvuku s tymto klucom v prvom jazyku banky (skupinu urcuje skript hlaseni).
    /// </summary>
    public string DescribeKey(string key)
    {
        ArgumentNullException.ThrowIfNull(key);
        if (key.Length == 0 || Languages.Count == 0 || Sounds(Languages[0].Key) is not { } language)
            return key;
        var sound = language.Groups.SelectMany(group => group.Sounds).FirstOrDefault(item => string.Equals(item.Key, key, StringComparison.OrdinalIgnoreCase));
        return sound is { Text.Length: > 0 } ? string.Format(CultureInfo.CurrentCulture, Resources.Key_Choice, key, sound.Text) : key;
    }

    /// <summary>
    /// Jazyk so skupinami a zvukmi; <see langword="null" />, ak jazyk v banke nie je alebo sa jeho zvuky nedaju nacitat.
    /// </summary>
    /// <param name="key">kluc jazyka, napr. CZ</param>
    public FyzLanguage? Sounds(string key)
    {
        if (Languages.FirstOrDefault(language => string.Equals(language.Key, key, StringComparison.OrdinalIgnoreCase)) is not { } language)
            return null;
        if (language.Groups is { Count: > 0 })
            return language;
        try
        {
            FyzBankXmlReader.ReadSounds(Path, language);
            return language;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or System.Xml.XmlException or FormatException)
        {
            Log.Warning(e, "Zvuky jazyka {Language} banky {Path} sa nepodarilo nacitat", key, Path);
            return null;
        }
    }
}
