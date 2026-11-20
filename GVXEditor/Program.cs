using GVXEditor.Config;
using GVXEditor.Mcp;
using GVXEditor.UI.Main;
using ToolsCore;
using ToolsCore.Iniss.Calendar;

namespace GVXEditor;

internal static class Program
{
    /// <summary>
    /// Vstupny bod programu.
    /// </summary>
    [STAThread]
    private static void Main()
    {
        // composition root (skladanie bez kontajnera): nastavenia programu a kontext editora; okna ich dostavaju explicitne
        var context = new EditorContext(AppInit.Initialization<GVXEditorConfig, GVXEditorStyle>());

        // server aj klient INISS2 pisu datumove poznamky po cesky (jazyk servera ostava cestina, nic ho nemeni) - kalendar
        // pise rovnako bez ohladu na jazyk programu; slovenske slova poznamok cita server aj DateLimit tiez
        DateLimit.Loc = DateLimit.Locale.Cz;

        using var mcp = new GvxMcpServer();
        AppInit.Run(() => new FMain(context, mcp));
    }
}
