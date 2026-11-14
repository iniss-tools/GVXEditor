using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Editing;
using GVXEditor.Properties;
using ToolsCore.Tools;

namespace GVXEditor.UI.Main;

/// <summary>
/// Nove cislo vlaku: premenuje sa v GV.XML, suboroch stanic, vylukach a odklonoch grafikonu.
/// </summary>
internal partial class FRenameNumber : Form
{
    private readonly GroupDocument _group;
    private readonly int _index;

    /// <param name="group">grafikon</param>
    /// <param name="index">poradie vlaku v GV.XML</param>
    public FRenameNumber(GroupDocument group, int index)
    {
        ArgumentNullException.ThrowIfNull(group);
        _group = group;
        _index = index;
        InitializeComponent();
        this.ApplyThemeAndFonts();

        foreach (var number in TrainNumbers.Of(group.Timetable!.Trains[index]))
            cboNumber.Items.Add(number);
        if (cboNumber.Items.Count > 0)
            cboNumber.SelectedIndex = 0;
        Input_Changed(this, EventArgs.Empty);
    }

    /// <summary>Povodne cislo.</summary>
    public string OldNumber => cboNumber.SelectedItem as string ?? "";

    /// <summary>Nove cislo.</summary>
    public string NewNumber => tbNew.Text.Trim();

    /// <summary>Premenovat aj ostatne vlaky s povodnym cislom.</summary>
    public bool AllTrains => cboxAllTrains.Checked;

    private void Input_Changed(object? sender, EventArgs e)
    {
        var others = OldNumber.Length > 0 ? TrainNumbers.OtherTrains(_group.Timetable!, _index, OldNumber).Count : 0;
        cboxAllTrains.Visible = others > 0;
        cboxAllTrains.Text = string.Format(CultureInfo.CurrentCulture, Resources.Rename_AllTrains, others);

        var records = OldNumber.Length > 0 ? TrainNumbers.RecordCount(_group, OldNumber) : 0;
        lInfo.Text = string.Format(CultureInfo.CurrentCulture, Resources.Rename_Info, records) +
                     (others > 0 && !AllTrains ? " " + Resources.Rename_Copy : "");
        bOk.Enabled = OldNumber.Length > 0 && NewNumber.Length > 0 && NewNumber != OldNumber;
    }

    private void bOk_Click(object? sender, EventArgs e)
    {
        // nove cislo, ktore uz ma iny vlak, by spojilo zaznamy stanic oboch vlakov
        if (TrainNumbers.OtherTrains(_group.Timetable!, _index, NewNumber).Count > 0 &&
            Utils.ShowQuestion(string.Format(CultureInfo.CurrentCulture, Resources.Rename_Exists, NewNumber)) != DialogResult.Yes)
            return;

        DialogResult = DialogResult.OK;
    }
}
