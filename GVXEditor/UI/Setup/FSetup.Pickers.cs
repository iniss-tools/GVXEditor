using System.Globalization;
using GVXEditor.Domain.Setup;
using GVXEditor.Domain.Views;
using GVXEditor.Properties;
using GVXEditor.UI.Controls;

namespace GVXEditor.UI.Setup;

// vyber odkazov: stanice nazvom podla nazvov v okne, kategorie vlakov zaskrtnutim, ciel textov tabul z predloh stanice
internal partial class FSetup
{
    private static readonly string[] Categories = ["V", "P", "K"];

    // stanice nazvom podla nazvov stanic v okne (aj neulozenych) a hodnoty, ktore su v tabulkach
    private void RefreshStationChoices()
    {
        var names = new Dictionary<int, string>();
        foreach (var name in _edit.Names.Where(name => name.Name.Length > 0))
        {
            if (name.Number is { } number)
                names.TryAdd(number, name.Name);
        }

        var station = _station;
        var present = _edit.DefaultStations.Select(item => item.Station)
            .Concat(station?.ShortPatterns.Concat(station.LongPatterns).SelectMany(pattern => pattern.Stops).Select(stop => stop.Station) ?? [])
            .Concat(station?.RouteSets.SelectMany(set => set.Routes).SelectMany(route => route.Stations).Select(stop => stop.Station) ?? [])
            .Concat(station?.StationSets.SelectMany(set => set.Texts).Select(text => text.Station) ?? []);
        _stations.Fill(names, present);
    }

    // ciel textov tabul: predlohy stanice a ich udaje (TCatalog.xml) a hodnoty zo suboru
    private void RefreshTargetChoices(int stationIndex)
    {
        var catalog = stationIndex >= 0 ? _edit.Group.Stations[stationIndex].Boards?.Catalog ?? [] : [];
        var targets = _station?.RouteSets.SelectMany(set => set.Targets).Concat(_station.StationSets.SelectMany(set => set.Targets)).ToList() ?? [];
        var templates = Plain(catalog.Select(template => template.Key), targets.Select(target => target.Catalog));
        var columns = Plain(catalog.SelectMany(template => template.Columns).Select(column => column.Key), targets.Select(target => target.Column));
        foreach (var column in new[] { cRouteTargetCatalog, cStationTargetCatalog })
            Choices(column, templates);
        foreach (var column in new[] { cRouteTargetColumn, cStationTargetColumn })
            Choices(column, columns);
    }

    // hodnoty bez popisu: kluce z predloh a kluce zo suboru, ktore v predlohach nie su
    private static List<ValueChoice> Plain(IEnumerable<string> known, IEnumerable<string> present) =>
        [.. known.Concat(present).Prepend("").Distinct(StringComparer.Ordinal).Select(key => new ValueChoice(key, key))];

    private static void Choices(DataGridViewComboBoxColumn column, List<ValueChoice> choices)
    {
        column.DataSource = null;
        column.ValueMember = nameof(ValueChoice.Key);
        column.DisplayMember = nameof(ValueChoice.Text);
        column.DataSource = choices;
    }

    private static string CategoryText(string key) =>
        Resources.ResourceManager.GetString("Value_Category_" + key, Resources.Culture) ?? key;

    private static string CategoriesText(string value) => string.Join(", ", SetupText.Items(value).Select(CategoryText));

    private string? PickCategories(string value)
    {
        var items = Categories.Select(key => new ValueChoice(key, string.Format(CultureInfo.CurrentCulture, Resources.Value_WithKey, CategoryText(key), key))).ToList();
        return FPickChecked.Pick(this, Resources.Pick_CategoriesTitle, Resources.Pick_CategoriesHint, items, SetupText.Items(value)) is { } picked
            ? string.Join(" ", picked)
            : null;
    }
}
