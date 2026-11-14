using System.Globalization;
using System.Media;
using GVXEditor.Domain;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;
using Serilog;
using ToolsCore.Iniss.Entities;
using ToolsCore.Iniss.Tools;
using ToolsCore.Tools;

namespace GVXEditor.UI.Controls;

/// <summary>
/// Vyber zvukov zo zvukovej banky instalacie: strom skupin a zvukov jazyka s hladanim a prehratim. Radenie vybera zvuky
/// v poradi (cesta <c>skupina/zvuk</c>, s jazykom <c>jazyk/skupina/zvuk</c>), kolaj jeden zvuk (len kluc zvuku - skupinu
/// urcuje skript hlaseni).
/// </summary>
internal partial class FPickSound : Form
{
    private readonly SoundBank _bank;
    private readonly bool _single;
    private readonly List<ValueChoice> _selected = [];
    private readonly SoundPlayer _player = new();
    private readonly string _key;

    // pri hladani sa rozbalia vsetky skupiny, len ak je najdenych zvukov malo
    private const int ExpandFoundLimit = 300;

    // skupiny, ktorych zvuky este nie su v strome
    private readonly Dictionary<TreeNode, List<FyzSound>> _pending = [];
    private bool _loading;

    /// <param name="bank">zvukova banka</param>
    /// <param name="title">titulok okna</param>
    /// <param name="paths">vybrane cesty zvukov v poradi; <see langword="null" /> = vyber jedneho zvuku</param>
    /// <param name="key">kluc zvuku pri vybere jedneho zvuku</param>
    public FPickSound(SoundBank bank, string title, IEnumerable<string>? paths, string key = "")
    {
        ArgumentNullException.ThrowIfNull(bank);
        _bank = bank;
        _single = paths is null;
        _key = key;
        InitializeComponent();
        tvSounds.BeforeExpand += tvSounds_BeforeExpand;
        this.ApplyThemeAndFonts();
        Disposed += (_, _) => _player.Dispose();
        Text = title;
        tbSoundFilter.HintText = Resources.Pick_Filter;
        foreach (var path in paths ?? [])
            _selected.Add(new ValueChoice(path, bank.Describe(path)));

        if (_single)
        {
            // jeden zvuk: bez zoznamu vybranych, strom cez celu sirku
            foreach (var control in new Control[] { bSoundAdd, bSoundRemove, bSoundUp, bSoundDown, lSounds, lbSounds, cboxAnyLanguage })
                control.Visible = false;
            bPlay.Left = tvSounds.Left;
            bPlay.Top = tvSounds.Bottom - bPlay.Height;
            bPlay.Anchor = AnchorStyles.Bottom | AnchorStyles.Left;
            tvSounds.Height -= bPlay.Height + 6;
            tvSounds.Width = lbSounds.Right - tvSounds.Left;
            tvSounds.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
        }

        _loading = true;
        cboLanguage.DataSource = bank.Languages.Select(language => new ValueChoice(language.Key,
            string.Format(CultureInfo.CurrentCulture, Resources.Value_WithKey, language.Name, language.Key))).ToList();
        _loading = false;
        ShowSounds();
        ShowSelected();
    }

    /// <summary>Vybrane cesty zvukov v poradi.</summary>
    public IReadOnlyList<string> Paths => [.. _selected.Select(choice => choice.Key)];

    /// <summary>Kluc zvuku vybraneho v strome (vyber jedneho zvuku).</summary>
    public string? SoundKey => (tvSounds.SelectedNode?.Tag as FyzSound)?.Key;

    /// <summary>
    /// Vyber zvukov v poradi; <see langword="null" />, ak obsluha okno zrusila.
    /// </summary>
    public static IReadOnlyList<string>? PickList(IWin32Window owner, SoundBank bank, string title, IEnumerable<string> paths)
    {
        using var form = new FPickSound(bank, title, paths);
        return form.ShowDialog(owner) == DialogResult.OK ? form.Paths : null;
    }

    /// <summary>
    /// Vyber kluca jedneho zvuku; <see langword="null" />, ak obsluha okno zrusila alebo zvuk nevybrala.
    /// </summary>
    public static string? PickKey(IWin32Window owner, SoundBank bank, string title, string key)
    {
        using var form = new FPickSound(bank, title, null, key);
        return form.ShowDialog(owner) == DialogResult.OK ? form.SoundKey : null;
    }

    private FyzLanguage? Language => cboLanguage.SelectedItem is ValueChoice choice ? _bank.Sounds(choice.Key) : null;

    private void cboLanguage_SelectedIndexChanged(object? sender, EventArgs e)
    {
        if (!_loading)
            ShowSounds();
    }

    private void tbSoundFilter_TextChanged(object? sender, EventArgs e) => ShowSounds();

    private static string SoundText(FyzSound sound) =>
        sound.Text.Length > 0 ? string.Format(CultureInfo.CurrentCulture, Resources.Key_Choice, sound.Key, sound.Text) : sound.Key;

    // skupiny a zvuky jazyka; hladanie podla kluca a textu zvuku alebo nazvu skupiny. Jazyk ma tisice zvukov a strom
    // s nimi sa otvaral aj zatvaral sekundy, preto skupina dostane zvuky az pri rozbaleni (Fill).
    private void ShowSounds()
    {
        var filter = tbSoundFilter.Text.Trim();
        bool Matches(string text) => text.Contains(filter, StringComparison.CurrentCultureIgnoreCase);
        _pending.Clear();
        var groups = new List<(TreeNode Node, List<FyzSound> Sounds)>();
        foreach (var group in Language?.Groups ?? [])
        {
            var sounds = group.Sounds.Where(sound => filter.Length == 0 || Matches(group.Key) || Matches(sound.Key) || Matches(sound.Text)).ToList();
            if (sounds.Count == 0)
                continue;
            // prazdny uzol len pre tlacidlo rozbalenia
            var node = new TreeNode(group.Key, [new TreeNode()]) { Tag = group };
            _pending[node] = sounds;
            groups.Add((node, sounds));
        }

        TreeNode? selected = null;
        var found = filter.Length > 0 ? groups.Sum(group => group.Sounds.Count) : 0;
        tvSounds.BeginUpdate();
        tvSounds.Nodes.Clear();
        tvSounds.Nodes.AddRange([.. groups.Select(group => group.Node)]);
        foreach (var (node, sounds) in groups)
        {
            var withKey = selected is null && _single && _key.Length > 0 && sounds.Any(sound => sound.Key == _key);
            if (!withKey && (filter.Length == 0 || found > ExpandFoundLimit))
                continue;
            Fill(node);
            node.Expand();
            if (withKey)
                selected = node.Nodes.Cast<TreeNode>().First(child => child.Tag is FyzSound sound && sound.Key == _key);
        }

        tvSounds.EndUpdate();
        if (selected is not null)
        {
            tvSounds.SelectedNode = selected;
            selected.EnsureVisible();
        }
    }

    // zvuky skupiny do stromu naraz (AddRange), len raz
    private void Fill(TreeNode node)
    {
        if (!_pending.Remove(node, out var sounds))
            return;
        node.Nodes.Clear();
        node.Nodes.AddRange([.. sounds.Select(sound => new TreeNode(SoundText(sound)) { Tag = sound })]);
    }

    private void tvSounds_BeforeExpand(object? sender, TreeViewCancelEventArgs e)
    {
        if (e.Node is null || !_pending.ContainsKey(e.Node))
            return;
        tvSounds.BeginUpdate();
        Fill(e.Node);
        tvSounds.EndUpdate();
    }

    private void ShowSelected(IEnumerable<ValueChoice>? select = null)
    {
        lbSounds.BeginUpdate();
        lbSounds.Items.Clear();
        foreach (var choice in _selected)
            lbSounds.Items.Add(choice);
        foreach (var choice in select ?? [])
            lbSounds.SelectedItems.Add(choice);
        lbSounds.EndUpdate();
    }

    private void tvSounds_NodeMouseDoubleClick(object? sender, TreeNodeMouseClickEventArgs e)
    {
        if (e.Node?.Tag is not FyzSound)
            return;
        if (_single)
            DialogResult = DialogResult.OK;
        else
            bSoundAdd_Click(sender, e);
    }

    private void bSoundAdd_Click(object? sender, EventArgs e)
    {
        if (tvSounds.SelectedNode?.Tag is not FyzSound sound || cboLanguage.SelectedItem is not ValueChoice language)
            return;
        // bez jazyka sa zvuk prehra v jazyku hlasenia
        var path = cboxAnyLanguage.Checked ? $"{sound.Group.Key}/{sound.Key}" : $"{language.Key}/{sound.Group.Key}/{sound.Key}";
        var choice = new ValueChoice(path, _bank.Describe(path));
        _selected.Add(choice);
        ShowSelected([choice]);
    }

    private void bSoundRemove_Click(object? sender, EventArgs e)
    {
        foreach (var choice in lbSounds.SelectedItems.Cast<ValueChoice>().ToList())
            _selected.Remove(choice);
        ShowSelected();
    }

    private void bSoundUp_Click(object? sender, EventArgs e) => MoveSelected(-1);

    private void bSoundDown_Click(object? sender, EventArgs e) => MoveSelected(1);

    // posun vybranych o jedno miesto (radenie moze mat ten isty zvuk viackrat - podla polohy, nie hodnoty)
    private void MoveSelected(int by)
    {
        var indexes = lbSounds.SelectedIndices.Cast<int>().Order().ToList();
        if (indexes.Count == 0 || (by < 0 && indexes[0] == 0) || (by > 0 && indexes[^1] == _selected.Count - 1))
            return;
        foreach (var index in by < 0 ? indexes : Enumerable.Reverse(indexes))
            (_selected[index], _selected[index + by]) = (_selected[index + by], _selected[index]);
        ShowSelected();
        foreach (var index in indexes)
            lbSounds.SetSelected(index + by, true);
    }

    // prehra zvuk vybrany v strome (subor EWA sa prevedie na WAV v pamati)
    private void bPlay_Click(object? sender, EventArgs e)
    {
        if (tvSounds.SelectedNode?.Tag is not FyzSound sound)
            return;
        var file = sound.GetAbsPath(_bank.Path.TrimEnd('\\', '/') + Path.DirectorySeparatorChar);
        try
        {
            var wav = new MemoryStream();
            if (string.Equals(Path.GetExtension(file), ".wav", StringComparison.OrdinalIgnoreCase))
            {
                using var input = File.OpenRead(file);
                input.CopyTo(wav);
            }
            else
            {
                using var input = File.OpenRead(file);
                EwaCodec.Decode(input, wav);
            }

            wav.Position = 0;
            _player.Stop();
            _player.Stream = wav;
            _player.Play();
        }
        catch (Exception exception) when (exception is IOException or UnauthorizedAccessException or InvalidDataException or InvalidOperationException)
        {
            Log.Warning(exception, "Zvuk {File} sa nepodarilo prehrat", file);
            Utils.ShowError(string.Format(CultureInfo.CurrentCulture, Resources.Pick_PlayFailed, file));
        }
    }
}
