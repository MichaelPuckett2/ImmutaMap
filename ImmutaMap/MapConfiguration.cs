using System.Linq.Expressions;

namespace ImmutaMap;

/// <summary>
/// Configurations used for mapping.
/// </summary>
public class MapConfiguration<T>
{
    /// <summary>
    /// Properties that will skip mappingp.
    /// </summary>
    public IList<Expression<Func<T, object>>> Skips { get; } = new List<Expression<Func<T, object>>>();

    /// <summary>
    /// When true mapping will ignore property casing.
    /// </summary>
    public bool IgnoreCase { get; set; }

    /// <summary>
    /// When true mapping will throw valid exceptions, otherwise they are silently handled and ignored.
    /// </summary>
    public bool WillNotThrowExceptions { get; set; }

    internal HashSet<(string SourcePropertyName, string TargetPropertyName)> PropertyNameMaps { get; } = new HashSet<(string, string)>();

    public MapConfiguration<T> AddPropertyNameMap<TSource, TResult>(
        Expression<Func<TSource, object>> sourceExpression,
        Expression<Func<TResult, object>> targetExpression)
    {
        PropertyNameMaps.Add((sourceExpression.GetMemberName(), targetExpression.GetMemberName()));
        return this;
    }
    /// <summary>
    /// Static empty MapConfiguration.
    /// </summary>
    public static MapConfiguration<T> Empty { get; } = new MapConfiguration<T>();
}
