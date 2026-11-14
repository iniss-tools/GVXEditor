namespace GVXEditor.Domain.Stations;

/// <summary>
/// Nahradenie zaznamov jedneho vlaku v zozname suboru - zaznamy ostatnych vlakov ostanu na mieste.
/// </summary>
internal static class RecordLists
{
    /// <summary>
    /// Zoznam, v ktorom su zaznamy vlaku nahradene <paramref name="replacement" /> po poradi: nove zaznamy idu na miesta
    /// povodnych (aj ked v subore neboli za sebou), dalsie za posledny povodny, bez povodnych na koniec; miesta navyse
    /// zaniknu. Poradie urcuje, ktory zaznam server najde prvy. Ak sa nic nezmenilo, vrati <paramref name="all" /> - podla
    /// odkazu sa zisti, ze subor netreba zapisat.
    /// </summary>
    /// <param name="all">zaznamy suboru</param>
    /// <param name="mine">zaznam patri vlaku</param>
    /// <param name="replacement">nove zaznamy vlaku v poradi</param>
    /// <param name="same">rovnost zaznamov (zaznamy so zoznamami nemaju hodnotovu rovnost)</param>
    public static IReadOnlyList<T> Replace<T>(IReadOnlyList<T> all, Func<T, bool> mine, IReadOnlyList<T> replacement, Func<T, T, bool> same)
    {
        ArgumentNullException.ThrowIfNull(all);
        ArgumentNullException.ThrowIfNull(mine);
        ArgumentNullException.ThrowIfNull(replacement);
        ArgumentNullException.ThrowIfNull(same);

        var last = -1;
        for (var i = 0; i < all.Count; i++)
        {
            if (mine(all[i]))
                last = i;
        }

        var result = new List<T>(all.Count + replacement.Count);
        var next = 0;
        for (var i = 0; i < all.Count; i++)
        {
            if (!mine(all[i]))
                result.Add(all[i]);
            else if (next < replacement.Count)
                result.Add(replacement[next++]);

            // zaznamy navyse za posledne povodne miesto vlaku
            if (i == last)
            {
                while (next < replacement.Count)
                    result.Add(replacement[next++]);
            }
        }

        while (next < replacement.Count)
            result.Add(replacement[next++]);

        return result.Count == all.Count && result.Zip(all).All(pair => same(pair.First, pair.Second)) ? all : result;
    }
}
