using System.Globalization;

namespace GVXEditor.Domain.Views;

/// <summary>
/// Porovnanie textov s cislami podla hodnoty cisel (<c>611</c> pred <c>3101</c>, <c>Os 2</c> pred <c>Os 10</c>), ostatny text
/// podla jazyka programu bez ohladu na velkost pismen.
/// </summary>
internal static class NaturalText
{
    public static int Compare(string? a, string? b)
    {
        a ??= "";
        b ??= "";
        int i = 0, j = 0;
        while (i < a.Length && j < b.Length)
        {
            if (char.IsAsciiDigit(a[i]) && char.IsAsciiDigit(b[j]))
            {
                var startA = i;
                var startB = j;
                while (i < a.Length && char.IsAsciiDigit(a[i])) i++;
                while (j < b.Length && char.IsAsciiDigit(b[j])) j++;
                var numberA = a.AsSpan(startA, i - startA).TrimStart('0');
                var numberB = b.AsSpan(startB, j - startB).TrimStart('0');
                // dlhsie cislo bez nul na zaciatku je vacsie, rovnako dlhe sa porovnaju po cislicach
                var result = numberA.Length != numberB.Length ? numberA.Length.CompareTo(numberB.Length) : numberA.SequenceCompareTo(numberB);
                if (result != 0)
                    return result;
            }
            else
            {
                var startA = i;
                var startB = j;
                while (i < a.Length && !char.IsAsciiDigit(a[i])) i++;
                while (j < b.Length && !char.IsAsciiDigit(b[j])) j++;
                var result = CultureInfo.CurrentCulture.CompareInfo.Compare(a.AsSpan(startA, i - startA), b.AsSpan(startB, j - startB),
                    CompareOptions.IgnoreCase);
                if (result != 0)
                    return result;
            }
        }

        return (a.Length - i).CompareTo(b.Length - j);
    }
}
