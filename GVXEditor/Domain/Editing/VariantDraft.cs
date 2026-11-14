using System.ComponentModel;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Editing;

/// <summary>
/// Varianta vlaku v okne Upravit vlak.
/// </summary>
internal sealed class VariantDraft
{
    /// <summary>Datumova poznamka (server cita cesky aj slovensky, sviatky ceske); prazdna = kazdy den obdobia.</summary>
    public string Validity { get; set; } = "";

    /// <summary>Dopravca.</summary>
    public string Operator { get; set; } = "";

    /// <summary>Priznaky varianty.</summary>
    public GvVariantFlags Flags { get; set; }

    /// <summary>Poznamka.</summary>
    public string Note { get; set; } = "";

    /// <summary>Body trasy v poradi jazdy.</summary>
    public BindingList<StopDraft> Stops { get; init; } = [];

    /// <summary>
    /// O kolko dni po referencnom dni vlaku je prvy bod - z povodnej varianty, nova ma 0.
    /// </summary>
    public int FirstDay { get; init; }

    /// <summary>
    /// Koncept varianty.
    /// </summary>
    public static VariantDraft Of(GvVariant variant)
    {
        ArgumentNullException.ThrowIfNull(variant);
        var first = variant.Stops.Count > 0 ? variant.Stops[0] : null;
        return new VariantDraft
        {
            Validity = variant.Validity,
            Operator = variant.Operator,
            Flags = variant.Flags,
            Note = variant.Note,
            Stops = [.. variant.Stops.Select(StopDraft.Of)],
            FirstDay = first is null ? 0 : first.Departure.HasValue ? first.DepartureDay : first.ArrivalDay
        };
    }

    /// <summary>
    /// Kopia varianty s kopiami bodov.
    /// </summary>
    public VariantDraft Clone() => new()
    {
        Validity = Validity,
        Operator = Operator,
        Flags = Flags,
        Note = Note,
        Stops = [.. Stops.Select(stop => stop.Clone())],
        FirstDay = FirstDay
    };
}
