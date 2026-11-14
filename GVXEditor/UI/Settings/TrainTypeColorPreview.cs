using GVXEditor.Config;
using ToolsCore.XML;

namespace GVXEditor.UI.Settings;

/// <summary>
/// Nahlad farieb druhov vlakov na stranke farebnej schemy: riadky zoznamu vlakov s cislom a druhom. Klik na riadok vyberie
/// skupinu druhu.
/// </summary>
internal sealed class TrainTypeColorPreview : Control, IColorPreview
{
    // cislo a druh vlaku ako v zozname vlakov (data INISS2, nie text rozhrania)
    private static readonly (string Number, string Type, string Id)[] Rows =
        [("3101", "Os", "Os"), ("611", "R", "R"), ("171", "EC", "X"), ("99", "Sl", "Sl")];

    private readonly List<(Rectangle Bounds, string Id)> _areas = [];
    private GVXEditorStyle? _style;
    private Font? _bold;

    public TrainTypeColorPreview()
    {
        DoubleBuffered = true;
        ResizeRedraw = true;
    }

    /// <inheritdoc />
    public Control Control => this;

    /// <inheritdoc />
    public event EventHandler<string>? KeyClicked;

    /// <inheritdoc />
    public void ShowScheme(Style style)
    {
        _style = style as GVXEditorStyle;
        Invalidate();
    }

    /// <inheritdoc />
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        _areas.Clear();
        if (_style is null)
            return;

        var box = _style.ControlsColorScheme.Box;
        var g = e.Graphics;
        using (var back = new SolidBrush(box.BackColor))
            g.FillRectangle(back, ClientRectangle);

        var row = Font.Height + LogicalToDeviceUnits(8);
        var numberWidth = LogicalToDeviceUnits(70);
        var typeWidth = LogicalToDeviceUnits(60);
        var scheme = _style.TrainTypeScheme;
        ColorSetting[] settings = [scheme.Os, scheme.R, scheme.X, scheme.Sl];
        using var grid = new Pen(_style.ControlsColorScheme.Border.ForeColor);
        const TextFormatFlags flags = TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPrefix;
        var padding = LogicalToDeviceUnits(4);
        for (var i = 0; i < Rows.Length; i++)
        {
            var bounds = new Rectangle(0, i * row, Width, row);
            var type = new Rectangle(numberWidth, bounds.Y, typeWidth, row);
            var setting = settings[i];
            if (setting.BackColor.A > 0)
            {
                using var back = new SolidBrush(setting.BackColor);
                g.FillRectangle(back, type);
            }

            var font = setting.Bold ? _bold ??= new Font(Font, FontStyle.Bold) : Font;
            TextRenderer.DrawText(g, Rows[i].Number, Font, new Rectangle(padding, bounds.Y, numberWidth - padding, row), box.ForeColor, flags);
            TextRenderer.DrawText(g, Rows[i].Type, font, type with { X = type.X + padding, Width = type.Width - padding }, setting.ForeColor, flags);
            g.DrawLine(grid, 0, bounds.Bottom - 1, Width, bounds.Bottom - 1);
            _areas.Add((bounds, "TrainType." + Rows[i].Id));
        }

        g.DrawLine(grid, numberWidth, 0, numberWidth, Rows.Length * row);
        g.DrawLine(grid, numberWidth + typeWidth, 0, numberWidth + typeWidth, Rows.Length * row);
    }

    /// <inheritdoc />
    protected override void OnFontChanged(EventArgs e)
    {
        _bold?.Dispose();
        _bold = null;
        base.OnFontChanged(e);
    }

    /// <inheritdoc />
    protected override void OnMouseClick(MouseEventArgs e)
    {
        base.OnMouseClick(e);
        var area = _areas.FirstOrDefault(a => a.Bounds.Contains(e.Location));
        if (area.Id is not null)
            KeyClicked?.Invoke(this, area.Id);
    }

    /// <inheritdoc />
    protected override void Dispose(bool disposing)
    {
        if (disposing)
            _bold?.Dispose();
        base.Dispose(disposing);
    }
}
