namespace ImmutaMap.Utilities;

public record Boolean<T>(in T Item, bool BooleanValue)
{
    public static implicit operator T(Boolean<T> boolItem) => boolItem.Item;
    public static implicit operator bool(Boolean<T> boolItem) => boolItem.BooleanValue;
    public static implicit operator Boolean<T>((T, bool) tuple) => new(tuple.Item1, tuple.Item2);
}
