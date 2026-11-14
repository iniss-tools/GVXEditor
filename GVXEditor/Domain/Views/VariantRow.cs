using System.Collections;
using GVXEditor.Properties;
using ToolsCore.Iniss2.Grafikon;

namespace GVXEditor.Domain.Views;

/// <summary>
/// Riadok zoznamu variant vlaku.
/// </summary>
/// <param name="Variant">varianta</param>
/// <param name="Index">poradie varianty od 1 - v dany den plati prva varianta, ktora ide</param>
/// <param name="Validity">datumova poznamka ako v subore</param>
/// <param name="Days">pocet dni, ked plati prave tato varianta (bez dni skorsich variant)</param>
/// <param name="Operator">dopravca</param>
/// <param name="Flags">priznaky klucmi ako v subore</param>
/// <param name="Note">prvy riadok poznamky</param>
internal sealed record VariantRow(GvVariant Variant, int Index, string Validity, int Days, string Operator, string Flags, string Note)
{
    /// <summary>
    /// Riadky variant vlaku v poradi suboru.
    /// </summary>
    public static List<VariantRow> Create(GvTrain train)
    {
        ArgumentNullException.ThrowIfNull(train);

        return train.Variants.Select((variant, index) => new VariantRow(variant, index + 1, ValidityText(variant.Validity),
            Count(train.ActiveDaysOf(variant)), variant.Operator, ValueTexts.VariantFlags(variant.Flags), FirstLine(variant.Note))).ToList();
    }

    private static string ValidityText(string validity) => validity switch
    {
        "" => Resources.Validity_Daily,
        "-" or "--" => Resources.Validity_Never,
        _ => validity
    };

    private static int Count(BitArray days)
    {
        var count = 0;
        for (var i = 0; i < days.Length; i++)
        {
            if (days[i])
                count++;
        }

        return count;
    }

    private static string FirstLine(string note)
    {
        var end = note.AsSpan().IndexOfAny('\r', '\n');
        return end < 0 ? note : note[..end];
    }
}
