using System.Collections.ObjectModel;

namespace ImmutaMap;

/// <summary>
/// Configuration used for building the target.
/// </summary>
public class Configuration<TSource, TTarget> : IConfiguration<TSource, TTarget>, ITransform
{
    public HashSet<(string SourcePropertyName, string TargetPropertyName)> PropertyNameMaps { get; } = new();
    public HashSet<string> SkipPropertyNames { get; } = new();


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

    public ICollection<ITransformer> Transformers { get; } = new Collection<ITransformer>();

    /// <summary>
    /// Static empty Map.
    /// </summary>
    public static Configuration<TSource, TTarget> Empty { get; } = new Configuration<TSource, TTarget>();
}

/// <summary>
/// Configuration used for building the target.
/// </summary>
public class AsyncConfiguration<TSource, TTarget> : IConfiguration<TSource, TTarget>, ITransformAsync
{
    public HashSet<(string SourcePropertyName, string TargetPropertyName)> PropertyNameMaps { get; } = new();
    public HashSet<string> SkipPropertyNames { get; } = new();

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

    public ICollection<IAsyncTransformer> AsyncTransformers { get; } = new Collection<IAsyncTransformer>();

    /// <summary>
    /// Static empty Map.
    /// </summary>
    public static AsyncConfiguration<TSource, TTarget> Empty { get; } = new AsyncConfiguration<TSource, TTarget>();
}