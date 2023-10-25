namespace ImmutaMap;

public interface IConfiguration<TSource, TTarget>
{
    bool IgnoreCase { get; set; }
    HashSet<(string SourcePropertyName, string TargetPropertyName)> PropertyNameMaps { get; }
    HashSet<string> SkipPropertyNames { get; }
    IList<Expression<Func<TSource, TTarget>>> Skips { get; }
    ICollection<ITransformer> Transformers { get; }
    bool WillNotThrowExceptions { get; set; }
}

public interface IAsyncConfiguration<TSource, TTarget> : IConfiguration<TSource, TTarget>
{    
    ICollection<IAsyncTransformer> AsyncTransformers { get; }
}