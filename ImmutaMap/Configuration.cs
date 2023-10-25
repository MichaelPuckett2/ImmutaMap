using System.Collections.ObjectModel;

namespace ImmutaMap;

/// <summary>
/// Configurations used for mapping.
/// </summary>
public class Configuration<TSource, TTarget> : IConfiguration<TSource, TTarget>
{
    public HashSet<(string SourcePropertyName, string TargetPropertyName)> PropertyNameMaps { get; } = new();
    public HashSet<string> SkipPropertyNames { get; } = new();

    public ICollection<ITransformer> Transformers { get; } = new Collection<ITransformer>();

    /// <summary>
    /// Properties that will skip mappingp.
    /// </summary>
    public IList<Expression<Func<TSource, TTarget>>> Skips { get; } = new List<Expression<Func<TSource, TTarget>>>();

    /// <summary>
    /// When true mapping will ignore property casing.
    /// </summary>
    public bool IgnoreCase { get; set; }

    /// <summary>
    /// When true mapping will throw valid exceptions, otherwise they are silently handled and ignored.
    /// </summary>
    public bool WillNotThrowExceptions { get; set; }

    /// <summary>
    /// Static empty Map.
    /// </summary>
    public static IConfiguration<TSource, TTarget> Empty { get; } = new Configuration<TSource, TTarget>();
}
