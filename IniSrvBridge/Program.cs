using System;
using System.IO;
using System.Text;

namespace IniSrvBridge;

/// <summary>
/// Overenie dat instalacie INISS2 kodom servera: nacita IniSrv.exe instalacie a zopakuje nacitanie a preklad zo startu
/// servera nad kopiou dat. Siet, vlakna, zariadenia ani zvuk nespusta a do instalacie nic nezapisuje.
/// </summary>
internal static class Program
{
    private const string Usage = """
        IniSrvBridge - overenie dat INISS2 kodom servera

          IniSrvBridge.exe --server <priecinok s IniSrv.exe> --root <priecinok s kopiou Data> [--rawbank <priecinok>] [--code]
          IniSrvBridge.exe --server <priecinok s IniSrv.exe> --symbols

        --server   priecinok servera instalacie, z neho sa nacita IniSrv.exe (len cita)
        --root     docasny priecinok so Data - tu server cita data, zapisuje logy a preklad
        --rawbank  banka zvukov (predvolene <server>\RawBank, len cita)
        --code     len preklad kodu (statedgm.xml, LogText.vb, TabTab, podmienky Tables.xml): chyba prekladu nezastavi
                   dalsi preklad, kroky za Tables.xml sa vynechaju
        --symbols  mena typu VlakS a typov servera pre dopĺňanie v editore kodu (JSON)

        Vysledok je JSON na standardnom vystupe. Navratove kody: 0 = server by sa spustil, 1 = chyba, 2 = zle argumenty.
        """;

    private static int Main(string[] args)
    {
        Console.OutputEncoding = new UTF8Encoding(false);

        string? server = null, root = null, rawBank = null;
        bool code = false, symbols = false;
        for (var i = 0; i < args.Length; i++)
        {
            var value = i + 1 < args.Length ? args[i + 1] : null;
            switch (args[i])
            {
                case "--server" when value is not null:
                    server = value;
                    i++;
                    break;
                case "--root" when value is not null:
                    root = value;
                    i++;
                    break;
                case "--rawbank" when value is not null:
                    rawBank = value;
                    i++;
                    break;
                case "--code":
                    code = true;
                    break;
                case "--symbols":
                    symbols = true;
                    break;
                default:
                    Console.Error.WriteLine(Usage);
                    return 2;
            }
        }

        if (server is null || !File.Exists(Path.Combine(server, ServerLoad.ExecutableName)))
        {
            Console.Error.WriteLine(Usage);
            return 2;
        }

        if (symbols)
        {
            Console.Out.Write(ServerSymbols.Read(Path.GetFullPath(server)));
            return 0;
        }

        if (root is null || !Directory.Exists(root))
        {
            Console.Error.WriteLine(Usage);
            return 2;
        }

        var result = new ServerLoad(Path.GetFullPath(server), Path.GetFullPath(root), Path.GetFullPath(rawBank ?? Path.Combine(server, "RawBank")), code).Run();
        Console.Out.Write(result.ToJson());
        return result.Ok ? 0 : 1;
    }
}
