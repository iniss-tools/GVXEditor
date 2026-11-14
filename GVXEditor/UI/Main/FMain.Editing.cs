using System.Globalization;
using GVXEditor.Domain;
using GVXEditor.Domain.Analysis;
using GVXEditor.Domain.Editing;
using GVXEditor.Domain.Server;
using GVXEditor.Properties;
using GVXEditor.UI.EditTrain;
using Serilog;
using ToolsCore.Iniss2.Grafikon;
using ToolsCore.Tools;

namespace GVXEditor.UI.Main;

internal partial class FMain
{
    /// <summary>
    /// Poradie grafikonu vybraneho v strome (aj pri vybranej stanici); -1 = ziadny.
    /// </summary>
    private int SelectedGroupIndex => tvData.SelectedNode?.Tag switch
    {
        GroupNode group => group.Index,
        StationNode station => station.GroupIndex,
        _ => -1
    };

    /// <summary>
    /// Grafikon so zoznamom vlakov v pravej casti okna; <see langword="null" />, ak okno ukazuje nieco ine.
    /// </summary>
    private GroupDocument? ShownGroup =>
        _ctx.Workspace is { } workspace && tvData.SelectedNode?.Tag is GroupNode node && workspace.Groups[node.Index].Timetable is not null
            ? workspace.Groups[node.Index]
            : null;

    private void CreateEditCommands()
    {
        _commands.Add(GvxCommands.Save, () => _ = SaveWorkspaceAsync(), () => _ctx.Workspace?.IsDirty == true).Bind(tsmimSave, tsbSave);
        _commands.Add(GvxCommands.Check, ShowAnalysis, () => _ctx.Workspace is not null).Bind(tsmimCheck);
        _commands.Add(GvxCommands.VerifyServer, VerifyServer, () => _ctx.Workspace is not null).Bind(tsmimVerifyServer);
        _commands.Add(GvxCommands.Undo, () => Undo(1), () => _ctx.Workspace?.History.CanUndo == true).Bind(tsmimUndo, tssbUndo);
        _commands.Add(GvxCommands.Redo, () => Redo(1), () => _ctx.Workspace?.History.CanRedo == true).Bind(tsmimRedo, tssbRedo);
        _commands.Add(GvxCommands.AddTrain, AddTrain, () => ShownGroup is not null).Bind(tsmimNewTrain, tsbNewTrain, tsmicNewTrain);
        _commands.Add(GvxCommands.EditTrain, EditTrain, () => CurrentTrainIndex >= 0).Bind(tsmimEditTrain, tsbEditTrain, tsmicEditTrain);
        _commands.Add(GvxCommands.CopyTrain, CopyTrain, () => SelectedTrainIndexes.Count > 0).Bind(tsmimCopyTrain, tsbCopyTrain, tsmicCopyTrain);
        _commands.Add(GvxCommands.DeleteTrain, DeleteTrain, () => SelectedTrainIndexes.Count > 0).Bind(tsmimDeleteTrain, tsbDeleteTrain, tsmicDeleteTrain);
        _commands.Add(GvxCommands.RenameNumber, RenameNumber, () => CurrentTrainIndex >= 0).Bind(tsmimRenameNumber, tsmicRenameNumber);
        _commands.Add(GvxCommands.ShiftValidity, ShiftValidity, () => ShownGroup is not null).Bind(tsmimShiftValidity, tsmicShiftValidity);
    }

    private static string TrainLabel(GvTrain train)
    {
        var number = train.Variants.SelectMany(variant => variant.Stops).Select(stop => stop.Number.Length > 0 ? stop.Number : stop.DisplayNumber)
            .FirstOrDefault(text => text.Length > 0);
        return number ?? "?";
    }

    private void AddTrain()
    {
        if (ShownGroup is not { Timetable: { } timetable } group)
            return;

        // obdobie ako ma vybrany vlak (v grafikone maju vlaky spravidla rovnake), inak rok od dneska
        var source = CurrentTrainIndex >= 0 ? timetable.Trains[CurrentTrainIndex] : timetable.Trains.Count > 0 ? timetable.Trains[0] : null;
        var today = DateOnly.FromDateTime(DateTime.Today);
        var from = source is null || source.From == DateOnly.MinValue ? today : source.From;
        var noEnd = source?.To == DateOnly.MaxValue;
        var to = source is null || noEnd ? from.AddDays(364) : source.To;
        var draft = TrainDraft.New(from, to);
        draft.NoEnd = noEnd;

        using var form = new FEditTrain(_ctx, draft, group, -1, StationName, Resources.Edit_TitleNew);
        if (form.ShowDialog(this) != DialogResult.OK || form.Result is not { } train)
            return;

        var updated = form.ApplyRecords(group with { Timetable = TimetableEditing.Add(timetable, train) });
        ChangeGroup(group, updated, Label(Resources.History_New, train), timetable.Trains.Count, logText: form.LogText);
    }

    private void EditTrain()
    {
        var index = CurrentTrainIndex;
        if (ShownGroup is not { Timetable: { } timetable } group || index < 0)
            return;

        var original = timetable.Trains[index];
        var title = string.Format(CultureInfo.CurrentCulture, Resources.Edit_Title, TrainLabel(original));
        using var form = new FEditTrain(_ctx, TrainDraft.Of(original), group, index, StationName, title);
        if (form.ShowDialog(this) != DialogResult.OK || form.Result is not { } train)
            return;

        // okno potvrdene bez zmeny vlaku necha GV.XML, zmenia sa len zaznamy stanic, ak nejake
        var changed = TimetableEditing.SameTrain(original, train) ? timetable : TimetableEditing.Replace(timetable, index, train);
        var updated = form.ApplyRecords(ReferenceEquals(changed, timetable) ? group : group with { Timetable = changed });
        if (!ReferenceEquals(updated, group) || form.LogText is not null)
            ChangeGroup(group, updated, Label(Resources.History_Edit, train), index, logText: form.LogText);
    }

    // kopia kazdeho vybraneho vlaku hned za nim; vybrane su potom kopie
    private void CopyTrain()
    {
        var indexes = SelectedTrainIndexes;
        if (ShownGroup is not { Timetable: { } timetable } group || indexes.Count == 0)
            return;

        var copies = indexes.Select((index, order) => index + order + 1).ToList();
        var current = indexes.IndexOf(CurrentTrainIndex) is var position and >= 0 ? copies[position] : copies[0];
        var label = indexes.Count == 1
            ? Label(Resources.History_Copy, timetable.Trains[indexes[0]])
            : string.Format(CultureInfo.CurrentCulture, Resources.History_CopyMany, indexes.Count);
        ChangeGroup(group, group with { Timetable = TimetableEditing.Copy(timetable, indexes) }, label, current, copies);
    }

    private void DeleteTrain()
    {
        var indexes = SelectedTrainIndexes;
        if (ShownGroup is not { Timetable: { } timetable } group || indexes.Count == 0)
            return;

        var trains = indexes.Select(index => timetable.Trains[index]).ToList();
        var question = trains.Count == 1
            ? string.Format(CultureInfo.CurrentCulture, Resources.Main_DeleteTrain, TrainLabel(trains[0]))
            : string.Format(CultureInfo.CurrentCulture, Resources.Main_DeleteTrains, trains.Count);
        if (Utils.ShowQuestion(question) != DialogResult.Yes)
            return;

        // zaznamy stanic, ktore po zmazani nepouziva ziadny vlak, sa zmazu so suhlasom
        var updated = group with { Timetable = TimetableEditing.Remove(timetable, indexes) };
        var cleaned = updated;
        var removed = 0;
        foreach (var train in trains)
        {
            (cleaned, var count) = TrainNumbers.RemoveUnused(cleaned, train);
            removed += count;
        }

        var records = trains.Count == 1 ? Resources.Main_DeleteRecords : Resources.Main_DeleteRecordsMany;
        if (removed > 0 && Utils.ShowQuestion(string.Format(CultureInfo.CurrentCulture, records, removed)) == DialogResult.Yes)
            updated = cleaned;

        var label = trains.Count == 1
            ? Label(Resources.History_Delete, trains[0])
            : string.Format(CultureInfo.CurrentCulture, Resources.History_DeleteMany, trains.Count);
        var next = NextAfterDelete(indexes);
        ChangeGroup(group, updated, label, next, next >= 0 ? [next] : []);
    }

    // po zmazani ostane kurzor na vlaku za prvym zmazanym v poradi zoznamu (aj zoradeneho), inak na poslednom pred nim;
    // vrati jeho poradie v GV.XML po zmazani
    private int NextAfterDelete(List<int> deleted)
    {
        var rows = dgvTrains.Rows.Cast<DataGridViewRow>().Select(row => row.DataBoundItem).OfType<Domain.Views.TrainRow>().ToList();
        var first = rows.FindIndex(row => deleted.Contains(row.Index));
        var next = rows.Skip(first + 1).FirstOrDefault(row => !deleted.Contains(row.Index))
                   ?? rows.Take(Math.Max(first, 0)).LastOrDefault(row => !deleted.Contains(row.Index));
        return next is null ? -1 : next.Index - deleted.Count(index => index < next.Index);
    }

    private void RenameNumber()
    {
        var index = CurrentTrainIndex;
        if (ShownGroup is not { Timetable: not null } group || index < 0)
            return;

        using var form = new FRenameNumber(group, index);
        if (form.ShowDialog(this) != DialogResult.OK)
            return;

        var label = string.Format(CultureInfo.CurrentCulture, Resources.History_Rename, form.OldNumber, form.NewNumber);
        ChangeGroup(group, TrainNumbers.Rename(group, index, form.OldNumber, form.NewNumber, form.AllTrains), label, index);
    }

    // pri vybere viacerych vlakov len ich, inak vsetky vlaky grafikonu
    private void ShiftValidity()
    {
        if (ShownGroup is not { Timetable: { } timetable } group)
            return;

        var selected = SelectedTrainIndexes;
        var only = selected.Count > 1 ? selected.ToHashSet() : null;
        using var form = new FShiftValidity(group.Group.Name, only?.Count ?? 0);
        if (form.ShowDialog(this) != DialogResult.OK || form.Days == 0)
            return;

        var label = only is null
            ? string.Format(CultureInfo.CurrentCulture, Resources.History_Shift, form.Days)
            : string.Format(CultureInfo.CurrentCulture, Resources.History_ShiftSelected, only.Count, form.Days);
        ChangeGroup(group, group with { Timetable = TimetableEditing.ShiftValidity(timetable, form.Days, only) }, label, CurrentTrainIndex, selected);
    }

    private static string Label(string format, GvTrain train) => string.Format(CultureInfo.CurrentCulture, format, TrainLabel(train));

    // zmena vybraneho grafikonu ako krok historie, potom zoznam s vybranym vlakom (poradie v GV.XML) a vyberom
    private void ChangeGroup(GroupDocument group, GroupDocument updated, string label, int select, IReadOnlyCollection<int>? selected = null,
        string? logText = null)
    {
        if (_ctx.Workspace is not { } workspace)
            return;

        // prikazy uprav idu len pri zozname vlakov - grafikon je vybrany v strome
        var index = SelectedGroupIndex;
        if (index < 0 || !ReferenceEquals(workspace.Groups[index], group))
            return;

        Log.Information("Grafikon {Group}: {Change}", group.Group.Name, label);
        // dodatok zalozeny v okne vlaku: skript hlaseni a grafikon ako jeden krok historie
        if (logText is null)
            workspace.Change(index, updated, label);
        else
            workspace.ChangeState(workspace.History.Current.With(index, updated) with { LogText = logText }, label);
        RefreshView(select, selected ?? [select]);
    }

    private void Undo(int steps)
    {
        if (_ctx.Workspace is not { } workspace || !workspace.History.CanUndo)
            return;

        workspace.History.Undo(steps);
        RefreshView();
    }

    private void Redo(int steps)
    {
        if (_ctx.Workspace is not { } workspace || !workspace.History.CanRedo)
            return;

        workspace.History.Redo(steps);
        RefreshView();
    }

    /// <summary>
    /// Prava cast okna podla aktualneho stavu dat s rovnakym vyberom vlakov (po zmene v inom okne, kroku spat alebo znova).
    /// </summary>
    private void RefreshView() => RefreshView(CurrentTrainIndex, SelectedTrainIndexes);

    /// <summary>
    /// Prava cast okna podla aktualneho stavu dat (po zmene, kroku spat alebo znova), stav prikazov a titulok.
    /// </summary>
    /// <param name="current">poradie vlaku v GV.XML, na ktorom ma byt kurzor v zozname vlakov</param>
    /// <param name="selected">poradie vybranych vlakov v GV.XML</param>
    private void RefreshView(int current, IReadOnlyCollection<int> selected)
    {
        switch (tvData.SelectedNode?.Tag)
        {
            case GroupNode group:
                ShowTrains(group.Index);
                SelectTrains(current, selected);
                break;
            case StationNode station:
                ShowStation(station);
                break;
        }

        UpdateDirtyState();
    }

    /// <summary>
    /// Titulok (program, priecinok dat instalacie, * pri neulozenych zmenach) a ponuky Spat a Znova podla historie.
    /// </summary>
    private void UpdateDirtyState()
    {
        var workspace = _ctx.Workspace;
        Text = workspace is null ? Application.ProductName : Application.ProductName + " - " + workspace.DataDir + (workspace.IsDirty ? "*" : "");

        var undo = First(workspace?.History.UndoLabels);
        var redo = First(workspace?.History.RedoLabels);
        tsmimUndo.Text = undo is null ? Resources.Cmd_Undo : string.Format(CultureInfo.CurrentCulture, Resources.History_UndoItem, undo);
        tsmimRedo.Text = redo is null ? Resources.Cmd_Redo : string.Format(CultureInfo.CurrentCulture, Resources.History_RedoItem, redo);
        tssbUndo.Text = tsmimUndo.Text;
        tssbRedo.Text = tsmimRedo.Text;
        UpdateCommandStates();
        ScheduleAnalysis();
    }

    private static string? First(IReadOnlyList<string>? labels) => labels is { Count: > 0 } ? labels[0] : null;

    // rozbalovacia ponuka Spat a Znova - zoznam krokov, klik vrati alebo zopakuje vsetky po vybrany
    private static void FillHistoryMenu(ToolStripSplitButton button, IReadOnlyList<string> labels, Action<int> run)
    {
        button.DropDownItems.Clear();
        for (var i = 0; i < labels.Count; i++)
        {
            var steps = i + 1;
            var entry = new ToolStripMenuItem(labels[i], null, (_, _) => run(steps));
            entry.ApplyThemeAndFont();
            button.DropDownItems.Add(entry);
        }
    }

    private void tssbUndo_DropDownOpening(object? sender, EventArgs e) =>
        FillHistoryMenu(tssbUndo, _ctx.Workspace?.History.UndoLabels ?? [], Undo);

    private void tssbRedo_DropDownOpening(object? sender, EventArgs e) =>
        FillHistoryMenu(tssbRedo, _ctx.Workspace?.History.RedoLabels ?? [], Redo);

    /// <summary>
    /// Ulozi zmenene grafikony. Pri beziacom serveri nezapise nic, subory zmenene inym programom prepise len so suhlasom.
    /// </summary>
    /// <returns>zmeny su ulozene (alebo ziadne neboli)</returns>
    private bool SaveWorkspace() =>
        _ctx.Workspace is not { } workspace || !workspace.IsDirty || SaveChecked(workspace, VerifyBlocking());

    /// <summary>
    /// Ulozenie s overenim kodom servera na pozadi (prikaz Ulozit).
    /// </summary>
    private async Task<bool> SaveWorkspaceAsync()
    {
        if (_ctx.Workspace is not { } workspace || !workspace.IsDirty)
            return true;
        return SaveChecked(workspace, await VerifyAsync());
    }

    // chyba, pre ktoru by sa server nespustil (kontrola aj overenie kodom servera), sa ulozi len so suhlasom
    private bool SaveChecked(Iniss2Workspace workspace, ServerVerification? server)
    {
        var errors = InstallationChecks.Run(workspace).Where(finding => finding.Severity == FindingSeverity.Error).ToList();
        if (server is not null)
            errors.AddRange(ServerFindings.Of(server).Where(finding => finding.Severity == FindingSeverity.Error));
        if (errors.Count > 0 && Utils.ShowQuestion(string.Format(CultureInfo.CurrentCulture, Resources.Main_SaveWithErrors, errors.Count) +
                Environment.NewLine + Environment.NewLine + errors[0].Place + ": " + errors[0].Message) != DialogResult.Yes)
            return false;

        try
        {
            var result = workspace.Save();
            if (result.Outcome == SaveOutcome.ChangedOnDisk)
            {
                var files = string.Join(Environment.NewLine, result.ChangedFiles);
                if (Utils.ShowQuestion(string.Format(CultureInfo.CurrentCulture, Resources.Main_SaveChangedOnDisk, files)) != DialogResult.Yes)
                    return false;
                result = workspace.Save(overwriteChanged: true);
            }

            switch (result.Outcome)
            {
                case SaveOutcome.ServerRunning:
                    Log.Warning("Instalacia {DataDir} sa neulozila - bezi jej server", workspace.DataDir);
                    Utils.ShowWarning(Resources.Main_SaveServerRunning);
                    return false;
                case SaveOutcome.Saved:
                    Log.Information("Instalacia {DataDir} je ulozena", workspace.DataDir);
                    break;
            }
        }
        catch (SaveFailedException e)
        {
            Utils.ShowError(e.RolledBack
                ? string.Format(CultureInfo.CurrentCulture, Resources.Main_SaveFailedRolledBack, e.Message)
                : string.Format(CultureInfo.CurrentCulture, Resources.Main_SaveFailedBackup, e.BackupPath, e.Message));
            return false;
        }
        finally
        {
            UpdateDirtyState();
        }

        return true;
    }

    /// <summary>
    /// Pred zatvorenim instalacie (otvorenie inej, zatvorenie okna) ponukne ulozenie zmien.
    /// </summary>
    /// <returns>instalaciu mozno zatvorit</returns>
    private bool ConfirmClose()
    {
        if (_ctx.Workspace?.IsDirty != true)
            return true;

        return Utils.ShowQuestion(Resources.Main_SaveChanges, MessageBoxButtons.YesNoCancel) switch
        {
            DialogResult.Yes => SaveWorkspace(),
            DialogResult.No => true,
            _ => false
        };
    }

    private void dgvTrains_CellDoubleClick(object? sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex >= 0)
            _commands[GvxCommands.EditTrain.Id].Execute();
    }
}
