using System.ComponentModel;
using System.Globalization;
using GVXEditor.Domain.Views;

namespace GVXEditor.UI.Controls;

/// <summary>
/// Vyber stanice nazvom v tabulke: stlpec nazvu je vyber viazany na tu istu vlastnost ako stlpec cisla. Cislo, ktore
/// v zozname nie je (nova stanica, preklep), sa do zoznamu doplni, aby ho tabulka vedela ukazat.
/// </summary>
internal sealed class StationChoices
{
    private readonly BindingList<ValueChoice> _choices = [];

    /// <summary>Stanice na vyber: kluc je cislo ako text, text nazov (pri rovnakych nazvoch s cislom).</summary>
    public IReadOnlyList<ValueChoice> Items => _choices;

    /// <summary>
    /// Zoznam stanic podla nazvu a hodnoty, ktore su v datach.
    /// </summary>
    /// <param name="names">nazvy stanic podla cisla</param>
    /// <param name="present">cisla stanic v datach ako text</param>
    public static StationChoices Of(IReadOnlyDictionary<int, string> names, IEnumerable<string> present)
    {
        var result = new StationChoices();
        result.Fill(names, present);
        return result;
    }

    /// <summary>
    /// Naplni zoznam znova (napr. po zmene nazvov stanic v okne); stlpce viazane cez <see cref="Bind" /> ho ukazu hned.
    /// </summary>
    public void Fill(IReadOnlyDictionary<int, string> names, IEnumerable<string> present)
    {
        ArgumentNullException.ThrowIfNull(names);
        ArgumentNullException.ThrowIfNull(present);
        var duplicates = names.Values.GroupBy(name => name, StringComparer.CurrentCultureIgnoreCase).Where(group => group.Count() > 1)
            .Select(group => group.Key).ToHashSet(StringComparer.CurrentCultureIgnoreCase);
        _choices.RaiseListChangedEvents = false;
        _choices.Clear();
        _choices.Add(new ValueChoice("", ""));
        foreach (var (number, name) in names.OrderBy(pair => pair.Value, StringComparer.CurrentCulture).ThenBy(pair => pair.Key))
            _choices.Add(new ValueChoice(Key(number), duplicates.Contains(name) ? $"{name} ({Key(number)})" : name));
        foreach (var value in present)
            Ensure(value);
        _choices.RaiseListChangedEvents = true;
        _choices.ResetBindings();
    }

    private static string Key(int number) => number.ToString(CultureInfo.InvariantCulture);

    /// <summary>Doplni hodnotu, ktora v zozname nie je (cislo bez nazvu alebo neplatny text) - ukaze sa tak, ako je.</summary>
    public void Ensure(string? value)
    {
        var key = value ?? "";
        if (!_choices.Any(choice => choice.Key == key))
            _choices.Add(new ValueChoice(key, key));
    }

    /// <summary>Text stanice: nazov, inak cislo.</summary>
    public string Text(string key) => _choices.FirstOrDefault(choice => choice.Key == key)?.Text ?? key;

    /// <summary>
    /// Stlpce nazvu v tabulke ako vyber zo zoznamu; po zmene ktorejkolvek bunky sa nova hodnota doplni do zoznamu.
    /// </summary>
    /// <param name="grid">tabulka</param>
    /// <param name="columns">stlpce nazvu viazane na vlastnost s cislom stanice</param>
    public void Bind(DataGridView grid, params DataGridViewComboBoxColumn[] columns)
    {
        ArgumentNullException.ThrowIfNull(grid);
        foreach (var column in columns)
        {
            column.DataSource = null;
            column.ValueMember = nameof(ValueChoice.Key);
            column.DisplayMember = nameof(ValueChoice.Text);
            column.DataSource = _choices;
        }

        grid.CellValueChanged += (_, e) =>
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0 || !Array.Exists(columns, column => column.DataPropertyName == grid.Columns[e.ColumnIndex].DataPropertyName))
                return;
            Ensure(grid.Rows[e.RowIndex].Cells[e.ColumnIndex].Value as string);
            grid.InvalidateRow(e.RowIndex);
        };
        // hodnota, ktora sa este nestihla doplnit, nesmie zhodit tabulku
        grid.DataError += (_, e) => e.ThrowException = false;
    }
}
