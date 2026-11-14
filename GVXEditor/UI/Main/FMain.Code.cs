using System.Globalization;
using GVXEditor.Domain.Code;
using GVXEditor.Properties;
using GVXEditor.UI.Boards;
using GVXEditor.UI.Code;
using Serilog;

namespace GVXEditor.UI.Main;

// kod, ktory server preklada: stavovy diagram, tabulky TabTab, skript hlaseni a tabule s podmienkami zobrazenia
internal partial class FMain
{
    private void CreateCodeCommands()
    {
        _commands.Add(GvxCommands.StateDiagram, StateDiagram, () => SelectedGroupIndex >= 0).Bind(tsmimStateDiagram, tsmitStateDiagram);
        _commands.Add(GvxCommands.TabTab, TabTab, () => SelectedGroupIndex >= 0).Bind(tsmimTabTab, tsmitTabTab);
        _commands.Add(GvxCommands.Reports, Reports, () => _ctx.Workspace?.History.Current.LogText is not null).Bind(tsmimReports);
        _commands.Add(GvxCommands.LogText, LogText, () => _ctx.Workspace?.History.Current.LogText is not null).Bind(tsmimLogText);
        _commands.Add(GvxCommands.Boards, Boards, () => SelectedGroupIndex >= 0).Bind(tsmimBoards, tsmitBoards);
    }

    private void StateDiagram()
    {
        var index = SelectedGroupIndex;
        if (_ctx.Workspace is not { } workspace || index < 0)
            return;

        var group = workspace.Groups[index];
        using var form = new FStateDiagram(_ctx, index, Math.Max(0, SelectedStationIndex()));
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var updated = form.Apply(group);
        if (ReferenceEquals(updated, group))
            return;

        var label = string.Format(CultureInfo.CurrentCulture, Resources.History_StateDiagram, group.Group.Name);
        Log.Information("Grafikon {Group}: {Change}", group.Group.Name, label);
        workspace.Change(index, updated, label);
        RefreshView();
    }

    private void TabTab()
    {
        var index = SelectedGroupIndex;
        if (_ctx.Workspace is not { } workspace || index < 0)
            return;

        var group = workspace.Groups[index];
        using var form = new FTabTabs(_ctx, index, SelectedStationIndex());
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var updated = form.Apply(group);
        if (ReferenceEquals(updated, group))
            return;

        var label = string.Format(CultureInfo.CurrentCulture, Resources.History_TabTab, group.Group.Name);
        Log.Information("Grafikon {Group}: {Change}", group.Group.Name, label);
        workspace.Change(index, updated, label);
        RefreshView();
    }

    private void Boards()
    {
        var index = SelectedGroupIndex;
        if (_ctx.Workspace is not { } workspace || index < 0)
            return;

        var group = workspace.Groups[index];
        using var form = new FBoards(_ctx, index, Math.Max(0, SelectedStationIndex()));
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var updated = form.Apply(group);
        if (ReferenceEquals(updated, group))
            return;

        var label = string.Format(CultureInfo.CurrentCulture, Resources.History_Boards, group.Group.Name);
        Log.Information("Grafikon {Group}: {Change}", group.Group.Name, label);
        workspace.Change(index, updated, label);
        RefreshView();
    }

    private void Reports()
    {
        if (_ctx.Workspace is not { } workspace || workspace.History.Current.LogText is not { } text)
            return;

        using var form = new FReports(_ctx);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        // skript a pri premenovani aj odkazy v datach ako jeden krok historie
        var current = workspace.History.Current;
        var updated = form.Result();
        var state = ReferenceEquals(updated, text) ? current : current with { LogText = updated };
        if (form.UpdateReferences)
            state = ReportRenames.Apply(state, form.Renames);
        if (ReferenceEquals(state, current))
            return;

        Log.Information("Instalacia {DataDir}: {Change}", workspace.DataDir, Resources.History_Reports);
        workspace.ChangeState(state, Resources.History_Reports);
        RefreshView();
    }

    private void LogText()
    {
        if (_ctx.Workspace is not { } workspace || workspace.History.Current.LogText is not { } text)
            return;

        using var form = new FLogText(_ctx);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var updated = form.Result();
        if (ReferenceEquals(updated, text))
            return;

        Log.Information("Instalacia {DataDir}: {Change}", workspace.DataDir, Resources.History_LogText);
        workspace.ChangeLogText(updated, Resources.History_LogText);
        RefreshView();
    }
}
