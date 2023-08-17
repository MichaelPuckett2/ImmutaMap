namespace ImmutaMap;

public static class EnumerationExtensions
{
    public static IEnumerable<(T, T)> EnumerateWith<T>(this IEnumerable<T> enumerable, IEnumerable<T> nextEnumerable) where T : notnull
    {
        var enumerator = enumerable.GetEnumerator();
        var nextEnumerator = nextEnumerable.GetEnumerator();
        while (enumerator.MoveNext() && nextEnumerator.MoveNext())
            yield return (enumerator.Current, nextEnumerator.Current);
    }
}