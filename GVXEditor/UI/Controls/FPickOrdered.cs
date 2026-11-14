using GVXEditor.Domain.Views;
using GVXEditor.Properties;
using ToolsCore.Tools;

namespace GVXEditor.UI.Controls;

/// <summary>
/// Vyber zoznamu hodnot v poradi (stanice, jazyky): dostupne hodnoty s hladanim vlavo, vybrane v poradi vpravo.
/// </summary>
internal partial class FPickOrdered : Form
{
    private readonly IReadOnlyList<ValueChoice> _available;
    private readonly List<ValueChoice> _selected;

    /// <param name="title">titulok okna</param>
    /// <param name="available">hodnoty na vyber</param>
    /// <param name="selected">vybrane kluce v poradi; kluc mimo <paramref name="available" /> ostane s klucom ako textom</param>
    public FPickOrdered(string title, IReadOnlyList<ValueChoice> available, IEnumerable<string> selected)
    {
        ArgumentNullException.ThrowIfNull(available);
        ArgumentNullException.ThrowIfNull(selected);
        InitializeComponent();
        this.ApplyThemeAndFonts();
        Text = title;
        tbFilter.HintText = Resources.Pick_Filter;
        _available = available;
        _selected = [.. selected.Select(key => available.FirstOrDefault(choice => choice.Key == key) ?? new ValueChoice(key, key))];
        ShowLists();
    }

    /// <summary>Vybrane kluce v poradi.</summary>
    public IReadOnlyList<string> Selected => [.. _selected.Select(choice => choice.Key)];

    /// <summary>
    /// Vyber zoznamu; <see langword="null" />, ak obsluha okno zrusila.
    /// </summary>
    public static IReadOnlyList<string>? Pick(IWin32Window owner, string title, IReadOnlyList<ValueChoice> available, IEnumerable<string> selected)
    {
        using var form = new FPickOrdered(title, available, selected);
        return form.ShowDialog(owner) == DialogResult.OK ? form.Selected : null;
    }

    private void ShowLists(IEnumerable<ValueChoice>? select = null)
    {
        var filter = tbFilter.Text.Trim();
        lbAvailable.BeginUpdate();
        lbAvailable.Items.Clear();
        foreach (var choice in _available.Where(choice => !_selected.Contains(choice) &&
                                                          (filter.Length == 0 || choice.Text.Contains(filter, StringComparison.CurrentCultureIgnoreCase) ||
                                                           choice.Key.Contains(filter, StringComparison.OrdinalIgnoreCase))))
            lbAvailable.Items.Add(choice);
        lbAvailable.EndUpdate();

        lbSelected.BeginUpdate();
        lbSelected.Items.Clear();
        foreach (var choice in _selected)
            lbSelected.Items.Add(choice);
        foreach (var choice in select ?? [])
            lbSelected.SelectedItems.Add(choice);
        lbSelected.EndUpdate();
    }

    private void tbFilter_TextChanged(object? sender, EventArgs e) => ShowLists();

    private void bAdd_Click(object? sender, EventArgs e)
    {
        var added = lbAvailable.SelectedItems.Cast<ValueChoice>().ToList();
        _selected.AddRange(added);
        ShowLists(added);
    }

    private void bRemove_Click(object? sender, EventArgs e)
    {
        foreach (var choice in lbSelected.SelectedItems.Cast<ValueChoice>().ToList())
            _selected.Remove(choice);
        ShowLists();
    }

    private void bUp_Click(object? sender, EventArgs e) => MoveSelected(-1);

    private void bDown_Click(object? sender, EventArgs e) => MoveSelected(1);

    // posun vybranych o jedno miesto; blok na kraji zoznamu ostane
    private void MoveSelected(int by)
    {
        var moved = lbSelected.SelectedItems.Cast<ValueChoice>().ToList();
        var indexes = moved.Select(choice => _selected.IndexOf(choice)).Order().ToList();
        if (indexes.Count == 0 || (by < 0 && indexes[0] == 0) || (by > 0 && indexes[^1] == _selected.Count - 1))
            return;
        foreach (var index in by < 0 ? indexes : Enumerable.Reverse(indexes))
            (_selected[index], _selected[index + by]) = (_selected[index + by], _selected[index]);
        ShowLists(moved);
    }
}
