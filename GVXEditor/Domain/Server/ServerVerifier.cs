using System.Diagnostics;
using System.Text;
using System.Text.Json;
using Serilog;

namespace GVXEditor.Domain.Server;

/// <summary>
/// Overenie kodom servera: kopia dat s neulozenymi zmenami sa nacita mostom IniSrvBridge (x86, .NET Framework), ktory
/// zavola citace a preklad z IniSrv.exe instalacie. Do instalacie sa nic nezapisuje.
/// </summary>
internal static class ServerVerifier
{
    /// <summary>Most vedla programu.</summary>
    public const string BridgeName = "IniSrvBridge.exe";

    /// <summary>Program servera v priecinku instalacie.</summary>
    public const string ServerExecutable = "IniSrv.exe";

    private static readonly TimeSpan Timeout = TimeSpan.FromMinutes(2);

    /// <summary>Cesta k mostu; <see langword="null" />, ak vedla programu chyba.</summary>
    public static string? BridgePath =>
        Path.Combine(AppContext.BaseDirectory, BridgeName) is var path && File.Exists(path) ? path : null;

    /// <summary>
    /// Overenie je mozne: most je pri programe a instalacia ma program servera.
    /// </summary>
    public static bool IsAvailable(Iniss2Workspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        return BridgePath is not null && File.Exists(Path.Combine(workspace.ServerDir, ServerExecutable));
    }

    /// <summary>
    /// Nacita aktualny stav instalacie (s neulozenymi zmenami) kodom servera.
    /// </summary>
    /// <exception cref="ServerVerificationException">most sa nedal spustit alebo nevratil vysledok</exception>
    public static ServerVerification Run(Iniss2Workspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        Log.Information("Overenie instalacie {DataDir} kodom servera", workspace.DataDir);
        return Run(workspace, workspace.History.Current, false);
    }

    /// <summary>
    /// Prelozi kod stavu <paramref name="state" /> (statedgm.xml, LogText.vb, TabTab) kodom servera. Chyba prekladu nezastavi
    /// dalsi preklad, ostatne subory sa za TabTab necitaju. Pre podciarkovanie chyb pocas pisania.
    /// </summary>
    /// <param name="workspace">otvorena instalacia</param>
    /// <param name="state">stav s rozpisanou upravou okna</param>
    /// <exception cref="ServerVerificationException">most sa nedal spustit alebo nevratil vysledok</exception>
    public static ServerVerification RunCode(Iniss2Workspace workspace, WorkspaceState state) => Run(workspace, state, true);

    private static ServerVerification Run(Iniss2Workspace workspace, WorkspaceState state, bool codeOnly)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        var bridge = BridgePath ?? throw new ServerVerificationException(BridgeName);

        var temp = Path.Combine(Path.GetTempPath(), "GVXEditor", "verify-" + Guid.NewGuid().ToString("N"));
        var root = Path.Combine(temp, "Server");
        var watch = Stopwatch.StartNew();
        try
        {
            var data = Path.Combine(root, "Data");
            CopyDirectory(workspace.DataDir, data);
            workspace.WritePending(data, state);
            var result = RunBridge(bridge, workspace.ServerDir, root, codeOnly ? ["--code"] : []);
            if (codeOnly)
            {
                Log.Debug("Preklad kodu serverom: chyby {Errors}, {Milliseconds} ms", result.ErrorCount, watch.ElapsedMilliseconds);
            }
            else
            {
                Log.Information("Overenie kodom servera {Result}: chyby {Errors}, zaznamy Error.log {Log}, {Milliseconds} ms", result.Ok ? "bez chyby" : "s chybou",
                    result.ErrorCount, result.Log.Count, watch.ElapsedMilliseconds);
            }

            return result;
        }
        catch (Exception e) when (e is IOException or UnauthorizedAccessException or System.Xml.XmlException)
        {
            Log.Error(e, "Kopiu dat instalacie {DataDir} na overenie kodom servera sa nepodarilo pripravit", workspace.DataDir);
            throw new ServerVerificationException(e.Message, e);
        }
        finally
        {
            try
            {
                Directory.Delete(temp, true);
            }
            catch (Exception e) when (e is IOException or UnauthorizedAccessException)
            {
                Log.Warning(e, "Docasny priecinok overenia {Temp} sa nepodarilo zmazat", temp);
            }
        }
    }

    private static ServerVerification RunBridge(string bridge, string serverDir, string root, string[] options)
    {
        var start = new ProcessStartInfo(bridge)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8
        };
        foreach (var argument in new[] { "--server", serverDir, "--root", root, "--rawbank", Path.Combine(serverDir, "RawBank") }.Concat(options))
            start.ArgumentList.Add(argument);

        using var process = Process.Start(start) ?? throw new ServerVerificationException(BridgeName);
        var output = process.StandardOutput.ReadToEndAsync();
        var error = process.StandardError.ReadToEndAsync();
        if (!process.WaitForExit(Timeout))
        {
            process.Kill(true);
            Log.Error("Most {Bridge} neskoncil do {Timeout}", bridge, Timeout);
            throw new ServerVerificationException(BridgeName);
        }

        var json = output.GetAwaiter().GetResult();
        if (process.ExitCode is not (0 or 1) || json.Length == 0)
        {
            Log.Error("Most {Bridge} skoncil s kodom {ExitCode}: {Error}", bridge, process.ExitCode, error.GetAwaiter().GetResult());
            throw new ServerVerificationException(BridgeName);
        }

        try
        {
            return JsonSerializer.Deserialize<ServerVerification>(json) ?? throw new ServerVerificationException(BridgeName);
        }
        catch (JsonException e)
        {
            Log.Error(e, "Vysledok mosta {Bridge} nie je platny JSON", bridge);
            throw new ServerVerificationException(BridgeName, e);
        }
    }

    /// <summary>
    /// Mena z IniSrv.exe instalacie pre dopĺňanie v editore kodu: cleny <c>VlakS</c>, vycty a pomocne triedy servera. Vysledok
    /// sa pamata podla programu servera a casu jeho zmeny.
    /// </summary>
    /// <param name="workspace">otvorena instalacia</param>
    /// <returns>mena; prazdny zoznam, ak most alebo program servera chyba alebo zlyhal</returns>
    public static IReadOnlyList<ServerSymbol> ReadSymbols(Iniss2Workspace workspace)
    {
        ArgumentNullException.ThrowIfNull(workspace);
        if (!IsAvailable(workspace))
            return [];

        var server = Path.Combine(workspace.ServerDir, ServerExecutable);
        var key = server + "|" + File.GetLastWriteTimeUtc(server).Ticks.ToString(System.Globalization.CultureInfo.InvariantCulture);
        lock (Symbols)
        {
            if (Symbols.TryGetValue(key, out var cached))
                return cached;
        }

        var start = new ProcessStartInfo(BridgePath!)
        {
            UseShellExecute = false,
            CreateNoWindow = true,
            RedirectStandardOutput = true,
            RedirectStandardError = true,
            StandardOutputEncoding = Encoding.UTF8
        };
        foreach (var argument in new[] { "--server", workspace.ServerDir, "--symbols" })
            start.ArgumentList.Add(argument);

        try
        {
            using var process = Process.Start(start);
            if (process is null)
                return [];
            var output = process.StandardOutput.ReadToEndAsync();
            var error = process.StandardError.ReadToEndAsync();
            if (!process.WaitForExit(Timeout) || process.ExitCode != 0)
            {
                Log.Warning("Most {Bridge} nevratil mena servera: {Error}", BridgePath, error.GetAwaiter().GetResult());
                return [];
            }

            var symbols = JsonSerializer.Deserialize<ServerSymbolList>(output.GetAwaiter().GetResult())?.Symbols ?? [];
            lock (Symbols)
                Symbols[key] = symbols;
            return symbols;
        }
        catch (Exception e) when (e is System.ComponentModel.Win32Exception or IOException or JsonException or InvalidOperationException)
        {
            Log.Warning(e, "Mena servera {Server} sa nepodarilo nacitat", server);
            return [];
        }
    }

    private static readonly Dictionary<string, IReadOnlyList<ServerSymbol>> Symbols = new(StringComparer.OrdinalIgnoreCase);

    private static void CopyDirectory(string source, string target)
    {
        Directory.CreateDirectory(target);
        foreach (var file in Directory.EnumerateFiles(source))
            File.Copy(file, Path.Combine(target, Path.GetFileName(file)));
        foreach (var directory in Directory.EnumerateDirectories(source))
            CopyDirectory(directory, Path.Combine(target, Path.GetFileName(directory)));
    }
}
