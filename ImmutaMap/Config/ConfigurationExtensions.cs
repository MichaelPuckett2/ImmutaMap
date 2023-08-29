using ImmutaMap.Filters;

namespace ImmutaMap.Config;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds a property name map for cases where property names in the source do not match the name in the target.
    /// </summary>
    /// <typeparam name="TResult">Property result value Type.</typeparam>
    /// <param name="sourceExpression">Expression to gain source property name.</param>
    /// <param name="targetExpression">Expression to gain target property name.</param>
    /// <returns>Current MapConfiguration.</returns>
    public static IConfiguration MapNames<TSource, TTarget>(
       this IConfiguration configuration,
       Expression<Func<TSource, object>> sourceExpression,
       Expression<Func<TTarget, object>> targetExpression)
       where TTarget : notnull where TSource : notnull
    {
        Func<PropertyInfo?, PropertyInfo?, bool> equals = (a, b)
            => a != null && (b == null || a.Name == sourceExpression.GetMemberName() && b.Name == targetExpression.GetMemberName());
        configuration.Comparers.Add(equals);
        return configuration;
    }

    /// <summary>
    /// Maps the source property value to the target with the given expression value.
    /// </summary>
    /// <typeparam name="TProperty">The source property type being mapped.</typeparam>
    /// <param name="property">The expression used to get the source property name and value. Invoked on Build()</param>
    /// <param name="propertyValue">The function used to get the target property value. Invoked on Build()</param>
    /// <returns>Current mapConfiguration.</returns>
    public static IConfiguration TransformType<TSource, TProperty>(
       this IConfiguration configuration,
       Expression<Func<TSource, TProperty>> property,
       Func<TProperty, object> propertyValue)
       where TSource : notnull where TProperty : notnull
    {
        configuration.Transformers.Add(new PropertyTransformer<TProperty>(property.GetMemberName(), propertyValue));
        return configuration;
    }

    /// <summary>
    /// Maps the source properties, containing the given attribute, in a defined way.
    /// </summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="func">The function defined to work on the attribute mapping. Passes the attribute found, the source value, and expects the target value in return.</param>
    /// <returns>Current mapConfiguration.</returns>
    public static IConfiguration TransformSourceOnAttribute<TSource, TTarget, TAttribute>(
       this IConfiguration configuration,
       Func<TAttribute, object, object> func)
       where TTarget : notnull where TSource : notnull
       where TAttribute : Attribute
    {
        var att = new SourceAttributeTransformer<TAttribute>(func);
        configuration.Transformers.Add(att);
        return configuration;
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
    public static IConfiguration TransformTargetOnAttribute<TSource, TTarget, TAttribute>(
       this IConfiguration configuration,
       Func<TAttribute, object, object> func)
       where TTarget : notnull where TSource : notnull
       where TAttribute : Attribute
    {
        var transformer = new TargetAttributeTransformer<TAttribute>(func);
        configuration.Transformers.Add(transformer);
        return configuration;
    }

    /// <summary>
    /// Sets a rule to ignore case when mapping.
    /// </summary>
    /// <returns>Current Configuration</returns>
    public static IConfiguration IgnoreCase(this IConfiguration configuration)
    {
        Func<PropertyInfo?, PropertyInfo?, bool> equals = (a, b) => a?.Name.Equals(b?.Name, StringComparison.InvariantCultureIgnoreCase) ?? b is null;
        configuration.Comparers.Add(equals);
        return configuration;
    }

    /// <summary>
    /// Sets a rule to ignore case when mapping.
    /// </summary>
    /// <returns>Current Configuration</returns>
    public static IConfiguration FilterOut<TSource>(
       this IConfiguration configuration, Expression<Func<TSource, object>> expression)
       where TSource : notnull
    {
        configuration.Filters.Add((SkipPropertyNamesFilter<TSource>)expression);
        return configuration;
    }
}