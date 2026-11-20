# GVXEditor – pokyny pre agentov

Editor dát servera INISS2 (IniSrv, .NET): grafikon `GV.XML` a súbory staníc. GV odkazuje na grafikon a GVDEditor, X na
`GV.XML`. GVDEditor ostáva editorom INISS 3.39 a jeho export do INISS2 je len jednorazová migrácia. Repozitár je súčasťou
pracovného priečinka `INISSTools` (nie je git repozitár, každý podpriečinok má vlastné repo); vedľa musia byť naklonované
`ExControls` a `ToolsCore`. Komunikácia, texty UI aj komentáre v kóde (bez diakritiky) sú slovenské.

Pravidlá desktopových programov (WinForms, texty v `Resources`, ručné `*.Designer.cs`, príkazy `CommandSet`, téma, dialógy,
ikony SVG) a logovania (Serilog) sú v `../ToolsCore/AGENTS.md` – platia aj tu. Súbory INISS2 číta a zapisuje len
`ToolsCore.Iniss2` (zápis úpravou na mieste, pohľad servera), špecifikácia je v `iniss-tools-docs/docs/iniss2/`.

## Štruktúra a zásady

- Projekty: `GVXEditor`, `GVXEditor.Tests`, `GVXEditor.DocScreenshots` (snímky do `iniss-tools-docs/static/img/gvxeditor`,
  ukážková inštalácia `DemoInstallation` v `C:\INISS2` so značkou `.docshots`) a `IniSrvBridge` (net48 x86, zostaví sa
  a skopíruje vedľa GVXEditor.exe cieľmi v `GVXEditor.csproj`, nie je referencia).
- Overenie kódom servera (`Domain/Server/ServerVerifier`): kópia `Data` s neuloženými zmenami (`Iniss2Workspace.WritePending`)
  v `%TEMP%\GVXEditor\verify-*\Server`, most načíta `IniSrv.exe` inštalácie reflexiou a zopakuje kroky
  `modGlob.MainProc` bez registrácie, mutexu, PPV, zálohy, AGV, siete, vlákien, svetiel a inicializácie zvuku a liniek;
  výsledok JSON (chyba kroku, chyby prekladu s riadkom a miestom, záznamy Error.log). Cesty servera presmeruje:
  `Application.startupPath` (LogText.dll) a `executablePath` (odkaz prekladu na IniSrv.exe), `Log.msLogPath`/`msTmpLogPath`,
  polia `modGlob` (`constsDataDirFullPathName`…), aktuálny priečinok (dočasné súbory prekladu). Do inštalácie nikdy
  nezapisuje; beží pri príkaze Overiť kódom servera a pred uložením. Server hádže `ApplicationException` bez textu
  a príčinu píše do Error.log – most vtedy vezme posledný záznam logu.
- Namespace = priečinok: `Domain/` (`Iniss2Workspace` – otvorená inštalácia, grafikony z DirList.xml, GV.XML tak, ako je v
  súbore, názvy staníc ako server; `Views/` – riadky okien; `Editing/` – koncept vlaku `TrainDraft`, zmeny
  `TimetableEditing`, číslo vlaku naprieč súbormi `TrainNumbers`; `Code/` – kód, ktorý server prekladá: stanica
  `StationCode` (statedgm.xml, TabTab.xml), koncepty `StateDiagramEdit`/`TabTabEdit` s `Drafts/`, kód vložený v XML
  `EmbeddedCode` (zobrazenie bez obalu, zápis s pôvodným odsadením), miesto chyby prekladu `CodeErrorPlace` (z popisu
  servera: grafikon:stanica, graf, uzol, funkcia; TabTab názov tabuľky); `Stations/` – súbory staníc `StationFiles`, kľúče vlaku
  v staniciach `TrainStations`, úpravy `StationEdit`/`NoticesEdit` s konceptmi `Drafts/`; `Setup/` – nastavenie, ktoré
  server len číta: stanice `StationSetup` (PositionsDef, ReportKeys, StaHlasB/C, TTTrasy, TTStanice), grafikon `GroupSetup`
  (GV.stanice, Default.Stations), koncepty okien `GroupSetupEdit`/`StationSetupEdit`/`ConstReportsEdit` s `Drafts/`,
  zmeny štruktúry `StructureEdits`; `Boards/` – tabule stanice `StationBoards` (Tables.xml, TCatalog.xml), koncept okna
  `BoardsEdit` s `Drafts/`, hodnoty výčtov servera `BoardValues`, okolie pre pravidlá `BoardContext` (linky, koľaje,
  TabTab, písma ELEN, prvý grafikon stanice); `Rules/` – kontrola vlaku `TrainRules`, záznamov `RecordRules`,
  nastavenia `SetupRules`, kódu `CodeRules` a tabúľ `BoardRules`; `Analysis/` – kontrola inštalácie
  `InstallationChecks`), `Config/`, `UI/{Main,EditTrain,Setup,Code,Boards,Analysis,Settings}`; `EditorContext` a `Program`
  v koreni (composition root bez statického prístupu k dátam).
- Stav inštalácie je nemenný `WorkspaceState` (zoznam `GroupDocument`: `GvFile`, súbory staníc, nastavenie, kód a tabule staníc,
  nastavenie grafikonu, spoločné TabTab, výluky, odklony; hlásenia obsluhy `ConstReports` a text `LogText` pre celú
  inštaláciu; funkcie LogText sa počítajú z aktuálneho textu), každá zmena je nový stav
  v `UndoHistory` (`Iniss2Workspace.Change`, `ChangeConstReports`); názvy staníc (`StationNames`) sa počítajú
  z aktuálneho stavu; neuložené = iný odkaz na zoznam ako pri uložení
  (`GroupDocument.Changes` → `FileWrite`), preto úprava bez zmeny musí vrátiť ten istý objekt. Strom hlavného okna drží
  poradie grafikonu, nie `GroupDocument`. `Save` zapíše len zmenené súbory v `FileTransaction`, pri bežiacom serveri nič,
  súbor zmenený po načítaní (`FileStamp` podľa cesty) len so súhlasom a nový súbor len s obsahom; pred uložením beží
  `InstallationChecks` a chyba sa uloží len po potvrdení.
- Nový grafikon a nová stanica (`StructureEdits`) menia DirList.xml a priečinky, preto sa zapíšu hneď, nie sú krokom
  histórie: len bez neuložených zmien a pri zastavenom serveri, zápis vo `FileTransaction` (nový priečinok sa pri chybe
  zmaže), potom sa inštalácia načíta znova. Stanica dostane kópiu priečinka predlohy bez záznamov vlakov (sady
  `TTVlaky.xml` ostanú bez textov – `TrainTextsFile.RemoveTrains`), číslo v SR70.TXT a názov v GV.stanice.XML.
- Kód (okná `UI/Code`): editory `ToolsCore.CodeEditor`, mená na dopĺňanie z `IniSrv.exe` (most `--symbols`, pamätajú sa
  podľa programu servera), preklad počas písania `CodeCheck` – 1,5 s po zmene `ServerVerifier.RunCode` (stav s konceptom
  okna do kópie dát, most `--code`: chyba prekladu nezastaví ďalší preklad, podmienky `cond` z Tables.xml sa preložia bez
  kontroly konfigurácie tabúľ, ďalšie kroky sa vynechajú, ~2 s). Chyba podmienky tabule nemá od servera miesto, len kód
  s doplneným `return` – okno Tabule ju priradí každej časti s rovnakým kódom (`CodeErrorPlace.MatchesCondition`). Varovania
  kompilátora (BC4xxxx) server hlási len spolu s chybou – v editore sú upozornenie. Hlásenie stavu (`r`) server volá ako
  metódu LogText, ReportKeys dáva len písmeno. Pozor: udalosť zmeny editora môže prísť už pri nastavení textu
  v konštruktore – polia, ktoré obsluha udalosti používa, nastaviť pred tým.
- Okno Hlásenia (`UI/Code/FReports`, Grafikon → Hlásenia…): funkcie skriptu ako údaje nad `ToolsCore.Iniss2` `Grafikon/Script`.
  Kód je jediný zdroj pravdy (žiadne značky ani vedľajší súbor): funkcia je upraviteľná, len keď ju model opíše a zapíše
  späť rovnako, inak je ručná a otvorí sa v okne Skript hlásení na riadku (`FLogText` s textom a riadkom). `Domain/Code/
  ReportsEdit` drží text a po každej platnej úprave konceptu (`Drafts/ReportDraft`, `StepDraft`; susedné kroky jedného
  jazyka = jeden blok) hneď prepíše len rozsah tej funkcie; koncept s chybou `Rules/ReportRules` sa nezapíše (pri prepnutí
  a OK upozornenie). Odkazy z dát `ReportReferences` (ReportKeys, statedgm, ConstReports, dodatky, výluky, odklony, bez
  ohľadu na veľkosť písmen), náhľad textu z banky `ReportPreview` (jazyk, ktorý banka nemá, sa nevyhlási; vstavaná položka
  a volanie v lomených zátvorkách). Krok vstavanej položky má v `StepDraft.Kind` názov metódy a vo `Value` argumenty (nová
  dostane povinné argumenty `StepDraft.Template`), volanie má celý zápis `Funkcia(…)`; kontrola overí argumenty, skupiny
  zvukov v banke a funkcie (volanie, `AddressOf`). Hlásenia obsluhy ponúkajú vzory dodatku, odklonu aj výluky. Podmienky:
  `StepDraft.Condition` je reťaz výrazov `A » Else B` (vnorené If, vetva Else), susedné kroky s rovnakým jazykom
  a podmienkou sú jeden blok; stráže `ReportDraft.Guards` (riadok = `If výraz Then Return`) a sekcia; katalóg častých
  podmienok `ReportTexts.Conditions` a logické funkcie skriptu sú len pomoc pri zadávaní (ponuka v paneli krokov a pri
  stráži), podmienka môže byť ľubovoľný výraz – overí ho preklad kódom servera. Premenovanie hlásenia, na ktoré odkazujú
  dáta: okno pri OK ponúkne úpravu odkazov (`FReports.Renames`) a hlavné okno uloží skript aj `ReportRenames.Apply`
  (ReportKeys, statedgm, ConstReports, dodatky, výluky, odklony; AGV.xml nie) ako jeden krok `Iniss2Workspace.ChangeState`.
  Nový dodatok z okna vlaku (záložka Dodatky): `FReports` s textom a druhom na založenie (meno `D<n>` za najvyšším číslom),
  okno vlaku drží skript v `LogText` a hlavné okno ho uloží s grafikonom v jednom kroku. Kontrola inštalácie hlási typy
  hlásení, stavy a hlásenia obsluhy s funkciou, ktorú skript nemá, a základné hlásenie bez spúšťača (okrem mien, ktoré
  klient hľadá sám).
- Kontroly nastavenia podľa servera (dekompilát IniSrv): chyba = server sa nespustí alebo záznam nenájde (duplicitné
  číslo v Default.Stations, v GV.stanice prvého grafikonu zlyhá hľadanie názvu, kľúč koľaje, názov typu hlásenia,
  číslo hlásenia obsluhy, neznáma kategória), upozornenie = server záznam nepoužije (z písmena berie prvý znak,
  druhý rovnaký vzor alebo text stanice, prázdna predloha/údaj sady).
- Tabule (okno `UI/Boards/FBoards`, pravidlá `BoardRules` podľa `Table.Init`, `Catalog.Init` a `TableFontBanks` servera):
  server číta Tables.xml a TCatalog.xml len z prvého výskytu stanice v DirList, linku `lin` len pri tabuli s adresou
  a zariadením a len bez `tl` stanice v DirList. TableFontBanks.xml server prečíta, jeho banky však nepoužije (hľadá len
  binárne banky ELEN), preto ho program neupravuje; `f` pri ELEKON je chyba. Kontrola inštalácie pravidlá tabúľ
  nespúšťa (ako pri nastavení a kóde), chráni ich okno.
- Záznamy súborov staníc patria vlaku podľa čísla pri odchode zo stanice (`TrainStations.Keys` ako `TrainRanges` servera:
  kľúč `Z` a čas), výluky podľa čísla z prvého zastavenia v stanici grafikonu. `RecordLists.Replace` dáva nové záznamy
  na miesta pôvodných a text záznamov nemení (ani medzery) – korpus overil, že okno bez zmeny nič nezapíše.
- Koncept vlaku dopočíta kľúč `Z` zo staníc DirList a dni cez polnoc z časov ako klient INISS2 (`frmEditVlak`) – vlak
  bez `Z` v stanici grafikonu ho po úprave dostane; okno potvrdené bez zmeny nie je zmena (`TimetableEditing.SameTrain`).
  Upravený vlak ostáva na mieste, nový ide na koniec.
- Server drží dáta v pamäti a pri úprave vlaku z klienta ich prepíše. Program smie zapisovať len pri zastavenom serveri
  inštalácie (`Iniss2Server.IsRunning(serverDir)` – spustený program servera v priečinku inštalácie; mutex `INISRV_MUTEX`
  platí len v relácii a neurčí inštaláciu). AGV.xml je stav servera za behu, program ho nemení.
- Zapisuje sa vždy obsah súboru (`GvFile.Load`), nikdy pohľad servera (`GvFile.Read`/`ForServer` – stanice mimo DirList bez
  `Z`, rozdelené varianty MOs). Pohľad Stanica na deň je `StationTimetable.On` – ako server: platí prvá varianta, ktorá v deň
  ide, koľaj prvý vyhovujúci záznam.
- Dátumové poznámky: server ich číta po česky aj po slovensky (po českom slove skúsi začiatok slova, rovnaké vzory má
  `DateLimit`), píše ich však vždy po česky – jazyk servera `DateRem.Loc` nič nenastavuje; kalendár preto píše po česky
  (`DateLimit.Loc = Cz` v `Program`) bez ohľadu na jazyk UI. Sviatky vždy české.
- Hlavné okno má ukotviteľné panely ExControls `ExDockManager` (`UI/Main/FMain.Docking.cs`, spoločné časti
  `ToolsCore.Tools.AppDock`): návrh okna má strom `tvData` a dokument `pDocument` vedľa seba, za behu idú do panela
  Inštalácia a do `Document`; panel Kontrola inštalácie je `UI/Analysis/AnalysisView` (filtre závažnosti, kontrola po
  zmene s oneskorením cez `UpdateDirtyState`, skrytý panel až po zobrazení, výsledok overenia kódom servera platí do
  ďalšej zmeny). Kľúče panelov `Installation`/`Analysis` sú v uloženom rozložení (`ConfigBase.DockLayout`, ukladá sa
  pri zatvorení okna) – nemeniť; panely sa pridávajú pred `ApplyThemeAndFonts`, rozloženie sa obnoví až po ňom.
- Príkazy hlavného okna cez katalóg `UI/GvxCommands` (ID = názov prvku skratky v config.xml, nemeniť; zoznam stráži
  `GvxCommandsTests`). Úplnosť prekladu SK/CZ stráži `LocalizationTests`.
- Veľké okná úprav (Hlásenia, Skript hlásení, Stavový diagram, Tabuľky TabTab, Tabule, Upraviť vlak, Nastavenie
  grafikonu, Hlásenia obsluhy) nemajú `CancelButton` – Escape ich nezatvorí, rozpísané úpravy zahodí len tlačidlo Zrušiť.
  Výberové okná a jednoduché dialógy (Premenovať, Posunúť platnosť, Nový grafikon, Pridať stanicu, Generovať texty) Escape
  zatvára.
- Okná vznikajú generátorom `tools/formgen.py` (strom prvkov → Designer.cs, `.resx`, `.cs.resx`; `components` pre
  `ContextMenuStrip`, stĺpec `combo`): `tools/gen_fmain.py`, `gen_fedittrain.py`, `gen_fshift.py`, `gen_frename.py`,
  `gen_analysisview.py` (panel Kontrola inštalácie, UserControl), `gen_fsetup.py` (Nastavenie grafikonu, pomocné funkcie
  zoznamov používa aj `gen_fconstreports.py`),
  `gen_fstructure.py` (Nový grafikon, Pridať stanicu), `gen_fcode.py` (Stavový diagram, Tabuľky TabTab, Skript hlásení), `gen_freports.py` (Hlásenia, pomocné funkcie z `gen_fcode.py`),
  `gen_fboards.py` (Tabule, pomocné funkcie berie z `gen_fcode.py`).
  Kým sa Designer.cs neupraví ručne,
  zmena patrí do generátora a okno sa vygeneruje znova (pred tým porovnať výstup so súborom). Texty `Resources` generuje
  `tools/gen_resources.py`.
- Zoznam vlakov hlavného okna (`UI/Main/FMain.Trains.cs`): riadok `TrainRow` drží poradie vlaku v GV.XML (`Index`) –
  príkazy a výber pracujú s ním, nikdy s číslom riadka; triedenie `TrainRowOrder` (tretí klik na hlavičku vráti poradie
  súboru, rovnaké hodnoty v poradí súboru), výber viacerých vlakov (Kopírovať, Zmazať, Posunúť platnosť ako jeden krok
  histórie; Upraviť a Premenovať pre vlak pod kurzorom). Farby stĺpca Druh: skupiny `TrainTypeGroups` podľa zoznamov
  klienta INISS2 (predvolené štýly druhov), druh mimo nich podľa `sound` z TrTypes.xml; schéma `GvxColors`.
- Generovanie textov tabúľ (`Domain/Stations/Texts`, okno `UI/Boards/FGenerateTexts`, generátor `tools/gen_fgeneratetexts.py`):
  cieľová/východzia stanica a stanice do/zo smeru do sady TTVlaky.xml; stanice smeru = stanice hlásenia ako server
  (`ReportStationSelection` podľa `StaHlasVl.GenerujStanici`: StaHlasVl → vzor StaHlasB/C nad trasou bez `P` → Default.Stations,
  zo smeru bez vzoru len východzia). Text pri vlaku má u servera prednosť pred TTTrasy/TTStanice – zapíše sa len tam, kde by
  server ukázal niečo iné, doplnenie neprekryje text podľa trasy alebo stanice a prázdny text sa nezapisuje.
- Zoznam zistení na spodku okien je `UI/Controls/ProblemList` (ikona závažnosti pred textom, chyby prvé so stabilným
  poradím – okná podľa indexu riadka skáču na miesto zistenia); plní sa cez `ShowProblems`.
- Server MCP pre agentov `Mcp/` beží v procese editora (spoločný `ToolsCore.Mcp` `McpServerHost`): `GvxMcpServer` dodá pokyny
  pre agenta a `ApplyAsync` podľa `Config/GvxMcpSettings` (port 5491, kľúč `gvx_`; stránka Agenti (MCP) v `FAppSettings`, štart
  v `FMain_Load` a znova po zmene nastavenia). Nástroje sú statické metódy `Mcp/<Oblast>Tools.cs` nad `EditorAccess` (čítanie na
  vlákne okna, bez okna priamo – testy), parametre agenta overuje `McpGuards` (grafikon podľa názvu, stanica podľa čísla, priečinka
  alebo názvu, vlak podľa poradia v GV.XML; chyby `McpException("kod: popis")` s radou), výsledky sú záznamy `Mcp/Results/`
  s popismi pre agenta anglicky. Súbor dát s neuloženými zmenami dá `Iniss2Workspace.CurrentText` (zápis do dočasnej kópie
  súboru tými istými zápismi ako uloženie), zoznam `UnsavedFiles`; overenie kódom servera beží mimo vlákna okna. Náhľad
  generovania textov je spoločný s oknom (`TrainTextGenerating.Preview`, zmena `GeneratedTextChange`). Zmeny (len s povolením
  zmien – nástroje s `ReadOnly = false` sa inak neregistrujú) idú cez `EditorAccess.WriteAsync`: z aktuálneho nemenného stavu
  vypočíta nový (stav bez zmeny = ten istý objekt), jedno volanie = jeden krok späť „… (agent)“ (`History_Agent`), nič sa
  neukladá, `dryRun` vráti rovnaký výsledok bez zmeny. Hlavné okno je `IAgentHost` (`FMain.Agent.cs`): pri otvorenom okne,
  dialógu alebo overovaní (`!CanFocus`, `UseWaitCursor`) odmietne zmenu kódom `editor.busy` – rozpísaná úprava v okne by ju inak
  prepísala. Nástroje zmien používajú pravidlá okien: vlak `TrainRules` (chyba odmietne, upozornenia vráti; `set_train` berie
  varianty v tvare `get_train` – príznaky ako kľúče súboru, `Z` a dni cez polnoc dopočíta `TrainDraft.ToTrain`), hlásenia obsluhy
  `SetupRules` (zmena nesmie pridať chybu, staršiu znesie), premenovanie `TrainNumbers.Rename`, texty `TrainTextGenerating.Apply`.
  Záznamy vlaku v staniciach, kód, tabule a nastavenie agent len číta. Testy `GVXEditor.Tests/Mcp` – každý nástroj cez klienta
  voči `outputSchema` (nový nástroj tam pridať) a snímka `Mcp/mcp-tools.json`.
- Okná testovať bez zobrazenia na STA vlákne (`EditTrainFormTests`: `CreateControl` + `OnLoad` reflexiou).

## Príkazy

```bash
dotnet build GVXEditor.slnx -nologo -v q
dotnet test GVXEditor.slnx
dotnet run --project GVXEditor.DocScreenshots -- --theme=both   # snimky do docs; --only=hlavne-okno
GVXEDITOR_UPDATE_MCP=1 dotnet test GVXEditor.Tests --filter Nastroje_ZodpovedajuSnimke   # nova snimka nastrojov MCP
"/c/Program Files/Inno Setup 7/ISCC.exe" GVXEditorInstaller.iss   # instalator do Output/; najprv Release build
```

Vydanie: značka `vX.Y` spustí `.github/workflows/release.yml` (spoločný postup v `ToolsCore/Installer/README.md`).

## Spoločné pravidlá

- `Directory.Build.props`: `TreatWarningsAsErrors`, `Nullable`, `ImplicitUsings`, `AnalysisLevel latest-recommended` – build
  bez jediného varovania; zámerne vypnuté pravidlá len tam (`NoWarn` s dôvodom), varovanie opraviť v kóde, výnimočné
  potlačenie priamo na nájdenom člene (`[SuppressMessage(..., Justification = ...)]`).
- Jeden typ najvyššej úrovne na súbor (vnorené typy môžu ostať), viac súvisiacich súborov do vlastného priečinka.
- Kultúra: UI `CultureInfo.CurrentCulture`, formáty a logy `InvariantCulture`.
- GUI nespúšťať cez Bash – overuje ho používateľ ručne; bez klikania harness (`GVXEditor.DocScreenshots`, reflexia
  + `DrawToBitmap`).
- Testy MSTest, metódy `Oblast_CoOveruje`; do repa len testy s vlastnými dátami (`TestInstallation`). Korpusové overenie nad
  `D:\INISSroot` a porovnanie s kódom servera len na stroji používateľa (scratchpad); do `D:\INISSroot` nikdy nezapisovať,
  zapisovateľná kópia je `D:\INISS2Test`.
- Skripty: generátory ukladať ako súbor (bash heredoc mení `\\` a `\n`); v `.ps1` žiadna diakritika.

## Commity (Conventional Commits)

Commitovať len na požiadanie. Správy podľa [Conventional Commits](https://www.conventionalcommits.org/): `<typ>(<oblasť>): <popis>`.

- Typ: `feat`, `fix`, `refactor`, `perf`, `docs`, `test`, `build` (projekty, balíky, inštalátor), `ci` (workflow), `style`,
  `chore`, `revert`.
- Oblasť nepovinná, malými písmenami (napr. `main`, `station-day`, `settings`, `docshots`).
- Popis anglicky v rozkazovacom spôsobe, malým písmenom, bez bodky, najviac 72 znakov; názvy tried v backtickoch. Telo po
  prázdnom riadku vysvetľuje prečo; nekompatibilná zmena (formát súborov, config.xml) má `!` a pätičku `BREAKING CHANGE: …`.
- Príklad: ``fix(main): show variants of the selected train in `FMain` ``.
