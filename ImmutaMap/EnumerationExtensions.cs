namespace ImmutaMap;

public static class EnumerationExtensions
{
    /// <summary>
    /// Enumerates two enumerations at the same time untl the first one ends. 
    /// </summary>
    /// <typeparam name="T"><Type of first enumerable type./typeparam>
    /// <typeparam name="TTandem"><Type of second enumerable type./typeparam>
    /// <param name="items">First enumerable.</param>
    /// <param name="tandemItems">Second enumerable</param>
    /// <returns>IEnumerable(TFirst, TSecond)</returns>
    public static IEnumerable<(T, TTandem)> EnumerateWith<T, TTandem>(this IEnumerable<T> items, IEnumerable<TTandem> tandemItems) where T : notnull
    {
        var enumerator = items.GetEnumerator();
        var tandemEnumerator = tandemItems.GetEnumerator();
        while (enumerator.MoveNext() && tandemEnumerator.MoveNext())
            yield return (enumerator.Current, tandemEnumerator.Current);
    }
}