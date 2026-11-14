using GVXEditor.Config;
using GVXEditor.Domain;
using ToolsCore;

namespace GVXEditor;

/// <summary>
/// Stav editora: <see cref="Session" /> (cely beh programu) a <see cref="Workspace" /> (otvorena instalacia INISS2).
/// Vytvara ho <see cref="Program" /> a okna ho dostavaju explicitne - v kode nie je staticky pristup k datam.
/// </summary>
internal sealed class EditorContext(AppSession<GVXEditorConfig, GVXEditorStyle> session)
{
    /// <summary>
    /// Nastavenia programu (konfiguracia a styly).
    /// </summary>
    public AppSession<GVXEditorConfig, GVXEditorStyle> Session { get; } = session;

    /// <summary>
    /// Konfiguracia programu.
    /// </summary>
    public GVXEditorConfig Config => Session.Config;

    /// <summary>
    /// Pouzivany styl.
    /// </summary>
    public GVXEditorStyle UsingStyle => Session.UsingStyle;

    /// <summary>
    /// Otvorena instalacia; <see langword="null" /> = ziadna.
    /// </summary>
    public Iniss2Workspace? Workspace { get; private set; }

    /// <summary>
    /// Otvori instalaciu - vsetky jej data sa vymenia naraz.
    /// </summary>
    public void Open(Iniss2Workspace workspace) => Workspace = workspace;
}
