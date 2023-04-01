using System.Linq.Expressions;

namespace ImmutaMap;

/// <summary>
/// Configurations used for mapping.
/// </summary>
public class MapConfiguration<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
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

    internal HashSet<(string SourcePropertyName, string TargetPropertyName)> PropertyNameMaps { get; } = new HashSet<(string, string)>();

    public MapConfiguration<TSource, TTarget> AddPropertyNameMap<TResult>(
        Expression<Func<TSource, TResult>> sourceExpression,
        Expression<Func<TTarget, TResult>> targetExpression)
    {
        PropertyNameMaps.Add((sourceExpression.GetMemberName(), targetExpression.GetMemberName()));
        return this;
    }
    /// <summary>
    /// Static empty MapConfiguration.
    /// </summary>
    public static MapConfiguration<TSource, TTarget> Empty { get; } = new MapConfiguration<TSource, TTarget>();
}
