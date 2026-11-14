using ExControls;
using GVXEditor.Domain.Analysis;
using ToolsCore.Icons;

namespace GVXEditor.UI.Controls;

/// <summary>
/// Zoznam zisteni na spodku okien: pred textom ikona zavaznosti (chyba, upozornenie, informacia), chyby su prve.
/// </summary>
internal sealed class ProblemList : ExListBox
{
    private const int IconGap = 4;

    public ProblemList()
    {
        DrawMode = DrawMode.OwnerDrawFixed;
    }

    /// <summary>
    /// Zistenie v zozname.
    /// </summary>
    /// <param name="Severity">zavaznost - urcuje ikonu</param>
    /// <param name="Message">text pre obsluhu</param>
    public sealed record Problem(FindingSeverity Severity, string Message)
    {
        /// <inheritdoc />
        public override string ToString() => Message;
    }

    /// <summary>
    /// Zistenia v zozname: chyby, potom upozornenia.
    /// </summary>
    public void ShowProblems(IEnumerable<(bool IsError, string Message)> problems) =>
        ShowProblems(problems.Select(problem => new Problem(problem.IsError ? FindingSeverity.Error : FindingSeverity.Warning, problem.Message)));

    /// <summary>
    /// Zistenia v zozname zoradene podla zavaznosti (poradie rovnako zavaznych ostava).
    /// </summary>
    public void ShowProblems(IEnumerable<Problem> problems)
    {
        ArgumentNullException.ThrowIfNull(problems);
        BeginUpdate();
        Items.Clear();
        Items.AddRange([.. problems.OrderBy(problem => problem.Severity)]);
        EndUpdate();
    }

    /// <inheritdoc />
    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        UpdateItemHeight();
    }

    /// <inheritdoc />
    protected override void OnFontChanged(EventArgs e)
    {
        base.OnFontChanged(e);
        UpdateItemHeight();
    }

    /// <inheritdoc />
    protected override void OnDpiChangedAfterParent(EventArgs e)
    {
        base.OnDpiChangedAfterParent(e);
        UpdateItemHeight();
    }

    // riadok pre ikonu aj text s malym odstupom
    private void UpdateItemHeight() =>
        ItemHeight = Math.Min(255, Math.Max(Font.Height, LogicalToDeviceUnits(IconImages.Size)) + LogicalToDeviceUnits(2));

    /// <inheritdoc />
    protected override void OnDrawItem(DrawItemEventArgs e)
    {
        e.DrawBackground();
        if (e.Index >= 0 && e.Index < Items.Count)
        {
            var item = Items[e.Index];
            var bounds = e.Bounds;
            var x = bounds.X + LogicalToDeviceUnits(2);
            if (item is Problem problem && IconImages.For(this, IconName(problem.Severity)) is { } icon)
            {
                e.Graphics.DrawImage(icon, x, bounds.Y + (bounds.Height - icon.Height) / 2, icon.Width, icon.Height);
                x += icon.Width + LogicalToDeviceUnits(IconGap);
            }

            var text = new Rectangle(x, bounds.Y, Math.Max(0, bounds.Right - x), bounds.Height);
            TextRenderer.DrawText(e.Graphics, GetItemText(item), e.Font ?? Font, text, e.ForeColor,
                TextFormatFlags.VerticalCenter | TextFormatFlags.Left | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis);
        }

        e.DrawFocusRectangle();
    }

    /// <summary>
    /// Stavova ikona zavaznosti z banky ikon (aj panel Kontrola instalacie).
    /// </summary>
    public static string IconName(FindingSeverity severity) => severity switch
    {
        FindingSeverity.Error => "status-error",
        FindingSeverity.Warning => "status-warning",
        _ => "status-info"
    };
}
