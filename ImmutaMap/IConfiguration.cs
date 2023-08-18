namespace ImmutaMap;

public interface IConfiguration<TSource, TTarget>
    where TSource : notnull
    where TTarget : notnull
{
    IList<ITransformer> Transformers { get; }
    IList<IPropertiesFilter<TSource, TTarget>> Filters { get; }
    IList<G3EqualityComparer<PropertyInfo>> Comparers { get; }
}