namespace GVXEditor.UI.Controls;

/// <summary>
/// Bunka s vyberom v okne: na pravom okraji je tlacidlo s troma bodkami, vyber otvori klik nanho, dvojklik alebo F4
/// a Enter. Bunka na pisanie otvori vyber len tlacidlom a F4. Do suboru ide hodnota, ktoru vrati vyber.
/// </summary>
internal sealed class CellPicker
{
    private const int ButtonWidth = 18;

    private readonly DataGridView _grid;
    private readonly DataGridViewColumn _column;
    private readonly Func<object, string?> _pick;
    private readonly Func<object, string>? _format;
    private readonly bool _editable;

    /// <param name="grid">tabulka</param>
    /// <param name="column">stlpec viazany na textovu vlastnost (bunka je len na citanie)</param>
    /// <param name="pick">vyber pre polozku riadka; <see langword="null" /> = zrusene</param>
    /// <param name="format">text bunky pre polozku riadka; <see langword="null" /> = hodnota tak, ako je</param>
    private CellPicker(DataGridView grid, DataGridViewColumn column, Func<object, string?> pick, Func<object, string>? format, bool editable)
    {
        _grid = grid;
        _column = column;
        _pick = pick;
        _format = format;
        _editable = editable;
        column.ReadOnly = !editable;
        grid.CellPainting += Grid_CellPainting;
        grid.CellMouseClick += Grid_CellMouseClick;
        grid.CellDoubleClick += Grid_CellDoubleClick;
        grid.KeyDown += Grid_KeyDown;
        if (format is not null)
            grid.CellFormatting += Grid_CellFormatting;
    }

    /// <summary>
    /// Pripoji vyber k stlpcu tabulky; bunka je len na citanie a ukazuje text z <paramref name="format" />.
    /// </summary>
    public static void Attach(DataGridView grid, DataGridViewColumn column, Func<object, string?> pick, Func<object, string>? format = null)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(column);
        ArgumentNullException.ThrowIfNull(pick);
        _ = new CellPicker(grid, column, pick, format, false);
    }

    /// <summary>
    /// Pripoji vyber k stlpcu, do ktoreho sa da aj pisat (vyber otvori tlacidlo a F4).
    /// </summary>
    public static void AttachEditable(DataGridView grid, DataGridViewColumn column, Func<object, string?> pick)
    {
        ArgumentNullException.ThrowIfNull(grid);
        ArgumentNullException.ThrowIfNull(column);
        ArgumentNullException.ThrowIfNull(pick);
        _ = new CellPicker(grid, column, pick, null, true);
    }

    private bool IsPickerCell(int row, int column) => row >= 0 && column == _column.Index;

    private void Grid_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
    {
        if (!IsPickerCell(e.RowIndex, e.ColumnIndex) || _grid.Rows[e.RowIndex].DataBoundItem is not { } item)
            return;
        e.Value = _format!(item);
        e.FormattingApplied = true;
    }

    private static Rectangle ButtonBounds(Rectangle cell) => new(cell.Right - ButtonWidth - 2, cell.Top + 2, ButtonWidth, cell.Height - 5);

    private void Grid_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (!IsPickerCell(e.RowIndex, e.ColumnIndex) || e.Graphics is null)
            return;
        e.Paint(e.ClipBounds, DataGridViewPaintParts.All);
        var button = ButtonBounds(e.CellBounds);
        var style = e.CellStyle;
        var back = _grid.Enabled ? _grid.DefaultCellStyle.BackColor : SystemColors.Control;
        using (var fill = new SolidBrush(back))
            e.Graphics.FillRectangle(fill, button);
        ControlPaint.DrawBorder(e.Graphics, button, _grid.GridColor, ButtonBorderStyle.Solid);
        TextRenderer.DrawText(e.Graphics, "…", style?.Font ?? _grid.Font, button, _grid.DefaultCellStyle.ForeColor,
            TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
        e.Handled = true;
    }

    private void Grid_CellMouseClick(object? sender, DataGridViewCellMouseEventArgs e)
    {
        if (!IsPickerCell(e.RowIndex, e.ColumnIndex) || e.Button != MouseButtons.Left)
            return;
        var cell = _grid.GetCellDisplayRectangle(e.ColumnIndex, e.RowIndex, false);
        var button = ButtonBounds(new Rectangle(0, 0, cell.Width, cell.Height));
        if (button.Contains(e.Location))
            Pick(e.RowIndex);
    }

    private void Grid_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (!_editable && IsPickerCell(e.RowIndex, e.ColumnIndex))
            Pick(e.RowIndex);
    }

    private void Grid_KeyDown(object? sender, KeyEventArgs e)
    {
        if (!(e.KeyCode == Keys.F4 || (e.KeyCode == Keys.Enter && !_editable)) || _grid.CurrentCell is not { } cell || !IsPickerCell(cell.RowIndex, cell.ColumnIndex))
            return;
        e.Handled = true;
        Pick(cell.RowIndex);
    }

    private void Pick(int row)
    {
        if (_grid.Rows[row].DataBoundItem is not { } item || _pick(item) is not { } value)
            return;
        _grid.EndEdit();
        var cell = _grid.Rows[row].Cells[_column.Index];
        if (Equals(cell.Value, value))
            return;
        cell.Value = value;
        _grid.InvalidateRow(row);
    }
}
