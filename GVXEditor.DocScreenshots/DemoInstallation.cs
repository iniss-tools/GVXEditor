using ToolsCore.Iniss.Tools;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Iniss2.Tables;

namespace GVXEditor.DocScreenshots;

/// <summary>
/// Ukážková inštalácia INISS2 pre snímky: grafikon 2030 s fiktívnymi stanicami Lipovec a Horná Lehota na trati Javorina –
/// Dolné Mesto, vlaky so zmenou čísla, variantmi a nočný vlak cez polnoc.
/// </summary>
internal static class DemoInstallation
{
    public const string Group = "2030";
    public const string LipovecDirectory = "01 Lipovec";

    /// <summary>Piatok, v ktorý idú všetky vlaky ukážky.</summary>
    public static readonly DateOnly Day = new(2030, 5, 17);

    private const int Javorina = 9900050;
    private const int Lipovec = 9900100;
    private const int Bukova = 9900150;

    /// <summary>Cislo stanice Bukova ako text (pre okno Pridat stanicu).</summary>
    public const string BukovaNumber = "9900150";
    private const int HornaLehota = 9900200;
    private const int Brezova = 9900250;
    private const int DolneMesto = 9900300;

    private const string Operator = "Fiktívne dráhy";
    private static readonly DateOnly From = new(2029, 12, 9);
    private static readonly DateOnly To = new(2030, 12, 14);

    /// <summary>
    /// Zostaví inštaláciu v <paramref name="root" /> (priečinok vytvorený harnessom sa najprv zmaže).
    /// </summary>
    /// <returns>priečinok inštalácie</returns>
    public static string Build(string root)
    {
        var marker = Path.Combine(root, ".docshots");
        if (Directory.Exists(root))
        {
            if (!File.Exists(marker))
                throw new InvalidOperationException($"Priečinok {root} existuje a nevytvoril ho harness – nezmaže sa.");
            Directory.Delete(root, true);
        }

        Directory.CreateDirectory(root);
        File.WriteAllText(marker, "Ukážková inštalácia INISS2 pre GVXEditor.DocScreenshots – pri ďalšom spustení sa zmaže.");

        var dataDir = Path.Combine(root, "Server", "Data");
        Directory.CreateDirectory(dataDir);
        File.WriteAllText(Path.Combine(dataDir, "Config.xml"), "<?xml version=\"1.0\" encoding=\"windows-1250\"?>\r\n<INISSCFG />\r\n", Encodings.Win1250);

        var groupDir = Path.Combine(dataDir, Group);
        Station(dataDir, LipovecDirectory, Lipovec, ["1", "2", "3"],
        [
            new TrainPosition("611", "3", "3", "jede v 6,7"),
            new TrainPosition("611", "2", "2", ""),
            new TrainPosition("3101", "1", "1", ""),
            new TrainPosition("3102", "2", "2", ""),
            new TrainPosition("3103", "1", "1", ""),
            new TrainPosition("3104", "2", "2", ""),
            new TrainPosition("3105", "1", "1", ""),
            new TrainPosition("3107", "3", "3", ""),
            new TrainPosition("3108", "3", "3", ""),
            new TrainPosition("501", "1", "1", ""),
            new TrainPosition("1741", "2", "2", "")
        ]);
        Station(dataDir, "02 Horná Lehota", HornaLehota, ["1", "2"],
        [
            new TrainPosition("611", "1", "1", ""),
            new TrainPosition("3101", "1", "1", ""),
            new TrainPosition("3102", "2", "2", ""),
            new TrainPosition("3205", "1", "2", ""),
            new TrainPosition("1741", "2", "2", "")
        ]);

        GvStationsFile.Write(groupDir,
        [
            new GvStation(Javorina, "Javorina"), new GvStation(Lipovec, "Lipovec"), new GvStation(Bukova, "Buková"),
            new GvStation(HornaLehota, "Horná Lehota"), new GvStation(Brezova, "Brezová"), new GvStation(DolneMesto, "Dolné Mesto")
        ]);

        new GvFile([Lipovec, HornaLehota], Trains()) { Flags = 1, Language = "C" }.Write(groupDir);

        // zaznamy rychlika 611 v suboroch stanic, vyluka a odklon pre okno Upravit vlak
        var lipovecDir = Path.Combine(groupDir, LipovecDirectory);
        DodatkyFile.Write(lipovecDir,
        [
            new TrainAddition("611", "D1006", "VN", "jede v 6,7"),
            new TrainAddition("611", "4", "", ""),
            new TrainAddition("3101", "D1006", "", ""),
            new TrainAddition("3109", "D1006", "", "")
        ]);
        RazeniFile.Write(lipovecDir, [new TrainComposition("611", 0, "PpLl", "", ["CZ/Slova/razeni", "CZ/Slova/1trida", "CZ/Slova/vpredu"])]);
        ForeignFile.Write(lipovecDir, [new TrainLanguages("611", "", ["GB", "D"])]);
        VylukyFile.Write(groupDir, [new TrainLockout("611", "7", new DateOnly(2030, 5, 1), new DateOnly(2030, 5, 31), "jede v 6,7")]);
        OdklonyFile.Write(groupDir, [new TrainDiversion("611", HornaLehota, "O1", new DateOnly(2030, 6, 1), new DateOnly(2030, 6, 30), "")]);
        ConstReportsFile.Write(dataDir,
        [
            new ConstReport(4, "Bufetový vozeň", "Vozen_so_sluzbou", []),
            new ConstReport(7, "Výluka Lipovec – Horná Lehota", "Vyluka_na_useku", [Lipovec, HornaLehota])
        ]);
        // subory, bez ktorych sa server nespusti (kontrola instalacie ich hlada)
        foreach (var (file, text) in new[] { ("zpozdeni.xml", "<ZPOZDENI />"), ("PigCols.xml", "<PIGCOLS />") })
            File.WriteAllText(Path.Combine(dataDir, file), text, Encodings.Win1250);
        File.WriteAllText(Path.Combine(dataDir, TrTypesFile.FileName),
            """<TRTYPES><t key="Os" /><t key="R" /><t key="Sp" /></TRTYPES>""", Encodings.Win1250);
        foreach (var directory in new[] { LipovecDirectory, "02 Horná Lehota" })
        {
            File.WriteAllText(Path.Combine(groupDir, directory, StateDiagramFile.FileName), StateDiagram, Encodings.Win1250);
            ReportKeysFile.Write(Path.Combine(groupDir, directory),
                [new ReportKey("Přijede", "P", "V P K"), new ReportKey("Vjíždí", "V", "P K"), new ReportKey("Nástup", "N", "V P"), new ReportKey("Odjede", "O", "V P")]);
        }

        StationSetup(groupDir);
        File.WriteAllText(Path.Combine(groupDir, TabTabXml.GlobalFileName), TabTabGlobal, Encodings.Win1250);
        File.WriteAllText(Path.Combine(dataDir, CommLinesXml.FileName), CommLines, Encodings.Win1250);
        // zvukova banka pre vyber zvukov radenia a kolaji (bez nahravok - snimky ich neprehravaju)
        var bankDir = Path.Combine(root, "Server", "RawBank", "CZ");
        Directory.CreateDirectory(bankDir);
        File.WriteAllText(Path.Combine(bankDir, "FyzBank.xml"), SoundBank, new System.Text.UTF8Encoding(true));
        File.WriteAllText(Path.Combine(groupDir, LipovecDirectory, TablesFile.FileName), Tables, Encodings.Win1250);
        File.WriteAllText(Path.Combine(groupDir, LipovecDirectory, TableCatalogFile.FileName), Catalog, Encodings.Win1250);

        File.WriteAllText(Path.Combine(dataDir, LogTextFile.FileName), LogText.ReplaceLineEndings("\r\n"), Encodings.Win1250);
        return root;
    }

    // skript hlaseni: zakladne hlasenia (Odjede s vyberom podla stanice je rucny kod), ostatne a doplnkove hlasenie,
    // dodatky, vzory hlaseni obsluhy, odklon a znelka; funkcie, na ktore odkazuju data instalacie
    private const string LogText = """
        Imports IniSrv

        Public Class LogText
            Inherits LogTextBase

            ' Základné hlásenia vlaku
            <ZakladniHlaseni("Přijede")> _
            Public Sub Přijede()
                If Not Kolaj_dosadena() Then Return
                Zvuk("CZ/Slova/vc", 0, 200)
                DruhVlaku("V4", 5, True)
                CisloVlaku(VlakS.CVlaku)
                Zvuk("CZ/Slova/prijede")
                PoziceKolej("K1", VlakS.PozicePrijSkut)
                If DlouheHlaseni Then
                    Pauza(200)
                    Dodatky()
                End If
            End Sub ' Přijede

            <ZakladniHlaseni("Vjíždí")> _
            Public Sub Vjíždí()
                If Not Kolaj_dosadena() Then Return
                Zvuk("CZ/Slova/vc", 0, 200)
                DruhVlaku("V4", 5, True)
                Zvuk("CZ/Slova/vjizdi")
                PoziceKolej("K1", VlakS.PozicePrijSkut)
            End Sub ' Vjíždí

            <ZakladniHlaseni("Nástup")> _
            Public Sub Nástup()
                Zvuk("CZ/Slova/vc", 0, 200)
                DruhVlaku("V4", 5)
                Zvuk("CZ/Slova/pristaven")
                PoziceNastupiste("N1", VlakS.PoziceOdjSkut)
                PoziceKolej("K1", VlakS.PoziceOdjSkut)
                Pauza(200)
                Dodatky()
            End Sub ' Nástup

            <ZakladniHlaseni("Odjede")> _
            Public Sub Odjede()
                If VlakS.JeKoncici Then Return
                Select Case VlakS.Stanice.iSR70
                    Case 9900100 ' Lipovec: ostrovné nástupište
                        Zvuk("CZ/Vety/bezpec", 0, 200)
                End Select
                DruhVlaku("V4", 5)
                Zvuk("CZ/Slova/odjede")
                PoziceKolej("K1", VlakS.PoziceOdjSkut)
            End Sub ' Odjede

            ' Ostatné a doplnkové hlásenia
            <OstatniHlaseni("Bezpečnostná výzva")> _
            Public Sub Bezpecnostna_vyzva()
                Zvuk("CZ/Vety/bezpec")
                'Zvuk("GB/Vety/bezpec")
            End Sub ' Bezpecnostna_vyzva

            <DoplnkoveHlaseni("Posledná výzva")> _
            Public Sub Posledna_vyzva()
                If VlakS.JeKoncici Then Return
                Zvuk("CZ/Slova/posledni", 0, 200)
                DruhVlaku("V4", 5)
                PoziceKolej("K1", VlakS.PoziceOdjSkut)
            End Sub ' Posledna_vyzva

            ' Dodatky
            <Dodatek("Upozornenie na zmenu radenia")> _
            Public Sub D1006()
                Zvuk("CZ/Dodatky/D1006", 200, 0)
            End Sub ' D1006

            <Dodatek("Jedálny vozeň")> _
            Public Sub D1007()
                Zvuk("CZ/Dodatky/D1007", 200, 0)
            End Sub ' D1007

            ' Vzory hlásení obsluhy
            <VzorDodatku("Vozeň so službou v stanici")> _
            Public Sub Vozen_so_sluzbou(ByVal aiStanice As Integer())
                Zvuk("CZ/Slova/jidelni")
                SeznamStanic(aiStanice, "R1", "R1", "R1")
            End Sub ' Vozen_so_sluzbou

            <VzorVyluky("Výluka na úseku medzi stanicami", TYPVYLUKY.TRATOVA)> _
            Public Sub Vyluka_na_useku(ByVal aiStanice As Integer())
                ZacatekSekce()
                If (aiStanice IsNot Nothing) AndAlso (aiStanice.Length > 1) Then
                    Zvuk("CZ/Slova/vyluka")
                    SeznamStanic(aiStanice, "R1", "R1", "R1")
                End If
                KonecSekce(bRollBack:=True)
            End Sub ' Vyluka_na_useku

            <Odklon("Odklon cez Brezovú")> _
            Public Sub O1()
                Odklon_cez(Nothing)
            End Sub ' O1

            <VzorOdklonu("Odklon cez stanice")> _
            Public Sub Odklon_cez(ByVal aiStanice As Integer())
                Zvuk("CZ/Slova/odklon")
            End Sub ' Odklon_cez

            <InterpreterAttribute("Úvodná znelka")> _
            Public Sub Znělka()
                Zvuk("CZ/Znelky/gong")
            End Sub ' Znělka

            Private Function Kolaj_dosadena() As Boolean
                Return VlakS.PoziceOdjSkut IsNot Nothing
            End Function
        End Class

        """;

    // nastavenie stanice Lipovec pre okno Nastavenie grafikonu: oddelovac a spolocna hodnota v kolajach, vzory stanic
    // v hlaseni, texty tabul podla trasy a stanice, predvolene stanice hlasenia grafikonu
    private static void StationSetup(string groupDir)
    {
        var lipovecDir = Path.Combine(groupDir, LipovecDirectory);
        PositionsDefFile.Write(lipovecDir,
        [
            .. PositionsDefFile.Read(lipovecDir),
            new TrackDefinition { Flags = TrackFlags.Separator },
            new TrackDefinition { Key = "NEJ", Name = "Nejde", TrackText = "-", PlatformText = "-", Flags = TrackFlags.Common | TrackFlags.NoAudio }
        ]);
        ReportPatternsFile.Write(lipovecDir, ReportPatternsFile.ShortFileName,
        [
            [new ReportPatternStop(Lipovec, false), new ReportPatternStop(Brezova, false), new ReportPatternStop(HornaLehota, true),
                new ReportPatternStop(DolneMesto, true)],
            [new ReportPatternStop(Lipovec, false), new ReportPatternStop(Bukova, false), new ReportPatternStop(HornaLehota, true),
                new ReportPatternStop(DolneMesto, true)]
        ]);
        ReportPatternsFile.Write(lipovecDir, ReportPatternsFile.LongFileName,
        [
            [new ReportPatternStop(Lipovec, false), new ReportPatternStop(Brezova, true), new ReportPatternStop(HornaLehota, true),
                new ReportPatternStop(DolneMesto, true)]
        ]);
        RouteTextsFile.Write(lipovecDir,
        [
            new RouteTextSet("TrasaDoSmeru")
            {
                Targets = [new TextTarget("Odchody", "Smer"), new TextTarget("Nástupište", "Smer")],
                Routes =
                [
                    new RouteText("Brezová#Horná Lehota", TrainTextSet.NoFont)
                    {
                        Stations = [new RouteStation(Lipovec, false), new RouteStation(Brezova, true), new RouteStation(HornaLehota, true),
                            new RouteStation(DolneMesto, false)]
                    },
                    new RouteText("Horná Lehota", 2)
                    {
                        Stations = [new RouteStation(Lipovec, false), new RouteStation(Bukova, false), new RouteStation(HornaLehota, true),
                            new RouteStation(DolneMesto, false)]
                    }
                ]
            }
        ]);
        StationTextsFile.Write(lipovecDir,
        [
            new StationTextSet("CieloveStanice")
            {
                Targets = [new TextTarget("Odchody", "Cieľ")],
                Stations = [new StationText(DolneMesto, "Dolné Mesto", TrainTextSet.NoFont), new StationText(Javorina, "Javorina", TrainTextSet.NoFont)]
            }
        ]);
        DefaultStationsFile.Write(groupDir, [new DefaultStation(HornaLehota, "Horná Lehota"), new DefaultStation(Brezova, "Brezová") { Short = false }]);
    }

    // stavovy diagram stanic: vychodzi a prechadzajuci vlak, pomocne stavy a automatiky hlaseni
    private const string StateDiagram = """
        <?xml version="1.0" encoding="windows-1250"?>
        <SD indg=".IndCat3">
          <g ds="#Výchozí vlak">
            <n k="Kolej" f="AUXSTATE">
              <b t="Koľaj" p="0" />
            </n>
            <n k="#Start" f="STOJI JENAODJEZDOVE JEZOBRAZENAPOZICE JEZOBRAZENAKOLEJ" />
            <n k="Nástup" r="Nástup" f="STOJI POTVRZENAKOLEJ NYNISTOJI JENAODJEZDOVE JENASMEROVYCH JEZOBRAZENAPOZICE JEZOBRAZENAKOLEJ">
              <c>.StaniceJeRizena() AndAlso .PlanDobaPobytu &lt; 30</c>
              <b t="Nástup" p="6" />
              <a t=".DatumCasOdj(True).AddSeconds(-600)" m="1" h="2" f="NEVYPINATBEZKLIENTA" />
            </n>
            <n k="Odjede" r="Odjede" f="STOJI POTVRZENAKOLEJ NYNISTOJI JENAODJEZDOVE JENASMEROVYCH JEZOBRAZENAPOZICE JEZOBRAZENAKOLEJ BLIKA">
              <b t="Odjede" p="7" />
              <a t=".DatumCasOdj(True).AddSeconds(-60)" m="0" h="1" />
            </n>
            <n k="Smaž" f="ODBAVEN SHADOW">
              <b t="Zmaž" p="9" />
              <a t=".DatumCasOdj(True).AddSeconds(3600)" m="2" />
            </n>
          </g>
          <g ds="#Projíždějící vlak">
            <n k="#Start" f="JENAODJEZDOVE JENAPRIJEZDOVE JEZOBRAZENAPOZICE JEZOBRAZENAKOLEJ" />
            <n k="Projede" r="Odjede" f="POTVRZENAKOLEJ JENAODJEZDOVE JENAPRIJEZDOVE">
              <b t="Prejde" p="3" />
              <a t=".DatumCasPrij(True).AddSeconds(-180)" m="1" h="1" />
            </n>
          </g>
        </SD>
        """;

    // zvukova banka: slova radenia a zvuky kolaji a nastupist
    private const string SoundBank = """
        <?xml version="1.0" encoding="UTF-8"?>
        <FYZBANK>
          <lang default="true" k="CZ" n="Čeština" d="CZ">
            <g k="K1" d="K1\">
              <z k="1" t="na první kolej" f="1.EWA" l="900" />
              <z k="2" t="na druhou kolej" f="2.EWA" l="900" />
              <z k="3" t="na třetí kolej" f="3.EWA" l="900" />
            </g>
            <g k="N1" d="N1\">
              <z k="I" t="k prvnímu nástupišti" f="I.EWA" l="1100" />
              <z k="II" t="k druhému nástupišti" f="II.EWA" l="1100" />
            </g>
            <g k="Slova" d="Slova\">
              <z k="razeni" t="řazení vlaku" f="razeni.EWA" l="700" />
              <z k="1trida" t="vozy první třídy" f="1trida.EWA" l="900" />
              <z k="jidelni" t="jídelní vůz" f="jidelni.EWA" l="800" />
              <z k="vpredu" t="jsou na začátku vlaku" f="vpredu.EWA" l="1000" />
              <z k="vzadu" t="jsou na konci vlaku" f="vzadu.EWA" l="1000" />
              <z k="vc" t="Vážení cestující," f="vc.EWA" l="900" />
              <z k="prijede" t="přijede" f="prijede.EWA" l="500" />
              <z k="vjizdi" t="vjíždí" f="vjizdi.EWA" l="500" />
              <z k="pristaven" t="je přistaven" f="pristaven.EWA" l="600" />
              <z k="odjede" t="odjede" f="odjede.EWA" l="500" />
              <z k="posledni" t="poslední výzva k nástupu do vlaku" f="posledni.EWA" l="1400" />
              <z k="vyluka" t="z důvodu výluky je zavedena náhradní doprava mezi stanicemi" f="vyluka.EWA" l="2600" />
              <z k="odklon" t="vlak je veden odklonem" f="odklon.EWA" l="1200" />
            </g>
            <g k="Vety" d="Vety\">
              <z k="bezpec" t="Dbejte zvýšené opatrnosti, nevstupujte do kolejiště." f="bezpec.EWA" l="2800" />
            </g>
            <g k="Dodatky" d="Dodatky\">
              <z k="D1006" t="Upozorňujeme cestující na změnu řazení vlaku." f="D1006.EWA" l="2200" />
              <z k="D1007" t="Ve vlaku je řazen jídelní vůz." f="D1007.EWA" l="1700" />
            </g>
            <g k="V4" d="V4\">
              <z k="NOs" t="osobní vlak" f="NOs.EWA" l="700" />
              <z k="NR" t="rychlík" f="NR.EWA" l="600" />
              <z k="ZOs" t="zpožděný osobní vlak" f="ZOs.EWA" l="1000" />
              <z k="ZR" t="zpožděný rychlík" f="ZR.EWA" l="900" />
            </g>
            <g k="R1" d="R1\">
              <z k="9900100" t="Lipovec" f="9900100.EWA" l="600" />
              <z k="9900200" t="Horná Lehota" f="9900200.EWA" l="800" />
              <z k="9900250" t="Brezová" f="9900250.EWA" l="600" />
            </g>
            <g k="Znelky" d="Znelky\">
              <z k="gong" t="(znělka)" f="gong.EWA" l="1500" />
            </g>
          </lang>
        </FYZBANK>
        """;

    // linka tabul stanice Lipovec
    private const string CommLines = """
        <?xml version="1.0" encoding="windows-1250"?>
        <CommLines>
          <line k="Lipovec_Tabule" typ="TCP_CLIENT" protocol="LCD" line="127.0.0.1:47001" />
        </CommLines>
        """;

    // tabule stanice Lipovec: odchody v hale s informacnym riadkom, nastupistia s podmienkou a kolajami, odchody na webe
    private const string Tables = """
        <?xml version="1.0" encoding="windows-1250"?>
        <TABLES>
          <!-- tabule v hale -->
          <t k="Odchody" n="Odchody v hale" lin="Lipovec_Tabule" addr="1" lighttime0="06:00" lighttime1="20:00" light0="80" light1="40">
            <part n="Odchody" ck="Odchody" t="DEPARTURE" rec="8" />
            <part n="Info" ck="Info" t="INFO" rec="1" />
          </t>
          <!-- nastupistia -->
          <t k="Nástupište 1" n="Nástupište 1" lin="Lipovec_Tabule" addr="2" f="NOSCROLL">
            <part n="Nástupište 1" ck="Nástupište" t="PLATFORM" rec="1" cond=".Druh &lt;&gt; &quot;Sp&quot; AndAlso .PlanDobaPobytu &lt; 30">
              <pos k="1" />
              <pos k="2" pri="1" />
            </part>
          </t>
          <t k="Nástupište 2" n="Nástupište 2" lin="Lipovec_Tabule" addr="3" f="NOSCROLL">
            <part n="Nástupište 2" ck="Nástupište" t="PLATFORM" rec="1">
              <pos k="3" />
            </part>
          </t>
          <t k="Internet" n="Lipovec na webe" addr="-1" save="9900100O.xml">
            <part ck="Internet" t="DEPARTURE" rec="10" />
          </t>
        </TABLES>
        """;

    // katalogove predlohy stanice Lipovec: stlpce v znakoch, smer a cervene pole cez tabulky TabTab
    private const string Catalog = """
        <?xml version="1.0" encoding="windows-1250"?>
        <CATALOG>
          <t k="Odchody" n="Odchodová LCD" cls="LCD1">
            <div k="Čas" n="Čas odchodu" inf="CasOdjezdu" fnt="16">
              <pos c1="0" c2="5" />
            </div>
            <div k="Vlak" n="Druh a číslo" inf="TypCisloVlakuKompr" fnt="16">
              <pos c1="6" c2="14" />
            </div>
            <div k="Cieľ" n="Cieľová stanica" inf="CilNazev" fnt="16">
              <pos c1="15" c2="32" />
            </div>
            <div k="Smer" n="Smer" inf="PoNazev" fnt="16" trn="TRANSLATE" t1="Smer">
              <pos c1="33" c2="52" />
            </div>
            <div k="Koľaj" n="Koľaj" inf="KolejOdjezd" fnt="16" align="RIGHT">
              <pos c1="53" c2="56" />
            </div>
            <div k="Meškanie" n="Meškanie" inf="ZpozdeniOdjezd" fnt="16" align="RIGHT">
              <pos c1="57" c2="60" />
            </div>
            <div k="Rýchlik" n="Červené pole" inf="Free" fnt="16" trn="TABLE" t1="PRIZNAK">
              <pos c1="61" c2="62" />
            </div>
          </t>
          <t k="Info" n="Informačný riadok" cls="LCD1">
            <div k="Text" n="Text" inf="TextLine1" fnt="16">
              <pos c1="0" c2="62" />
            </div>
          </t>
          <t k="Nástupište" n="Nástupištná LCD" cls="LCD1">
            <div k="Vlak" n="Druh a číslo" inf="TypCisloVlakuKompr" fnt="16">
              <pos c1="0" c2="8" />
            </div>
            <div k="Cieľ" n="Cieľová stanica" inf="CilNazevNastupiste" fnt="16">
              <pos c1="9" c2="24" />
            </div>
            <div k="Čas" n="Čas odchodu" inf="CasOdjezdu" fnt="16" align="RIGHT">
              <pos c1="25" c2="30" />
            </div>
            <div k="Smer" n="Smer" inf="PoNazevNastupiste" fnt="16" trn="TRANSLATE" t1="Smer">
              <pos r="1" c1="0" c2="30" />
            </div>
          </t>
          <t k="Internet" n="Odchody na webe" cls="INTERNET">
            <div k="Čas" n="Čas odchodu" inf="CasOdjezdu">
              <pos c1="0" c2="5" />
            </div>
            <div k="Vlak" n="Druh a číslo" inf="TypCisloVlakuKompr">
              <pos c1="6" c2="14" />
            </div>
            <div k="Cieľ" n="Cieľová stanica" inf="CilNazev">
              <pos c1="15" c2="40" />
            </div>
          </t>
        </CATALOG>
        """;

    // spolocne tabulky TabTab grafikonu: text smeru pri vyluke a priznak rychlika
    private const string TabTabGlobal = """
        <?xml version="1.0" encoding="windows-1250"?>
        <TABTAB>
          <t n="Smer" d="Smer jazdy pri odchode">
            <expr>
              If .VylukaOdj Then Return .SpojTextyJazyku("NÁHRADNÁ DOPRAVA", "#ALTERNATE TRANSPORT", "#ERSATZVERKEHR")
              If .JeOdklonen Then Return .SpojTextyJazyku("ODKLON", "#DETOUR", "#UMLEITUNG")
              Return sOrig
            </expr>
          </t>
          <t n="PRIZNAK" viewval="true">
            <expr>
              Select Case .Druh
                Case "R", "Ex"
                  Return "(červené pole)"
                Case Else
                  Return "-"
              End Select
            </expr>
            <k>
              <k s="-" f="-2" />
              <v f="-2" />
            </k>
            <k>
              <k s="(červené pole)" f="-2" />
              <v s="&#127;" f="-2" />
            </k>
          </t>
        </TABTAB>
        """;

    private static void Station(string dataDir, string directory, int number, string[] tracks, TrainPosition[] positions)
    {
        var stationDir = Path.Combine(dataDir, Group, directory);
        Directory.CreateDirectory(stationDir);
        DirListXml.AddStation(dataDir, Group, new DirListItem(directory, number, "", "", DirListItemFlags.None));
        DirListXml.WriteStationNumber(stationDir, number);
        PositionsDefFile.Write(stationDir, tracks.Select(track => new TrackDefinition
        {
            Key = track, Name = track, TrackText = track, PlatformText = "I", TrackSound = track, PlatformSound = "I"
        }));
        PositionsFile.Write(stationDir, positions);
    }

    private static List<GvTrain> Trains() =>
    [
        Train(Variant(Stop(Javorina, null, 340, "3101", "Os"), Stop(Lipovec, 358, 360, interest: true), Stop(Brezova, 368, 369),
            Stop(HornaLehota, 380, 381, interest: true), Stop(DolneMesto, 405, null))),
        Train(Variant(Stop(DolneMesto, null, 370, "3102", "Os"), Stop(HornaLehota, 395, 396, interest: true), Stop(Brezova, 405, 406),
            Stop(Lipovec, 415, 417, interest: true), Stop(Javorina, 432, null)) with { Validity = "jede v X" }),
        Train(Variant(Stop(Javorina, null, 460, "3103", "Os"), Stop(Lipovec, 478, 480, interest: true), Stop(Brezova, 488, 489),
            Stop(HornaLehota, 500, 501, interest: true), Stop(DolneMesto, 525, null))),
        Train(
            Variant(Stop(Javorina, null, 510, "611", "R", "Javor"), Stop(Lipovec, 524, 526, interest: true), Stop(Bukova, 533, 533, passing: true),
                Stop(HornaLehota, 540, 542, interest: true), Stop(DolneMesto, 565, null)) with { Validity = "jede v 6,7" },
            Variant(Stop(Javorina, null, 490, "611", "R", "Javor"), Stop(Lipovec, 504, 506, interest: true), Stop(Bukova, 513, 513, passing: true),
                Stop(HornaLehota, 520, 522, interest: true), Stop(DolneMesto, 545, null))),
        Train(Variant(Stop(Javorina, null, 730, "3105", "Os"), Stop(Lipovec, 748, 750, interest: true), Stop(Brezova, 758, 759),
            Stop(HornaLehota, 770, 775, interest: true, number: "3205", type: "Os"), Stop(DolneMesto, 800, null))),
        Train(Variant(Stop(Javorina, null, 900, "501", "Ex", "Tatra"), Stop(Lipovec, 912, 913, interest: true),
            Stop(HornaLehota, 925, 925, interest: true, passing: true), Stop(DolneMesto, 940, null)) with { Flags = GvVariantFlags.N }),
        Train(Variant(Stop(DolneMesto, null, 970, "3104", "Os"), Stop(HornaLehota, 995, 996, interest: true), Stop(Brezova, 1005, 1006),
            Stop(Lipovec, 1015, 1017, interest: true), Stop(Javorina, 1032, null))),
        Train(Variant(Stop(Javorina, null, 1080, "3107", "Os"), Stop(Lipovec, 1098, null, interest: true))),
        Train(Variant(Stop(Lipovec, null, 1120, "3108", "Os", interest: true), Stop(Javorina, 1138, null))),
        Train(Variant(Stop(DolneMesto, null, 1410, "1741", "Sp", "Hron"), Stop(HornaLehota, 1432, 1434, interest: true),
            Stop(Lipovec, 15, 17, interest: true, day: 1), Stop(Javorina, 35, null, day: 1)) with { Validity = "jede v 5,7" })
    ];

    private static GvTrain Train(params GvVariant[] variants) => new(From, To, variants);

    private static GvVariant Variant(params GvStop[] stops) => new() { Operator = Operator, Stops = stops };

    private static GvStop Stop(int station, int? arrival, int? departure, string number = "", string type = "", string name = "", bool interest = false,
        bool passing = false, int day = 0) => new()
    {
        Station = station,
        Arrival = arrival,
        ArrivalDay = arrival.HasValue ? day : 0,
        Departure = departure,
        DepartureDay = departure.HasValue ? day : 0,
        Flags = (interest ? GvStopFlags.Interest : GvStopFlags.None) | (passing ? GvStopFlags.Passing : GvStopFlags.None),
        Number = number,
        TrainType = type,
        TrainName = name
    };
}
