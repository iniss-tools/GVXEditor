using ToolsCore.Iniss2.Tables;

namespace GVXEditor.Domain.Setup.Drafts;

/// <summary>Udaj predlohy tabule, v ktorom sa texty sady prejavia.</summary>
internal sealed class TargetDraft
{
    /// <summary>Kluc predlohy.</summary>
    public string Catalog { get; set; } = "";

    /// <summary>Kluc udaja predlohy.</summary>
    public string Column { get; set; } = "";

    /// <summary>Koncept udaja.</summary>
    public static TargetDraft Of(TextTarget target)
    {
        ArgumentNullException.ThrowIfNull(target);
        return new TargetDraft { Catalog = target.Catalog, Column = target.Column };
    }

    /// <summary>Udaj suboru.</summary>
    public TextTarget To() => new(Catalog, Column);
}
