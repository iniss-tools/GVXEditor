using GVXEditor.Domain.Views;
using ToolsCore.Tools;

namespace GVXEditor.UI.Controls;

/// <summary>
/// Vyber viacerych hodnot zaskrtnutim (pismena typov hlasenia, kategorie vlakov).
/// </summary>
internal partial class FPickChecked : Form
{
    /// <param name="title">titulok okna</param>
    /// <param name="hint">co znamena prazdny vyber a podobne</param>
    /// <param name="items">hodnoty na vyber</param>
    /// <param name="selected">zaskrtnute kluce; kluc mimo <paramref name="items" /> sa prida s klucom ako textom</param>
    public FPickChecked(string title, string hint, IReadOnlyList<ValueChoice> items, IEnumerable<string> selected)
    {
        ArgumentNullException.ThrowIfNull(items);
        ArgumentNullException.ThrowIfNull(selected);
        InitializeComponent();
        this.ApplyThemeAndFonts();
        Text = title;
        lHint.Text = hint;
        var keys = selected.ToHashSet(StringComparer.Ordinal);
        foreach (var item in items.Concat(keys.Where(key => items.All(item => item.Key != key)).Select(key => new ValueChoice(key, key))))
            clbItems.Items.Add(item, keys.Contains(item.Key));
    }

    /// <summary>Zaskrtnute kluce v poradi zoznamu.</summary>
    public IReadOnlyList<string> Selected => [.. clbItems.CheckedItems.Cast<ValueChoice>().Select(item => item.Key)];

    /// <summary>
    /// Vyber hodnot; <see langword="null" />, ak obsluha okno zrusila.
    /// </summary>
    public static IReadOnlyList<string>? Pick(IWin32Window owner, string title, string hint, IReadOnlyList<ValueChoice> items, IEnumerable<string> selected)
    {
        using var form = new FPickChecked(title, hint, items, selected);
        return form.ShowDialog(owner) == DialogResult.OK ? form.Selected : null;
    }
}
