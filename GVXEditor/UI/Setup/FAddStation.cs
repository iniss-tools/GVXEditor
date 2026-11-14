using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Setup;
using GVXEditor.Properties;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Tools;

namespace GVXEditor.UI.Setup;

/// <summary>
/// Okno Pridat stanicu: cislo, nazov, priecinok a druh stanice a stanica, z ktorej sa skopiruje nastavenie. Zapis robi volajuci.
/// </summary>
internal partial class FAddStation : Form
{
    private readonly Iniss2Workspace _workspace;
    private readonly int _group;
    private bool _updating;
    private bool _nameEdited;
    private bool _directoryEdited;

    /// <summary>Druh stanice v ponuke.</summary>
    private sealed record KindChoice(DirListItemFlags Flags, string Label)
    {
        public override string ToString() => Label;
    }

    /// <summary>Stanica v ponuke predloh.</summary>
    private sealed record TemplateChoice(int Group, int Station, string Label)
    {
        public override string ToString() => Label;
    }

    /// <param name="workspace">otvorena instalacia</param>
    /// <param name="group">poradie grafikonu, do ktoreho stanica pribudne</param>
    /// <param name="templateStation">poradie stanice grafikonu, ktora sa ponukne ako predloha; -1 = prva</param>
    public FAddStation(Iniss2Workspace workspace, int group, int templateStation)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        _workspace = workspace;
        _group = group;
        InitializeComponent();
        this.ApplyThemeAndFonts();
        Text = string.Format(CultureInfo.CurrentCulture, Resources.AddStation_Title, workspace.Groups[group].Group.Name);

        cboKind.DataSource = new List<KindChoice>
        {
            new(DirListItemFlags.None, Resources.Kind_Station),
            new(DirListItemFlags.Stop, Resources.Kind_Stop),
            new(DirListItemFlags.Junction, Resources.Kind_Junction),
            new(DirListItemFlags.Stop | DirListItemFlags.Junction, Resources.Kind_StopJunction)
        };

        var templates = workspace.Groups.SelectMany((document, g) => document.Stations.Select((station, s) =>
            new TemplateChoice(g, s, string.Create(CultureInfo.CurrentCulture, $"{document.Group.Name} / {station.Station.Directory}")))).ToList();
        cboTemplate.DataSource = templates;
        cboTemplate.SelectedItem = templates.Find(choice => choice.Group == group && choice.Station == (templateStation >= 0 ? templateStation : 0)) ??
                                   templates.FirstOrDefault();
        CheckInput(false);
    }

    /// <summary>Nova stanica podla okna.</summary>
    public NewStation Station => new(_group, tbDirectory.Text.Trim(), SetupText.Station(tbNumber.Text) ?? 0, tbName.Text.Trim(),
        (cboKind.SelectedItem as KindChoice)?.Flags ?? DirListItemFlags.None,
        (cboTemplate.SelectedItem as TemplateChoice)?.Group ?? -1, (cboTemplate.SelectedItem as TemplateChoice)?.Station ?? -1);

    private void Input_Changed(object? sender, EventArgs e)
    {
        if (_updating)
            return;

        if (sender == tbName)
            _nameEdited = tbName.Text.Length > 0;
        else if (sender == tbDirectory)
            _directoryEdited = tbDirectory.Text.Length > 0;

        // nazov podla cisla zo staniciach instalacie a priecinok podla nazvu, kym ich obsluha neprepise
        _updating = true;
        if (sender == tbNumber && !_nameEdited)
            tbName.Text = SetupText.Station(tbNumber.Text) is { } number && _workspace.StationNames.TryGetValue(number, out var name) ? name : "";
        if ((sender == tbNumber || sender == tbName) && !_directoryEdited)
            tbDirectory.Text = tbName.Text.Trim().Length > 0 ? StructureEdits.SuggestDirectory(_workspace.Groups[_group].Stations.Count + 1, tbName.Text) : "";
        _updating = false;

        CheckInput(true);
    }

    private bool CheckInput(bool show)
    {
        var problems = StructureEdits.Check(_workspace, Station);
        lProblem.Text = show ? string.Join(Environment.NewLine, problems.Take(2)) : "";
        bOk.Enabled = problems.Count == 0;
        return problems.Count == 0;
    }

    private void bOk_Click(object? sender, EventArgs e)
    {
        if (CheckInput(true))
            DialogResult = DialogResult.OK;
    }
}
