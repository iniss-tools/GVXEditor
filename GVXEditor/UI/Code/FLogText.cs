using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Properties;
using ScintillaNET;
using ToolsCore.CodeEditor;
using ToolsCore.Iniss.Tools;
using ToolsCore.Tools;

namespace GVXEditor.UI.Code;

/// <summary>
/// Okno Skript hlaseni: subor LogText.vb v editore VB.NET s hladanim. Server ho prelozi cez most chvilu po poslednej zmene
/// a chyby podciarkne na riadku.
/// </summary>
internal partial class FLogText : Form
{
    private readonly Iniss2Workspace _workspace;
    private readonly string _original;
    private readonly CodeCheck _check;
    private readonly List<int> _lines = [];
    private readonly int _line;
    private CodeCheckResult? _result;
    private bool _loading;

    /// <param name="context">kontext editora (otvorena instalacia so skriptom hlaseni)</param>
    /// <param name="text">text skriptu (napr. s neulozenymi zmenami okna Hlasenia); <see langword="null" /> = skript instalacie</param>
    /// <param name="line">riadok od 1, na ktory okno skoci po otvoreni; 0 = zaciatok</param>
    public FLogText(EditorContext context, string? text = null, int line = 0)
    {
        ArgumentNullException.ThrowIfNull(context);
        _workspace = context.Workspace ?? throw new InvalidOperationException("Instalacia nie je otvorena.");
        _original = text ?? _workspace.History.Current.LogText ?? throw new InvalidOperationException("Instalacia nema LogText.vb.");
        _line = line;
        InitializeComponent();
        this.ApplyThemeAndFonts();
        _check = new CodeCheck(_workspace, this, () => _workspace.History.Current with { LogText = Result() }, ShowResult);
        Disposed += (_, _) => _check.Dispose();
        CodeWindow.Prepare(context, this, [], codeText);
        codeText.Scintilla.EolMode = _original.Contains("\r\n", StringComparison.Ordinal) || !_original.Contains('\n', StringComparison.Ordinal) ? Eol.CrLf : Eol.Lf;
        _loading = true;
        codeText.Code = _original;
        _loading = false;
        Recheck();
        _check.Schedule();
    }

    /// <summary>
    /// Text suboru z okna; bez zmeny vrati povodny text (ten isty objekt).
    /// </summary>
    public string Result()
    {
        var text = codeText.Code;
        return text == _original ? _original : text;
    }

    /// <inheritdoc />
    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        if (_line <= 0 || codeText.Scintilla.Lines.Count == 0)
            return;
        var line = codeText.Scintilla.Lines[Math.Min(_line - 1, codeText.Scintilla.Lines.Count - 1)];
        line.Goto();
        codeText.Scintilla.FirstVisibleLine = Math.Max(0, line.Index - 3);
        codeText.Scintilla.Focus();
    }

    /// <inheritdoc />
    protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
    {
        switch (keyData)
        {
            case Keys.Control | Keys.F:
                codeText.ShowFindReplace(this, false);
                return true;
            case Keys.Control | Keys.H:
                codeText.ShowFindReplace(this, true);
                return true;
            default:
                return base.ProcessCmdKey(ref msg, keyData);
        }
    }

    private void tsbFind_Click(object? sender, EventArgs e) => codeText.ShowFindReplace(this, false);

    private void tsbReplace_Click(object? sender, EventArgs e) => codeText.ShowFindReplace(this, true);

    private void Text_Changed(object? sender, EventArgs e)
    {
        if (_loading)
            return;
        Recheck();
        _check.Schedule();
        lCheck.Text = CodeWindow.Status(_check, _result);
    }

    // znaky, ktore CP1250 nema - server subor cita v CP1250 a zapis by ich zmenil na otazniky
    private string? Unencodable()
    {
        var text = codeText.Code;
        var bad = text.Where(c => c > 127 && Encodings.Win1250.GetString(Encodings.Win1250.GetBytes(c.ToString())) != c.ToString()).Distinct().Take(5).ToList();
        return bad.Count == 0 ? null : string.Format(CultureInfo.CurrentCulture, Resources.Code_Unencodable, string.Concat(bad));
    }

    private void Recheck()
    {
        var problems = new List<(bool IsError, string Message, int Line)>();
        if (Unencodable() is { } unencodable)
            problems.Add((true, unencodable, 0));
        foreach (var place in _result?.Errors.Where(place => place.IsLogText) ?? [])
        {
            var where = string.Format(CultureInfo.CurrentCulture, Resources.Loc_Line, Resources.Page_LogText, place.Line);
            problems.Add((!place.IsWarning, CodeWindow.Message(place, where), place.Line));
        }

        var ordered = problems.OrderBy(problem => problem.IsError ? 0 : 1).ToList();
        _lines.Clear();
        _lines.AddRange(ordered.Select(problem => problem.Line));
        lbProblems.ShowProblems(ordered.Select(problem => (problem.IsError, problem.Message)));
        bOk.Enabled = Unencodable() is null;
    }

    private void ShowResult(CodeCheckResult result)
    {
        _result = result;
        lCheck.Text = CodeWindow.Status(_check, result);
        Recheck();
        codeText.SetIssues(result.Errors.Where(place => place.IsLogText)
            .Select(place => CodeWindow.Issue(codeText, "", place.Line, place.Error.Number + " " + place.Error.Message, !place.IsWarning, embedded: false)));
    }

    private void lbProblems_DoubleClick(object? sender, EventArgs e)
    {
        if (lbProblems.SelectedIndex < 0 || lbProblems.SelectedIndex >= _lines.Count || _lines[lbProblems.SelectedIndex] <= 0)
            return;
        var line = codeText.Scintilla.Lines[Math.Min(_lines[lbProblems.SelectedIndex] - 1, codeText.Scintilla.Lines.Count - 1)];
        line.Goto();
        codeText.Scintilla.Focus();
    }

    private void bOk_Click(object? sender, EventArgs e)
    {
        if (Unencodable() is { } unencodable)
        {
            Utils.ShowWarning(unencodable);
            return;
        }

        if ((_result?.Errors.Any(place => place.IsLogText && !place.IsWarning) ?? false) && Utils.ShowQuestion(Resources.Code_ConfirmCompile) != DialogResult.Yes)
            return;

        DialogResult = DialogResult.OK;
    }
}
