namespace ImmutaMap;

/// <summary>
/// Configurations used for mapping.
/// </summary>
public class Configuration<TSource, TTarget> : IConfiguration<TSource, TTarget>
    where TSource : notnull where TTarget : notnull
{
    public IList<G3EqualityComparer<PropertyInfo>> Comparers { get; } = new List<G3EqualityComparer<PropertyInfo>>
    {
        new G3EqualityComparer<PropertyInfo>((a, b) => a?.Name == b?.Name)
    };

    public IList<IPropertiesFilter<TSource, TTarget>> Filters { get; } = new List<IPropertiesFilter<TSource, TTarget>>();

    public IList<ITransformer> Transformers { get; } = new List<ITransformer>();

    /// <summary>
    /// When true mapping will throw valid exceptions, otherwise they are silently handled and ignored.
    /// </summary>
    public bool WillNotThrowExceptions { get; set; }

    /// <summary>
    /// Static empty Map.
    /// </summary>
    public static Configuration<TSource, TTarget> Empty { get; } = new Configuration<TSource, TTarget>();
}
