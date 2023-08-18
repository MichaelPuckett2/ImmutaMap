namespace ImmutaMap;

public record IgnoreHash<T>(T Item)
{
    public override int GetHashCode() => 0;
    public static implicit operator IgnoreHash<T>(T item) => new(item);
    public static implicit operator T(IgnoreHash<T> ignoreHash) => ignoreHash.Item;
}