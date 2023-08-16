namespace ImmutaMap;

public interface IConfiguration<TSource, TTarget>
    where TSource : notnull
    where TTarget : notnull
{
    bool IgnoreCase { get; set; }
    HashSet<(string SourcePropertyName, string TargetPropertyName)> PropertyNameMaps { get; }
    HashSet<string> SkipPropertyNames { get; }
    IList<Expression<Func<TSource, TTarget>>> Skips { get; }
    ICollection<ITransformer> Transformers { get; }
    bool WillNotThrowExceptions { get; set; }
}