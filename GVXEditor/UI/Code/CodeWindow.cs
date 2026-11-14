using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Code;
using GVXEditor.Domain.Rules;
using GVXEditor.Domain.Server;
using GVXEditor.Properties;
using ToolsCore.CodeEditor;

namespace GVXEditor.UI.Code;

/// <summary>
/// Spolocne casti okien kodu: tema a dopĺňanie editorov, zoznam zisteni a poloha chyby prekladu v editore.
/// </summary>
internal static class CodeWindow
{
    /// <summary>Parametre vyrazu TabTab (okrem VlakS, ktory maju aj vyrazy statedgm).</summary>
    public static IReadOnlyList<string> TabTabParameters { get; } = ["sOrig", "oDiv", "oWSData", "iTableType"];

    /// <summary>Parametre podmienky zobrazenia casti tabule (funkcia bezi v bloku With oVlak).</summary>
    public static IReadOnlyList<string> BoardParameters { get; } = ["oVlak", "oTable", "oWebData"];

    /// <summary>
    /// Editory kodu VB.NET v teme programu. Mena servera pre dopĺňanie sa nacitaju na pozadi.
    /// </summary>
    /// <param name="context">kontext editora</param>
    /// <param name="owner">okno - mena sa doplnia, ak este nie je zatvorene</param>
    /// <param name="parameters">parametre funkcie, do ktorej server kod vlozi (napr. <see cref="TabTabParameters" />)</param>
    /// <param name="boxes">editory</param>
    public static void Prepare(EditorContext context, Form owner, IReadOnlyList<string> parameters, params CodeEditorBox[] boxes)
    {
        ArgumentNullException.ThrowIfNull(context);
        ArgumentNullException.ThrowIfNull(owner);
        foreach (var box in boxes)
            box.UseVisualBasic(context.UsingStyle, context.Config.Fonts.EditorFont);

        if (context.Workspace is not { } workspace)
            return;
        _ = SetSymbolsAsync(context, workspace, owner, parameters, boxes);
    }

    private static async Task SetSymbolsAsync(EditorContext context, Iniss2Workspace workspace, Form owner, IReadOnlyList<string> parameters,
        CodeEditorBox[] boxes)
    {
        var server = await Task.Run(() => ServerVerifier.ReadSymbols(workspace)).ConfigureAwait(true);
        if (owner.IsDisposed)
            return;

        var symbols = server.Select(symbol => new CodeSymbol(symbol.Name,
                symbol.Kind switch { "member" => CodeSymbolKind.Member, "method" => CodeSymbolKind.MemberMethod, _ => CodeSymbolKind.Global },
                symbol.Signature, ""))
            .Concat(VisualBasicSyntax.Keywords.Split(' ').Select(Keyword))
            .Append(new CodeSymbol("VlakS", CodeSymbolKind.Global, "VlakS As VlakS", ""))
            .Concat(parameters.Select(name => new CodeSymbol(name, CodeSymbolKind.Global, name, "")))
            .ToList();
        foreach (var box in boxes)
            box.SetSymbols(symbols, context.UsingStyle);
    }

    // klucove slovo s velkym zaciatocnym pismenom ako vo VB.NET (AndAlso, Return)
    private static CodeSymbol Keyword(string word)
    {
        var name = word switch
        {
            "andalso" => "AndAlso", "orelse" => "OrElse", "isnot" => "IsNot", "elseif" => "ElseIf", "addressof" => "AddressOf", "gettype" => "GetType",
            "typeof" => "TypeOf", "trycast" => "TryCast", "directcast" => "DirectCast", "ctype" => "CType", "cstr" => "CStr", "cint" => "CInt",
            "cbool" => "CBool", "cdate" => "CDate", "cdbl" => "CDbl", "cbyte" => "CByte", "clng" => "CLng",
            _ => char.ToUpperInvariant(word[0]) + word[1..]
        };
        return new CodeSymbol(name, CodeSymbolKind.Keyword, name, "");
    }

    /// <summary>
    /// Zistenie na riadku vyrazu: <paramref name="line" /> je riadok v kode, ako ho prelozil server (aj s obalom v XML).
    /// </summary>
    /// <param name="box">editor vyrazu</param>
    /// <param name="stored">text vyrazu, ako sa zapisal do suboru</param>
    /// <param name="line">riadok od 1</param>
    /// <param name="message">text pre obsluhu</param>
    /// <param name="isError">chyba; inak upozornenie</param>
    /// <param name="embedded">vyraz vlozeny v XML (riadky obalu sa odpocitaju); <see langword="false" /> = cely subor</param>
    public static CodeIssue Issue(CodeEditorBox box, string stored, int line, string message, bool isError, bool embedded = true)
    {
        ArgumentNullException.ThrowIfNull(box);
        var lines = box.Scintilla.Lines;
        var offset = embedded ? EmbeddedCode.LineOffset(stored) : 0;
        var index = Math.Clamp(line - 1 - offset, 0, Math.Max(0, lines.Count - 1));
        if (lines.Count == 0)
            return new CodeIssue(0, 0, message, isError);
        var text = lines[index].Text.TrimEnd('\r', '\n');
        var indent = text.Length - text.TrimStart().Length;
        return new CodeIssue(lines[index].Position + indent, Math.Max(1, text.Length - indent), message, isError);
    }

    /// <summary>Text chyby prekladu pre zoznam zisteni.</summary>
    public static string Message(CodeErrorPlace place, string where)
    {
        ArgumentNullException.ThrowIfNull(place);
        return string.Format(CultureInfo.CurrentCulture, Resources.Code_Compile, where, place.Error.Number, place.Error.Message);
    }

    /// <summary>Zistenia pravidiel ako dvojice pre zoznam.</summary>
    public static IEnumerable<(bool IsError, string Message)> Of(IEnumerable<SetupProblem> problems) =>
        problems.Select(problem => (problem.IsError, problem.Message));

    /// <summary>
    /// Stav prekladu pod zoznamom zisteni.
    /// </summary>
    public static string Status(CodeCheck check, CodeCheckResult? result)
    {
        ArgumentNullException.ThrowIfNull(check);
        if (!check.IsAvailable)
            return Resources.Code_CheckUnavailable;
        if (check.IsBusy)
            return Resources.Code_Checking;
        if (result is null)
            return "";
        if (result.Failure is { } failure)
            return string.Format(CultureInfo.CurrentCulture, Resources.Code_CheckFailed, failure);
        if (result.Fatal is { } fatal)
            return string.Format(CultureInfo.CurrentCulture, Resources.Code_CheckFatal, result.Stage, fatal);
        return result.Errors.Count == 0 ? Resources.Code_CheckOk : "";
    }
}
