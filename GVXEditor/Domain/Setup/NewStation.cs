using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Setup;

/// <summary>
/// Nova stanica grafikonu: priecinok, cislo a druh v DirList.xml, nastavenie skopirovane z predlohy.
/// </summary>
/// <param name="Group">poradie grafikonu, do ktoreho stanica pribudne</param>
/// <param name="Directory">priecinok stanice v priecinku grafikonu</param>
/// <param name="Station">cislo stanice (SR70.TXT)</param>
/// <param name="Name">nazov stanice - pribudne do GV.stanice.XML grafikonu, ak tam cislo este nie je</param>
/// <param name="Flags">druh stanice</param>
/// <param name="TemplateGroup">poradie grafikonu predlohy</param>
/// <param name="TemplateStation">poradie stanice predlohy v jej grafikone</param>
internal sealed record NewStation(int Group, string Directory, int Station, string Name, DirListItemFlags Flags, int TemplateGroup, int TemplateStation);
