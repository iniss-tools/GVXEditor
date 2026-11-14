using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Setup;
using ToolsCore.Tools;

namespace GVXEditor.UI.Setup;

/// <summary>
/// Okno Novy grafikon: nazov priecinka, predloha a ci sa skopiruju aj vlaky. Zalozenie robi volajuci.
/// </summary>
internal partial class FNewGroup : Form
{
    private readonly Iniss2Workspace _workspace;

    /// <summary>Grafikon v ponuke predloh.</summary>
    /// <param name="Index">poradie grafikonu</param>
    /// <param name="Name">nazov priecinka</param>
    private sealed record GroupChoice(int Index, string Name)
    {
        public override string ToString() => Name;
    }

    /// <param name="workspace">otvorena instalacia</param>
    /// <param name="template">poradie grafikonu, ktory sa ponukne ako predloha; -1 = posledny</param>
    public FNewGroup(Iniss2Workspace workspace, int template)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        _workspace = workspace;
        InitializeComponent();
        this.ApplyThemeAndFonts();

        // predloha musi byt nacitana - z GV.XML sa berie zoznam stanic grafikonu
        var choices = workspace.Groups.Select((group, index) => (group, index)).Where(pair => pair.group.Timetable is not null)
            .Select(pair => new GroupChoice(pair.index, pair.group.Group.Name)).ToList();
        cboTemplate.DataSource = choices;
        var selected = choices.FindLast(choice => choice.Index == template) ?? choices.LastOrDefault();
        if (selected is not null)
        {
            cboTemplate.SelectedItem = selected;
            tbName.Text = Suggest(selected.Name);
        }

        CheckInput(false);
    }

    /// <summary>Nazov priecinka noveho grafikonu.</summary>
    public string GroupName => tbName.Text.Trim();

    /// <summary>Poradie grafikonu predlohy.</summary>
    public int Template => (cboTemplate.SelectedItem as GroupChoice)?.Index ?? -1;

    /// <summary>Skopirovat aj vlaky, vyluky, odklony a zaznamy vlakov.</summary>
    public bool CopyTrains => cboxCopyTrains.Checked;

    // grafikony sa spravidla volaju podla roku - navrhne sa nasledujuci volny
    private string Suggest(string template)
    {
        if (!int.TryParse(template, NumberStyles.None, CultureInfo.InvariantCulture, out var year))
            return "";
        for (var next = year + 1; next < year + 100; next++)
        {
            var name = next.ToString(CultureInfo.InvariantCulture);
            if (StructureEdits.CheckGroupName(_workspace, name) is null)
                return name;
        }

        return "";
    }

    private void Input_Changed(object? sender, EventArgs e) => CheckInput(true);

    private bool CheckInput(bool show)
    {
        var problem = Template < 0 ? Properties.Resources.Structure_NoTemplate : StructureEdits.CheckGroupName(_workspace, GroupName);
        lProblem.Text = show || tbName.Text.Length > 0 ? problem ?? "" : "";
        bOk.Enabled = problem is null;
        return problem is null;
    }

    private void bOk_Click(object? sender, EventArgs e)
    {
        if (CheckInput(true))
            DialogResult = DialogResult.OK;
    }
}
