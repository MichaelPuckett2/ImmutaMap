using ImmutaMap.Interfaces;
using ImmutaMap.Mappings;
using System.Linq.Expressions;

namespace ImmutaMap;

/// <summary>
/// Configurations used for mapping.
/// </summary>
public class Map<TSource, TTarget> : IMap<TSource, TTarget> 
    where TSource : notnull where TTarget : notnull
{
    internal List<IMapping> Mappings { get; } = new();
    internal HashSet<(string SourcePropertyName, string TargetPropertyName)> PropertyNameMaps { get; } = new();
    internal HashSet<string> SkipPropertyNames { get; } = new();

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
    /// Adds a property name map for cases where property names in the source do not match the name in the target.
    /// </summary>
    /// <typeparam name="TResult">Property result value Type.</typeparam>
    /// <param name="sourceExpression">Expression to gain source property name.</param>
    /// <param name="targetExpression">Expression to gain target property name.</param>
    /// <returns>Current MapConfiguration.</returns>
    public Map<TSource, TTarget> MapProperty<TResult>(
        Expression<Func<TSource, TResult>> sourceExpression,
        Expression<Func<TTarget, TResult>> targetExpression)
    {
        PropertyNameMaps.Add((sourceExpression.GetMemberName(), targetExpression.GetMemberName()));
        return this;
    }

    /// <summary>
    /// Maps the source property value to the target with the given expression value.
    /// </summary>
    /// <typeparam name="TSourcePropertyType">The source property type being mapped.</typeparam>
    /// <param name="sourceExpression">The expression used to get the source property name and value. Invoked on Build()</param>
    /// <param name="propertyResultFunc">The function used to get the target property value. Invoked on Build()</param>
    /// <returns>Current mapConfiguration.</returns>
    public Map<TSource, TTarget> MapPropertyType<TSourcePropertyType, TTargetPropertyType>(
        Expression<Func<TSource, TSourcePropertyType>> sourceExpression,
        Func<TSourcePropertyType, TTargetPropertyType> propertyResultFunc)
        where TSourcePropertyType : notnull where TTargetPropertyType : notnull
    {
        if (sourceExpression.Body is MemberExpression sourceMemberExpression)
        {
            Mappings.Add(new PropertyMapping<TSourcePropertyType, TTargetPropertyType>(sourceMemberExpression.Member.Name, propertyResultFunc));
        }
        return this;
    }

    /// <summary>
    /// Maps the source properties, containing the given attribute, in a defined way.
    /// </summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="func">The function defined to work on the attribute mapping. Passes the attribute found, the source value, and expects the target value in return.</param>
    /// <returns>Current mapConfiguration.</returns>
    public Map<TSource, TTarget> MapSourceAttribute<TAttribute>(Func<TAttribute, object, object> func)
        where TAttribute : Attribute
    {
        var att = new SourceAttributeMapping<TAttribute>(new Func<Attribute, object, object>((attribute, target) => func.Invoke((TAttribute)attribute, target)));
        Mappings.Add(att);
        return this;
    }

    /// <summary>
    /// Maps the target properties, containing the given attribute, in a defined way.
    /// </summary>
    /// <typeparam name="TSource">The source type.</typeparam>
    /// <typeparam name="TTarget">The target type.</typeparam>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="map">The map used in this method.</param>
    /// <param name="func">The function defined to work on the attribute mapping. Passes the attribute found, the source value, and expects the target value in return.</param>
    /// <returns>Current Map.</returns>
    public Map<TSource, TTarget> MapTargetAttribute<TAttribute>(Func<TAttribute, object, object> func)
        where TAttribute : Attribute
    {
        var att = new TargetAttributeMapping<TAttribute>(new Func<Attribute, object, object>((attribute, target) => func.Invoke((TAttribute)attribute, target)));
        Mappings.Add(att);
        return this;
    }

    /// <summary>
    /// Adds a custom map.
    /// </summary>
    /// <param name="mapping">The custom mapping added.</param>
    /// <returns>Current MapConfiguration.</returns>
    public Map<TSource, TTarget> MapCustom(IMapping mapping)
    {
        Mappings.Add(mapping);
        return this;
    }

    /// <summary>
    /// Maps a type from source property.
    /// </summary>
    /// <typeparam name="TType">The source property type being mapped.</typeparam>
    /// <param name="typeMapFunc">The function used to get the result value.</param>
    /// <returns>Current Map.</returns>
    public Map<TSource, TTarget> MapType<TType>(Func<object, object> typeMapFunc)
    {
        var typeMapping = new SourceTypeMapping(typeof(TType), typeMapFunc);
        Mappings.Add(typeMapping);
        return this;
    }

    /// <summary>
    /// Static empty Map.
    /// </summary>
    public static Map<TSource, TTarget> Empty { get; } = new Map<TSource, TTarget>();
}
