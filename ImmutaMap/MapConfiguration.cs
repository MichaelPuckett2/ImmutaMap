using ImmutaMap.Exceptions;
using ImmutaMap.Interfaces;
using ImmutaMap.Mappings;
using System.Linq.Expressions;

namespace ImmutaMap;

/// <summary>
/// Configurations used for mapping.
/// </summary>
public class Map<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    private readonly IList<IMapping> mappings = new List<IMapping>();

    internal IEnumerable<IMapping> Mappings => mappings;
    internal HashSet<(string SourcePropertyName, string TargetPropertyName)> PropertyNameMaps { get; } = new HashSet<(string, string)>();

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
    /// Adds a custom IMapping implementation.
    /// </summary>
    /// <param name="mapping">IMapping implementation.</param>
    public void AddMapping(IMapping mapping) => mappings.Add(mapping);

    /// <summary>
    /// Adds a property name map for cases where property names in the source do not match the name in the target.
    /// </summary>
    /// <typeparam name="TResult">Property result value Type.</typeparam>
    /// <param name="sourceExpression">Expression to gain source property name.</param>
    /// <param name="targetExpression">Expression to gain target property name.</param>
    /// <returns>Current MapConfiguration.</returns>
    public Map<TSource, TTarget> AddPropertyNameMap<TResult>(
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
    public Map<TSource, TTarget> MapProperty<TSourcePropertyType>(
        Expression<Func<TSource, TSourcePropertyType>> sourceExpression,
        Func<TSourcePropertyType, object> propertyResultFunc)
    {
        if (sourceExpression.Body is MemberExpression sourceMemberExpression)
        {
            AddMapping(new PropertyMapping<TSourcePropertyType>(sourceMemberExpression.Member.Name, propertyResultFunc));
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
        AddMapping(att);
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
    /// <returns>The map thsi method works against.</returns>
    public Map<TSource, TTarget> MapTargetAttribute<TAttribute>(Func<TAttribute, object, object> func)
        where TAttribute : Attribute
    {
        var att = new TargetAttributeMapping<TAttribute>(new Func<Attribute, object, object>((attribute, target) => func.Invoke((TAttribute)attribute, target)));
        AddMapping(att);
        return this;
    }

    /// <summary>
    /// Adds a custom map.
    /// </summary>
    /// <param name="mapping">The custom mapping added.</param>
    /// <returns>Current MapConfiguration.</returns>
    public Map<TSource, TTarget> MapCustom(IMapping mapping)
    {
        if (mapping == null) throw new MappingNullException();
        AddMapping(mapping);
        return this;
    }

    /// <summary>
    /// Maps a type from source property.
    /// </summary>
    /// <typeparam name="TType">The source property type being mapped.</typeparam>
    /// <param name="typeMapFunc">The function used to get the result value.</param>
    /// <returns>Current MapConfiguration.</returns>
    public Map<TSource, TTarget> MapType<TType>(Func<object, object> typeMapFunc)
    {
        var typeMapping = new SourceTypeMapping(typeof(TType), typeMapFunc);
        AddMapping(typeMapping);
        return this;
    }

    /// <summary>
    /// Static empty MapConfiguration.
    /// </summary>
    public static Map<TSource, TTarget> Empty { get; } = new Map<TSource, TTarget>();
}

public class Mapper<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    internal Mapper(TSource source, Map<TSource, TTarget> configuration)
    {
        Configuration = configuration;
        Source = source;
    }

    public Map<TSource, TTarget> Configuration { get; }
    public TSource Source { get; }

    /// <summary>
    /// Builds the Map, invoking all mappings added.  Takes the source values and places them in the targets values.
    /// </summary>
    /// <param name="map">The Map this method works against.</param>
    /// <returns>The result of the mapping as an instantiated TTarget Type.</returns>
    public TTarget Build()
    {
        return MapBuilder.GetNewInstance().Build(Configuration, Source);
    }
}