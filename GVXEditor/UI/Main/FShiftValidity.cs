using System.Globalization;
using GVXEditor.Properties;
using ToolsCore.Tools;

namespace GVXEditor.UI.Main;

/// <summary>
/// Pocet dni, o ktore sa posunie obdobie platnosti vsetkych alebo vybranych vlakov grafikonu (novy rok grafikonu: 364 dni
/// = 52 tyzdnov).
/// </summary>
internal partial class FShiftValidity : Form
{
    /// <param name="group">nazov grafikonu</param>
    /// <param name="selected">pocet vybranych vlakov, ktore sa posunu; 0 = vsetky vlaky grafikonu</param>
    public FShiftValidity(string group, int selected = 0)
    {
        InitializeComponent();
        this.ApplyThemeAndFonts();
        lPrompt.Text = selected > 0
            ? string.Format(CultureInfo.CurrentCulture, Resources.Main_ShiftSelectedPrompt, selected)
            : string.Format(CultureInfo.CurrentCulture, Resources.Main_ShiftPrompt, group);
    }

    /// <summary>Pocet dni; zaporny posunie spat.</summary>
    public int Days => (int)nudDays.Value;
}
