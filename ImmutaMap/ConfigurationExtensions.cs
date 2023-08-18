namespace ImmutaMap;

public static class ConfigurationExtensions
{
    /// <summary>
    /// Adds a property name map for cases where property names in the source do not match the name in the target.
    /// </summary>
    /// <typeparam name="TResult">Property result value Type.</typeparam>
    /// <param name="sourceExpression">Expression to gain source property name.</param>
    /// <param name="targetExpression">Expression to gain target property name.</param>
    /// <returns>Current MapConfiguration.</returns>
    public static IConfiguration<TSource, TTarget> MapNames<TSource, TTarget, TResult>(
       this IConfiguration<TSource, TTarget> configuration,
       Expression<Func<TSource, TResult>> sourceExpression,
       Expression<Func<TTarget, TResult>> targetExpression)
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
    /// <typeparam name="TSourcePropertyType">The source property type being mapped.</typeparam>
    /// <param name="sourceExpression">The expression used to get the source property name and value. Invoked on Build()</param>
    /// <param name="propertyResultFunc">The function used to get the target property value. Invoked on Build()</param>
    /// <returns>Current mapConfiguration.</returns>
    public static IConfiguration<TSource, TTarget> TransformType<TSource, TTarget, TSourcePropertyType, TTargetPropertyType>(
       this IConfiguration<TSource, TTarget> configuration,
       Expression<Func<TSource, TSourcePropertyType>> sourceExpression,
       Func<TSourcePropertyType, TTargetPropertyType> propertyResultFunc)
       where TTarget : notnull where TSource : notnull
       where TSourcePropertyType : notnull where TTargetPropertyType : notnull
    {
        configuration.Transformers.Add(new PropertyTransformer<TSourcePropertyType, TTargetPropertyType>(sourceExpression.GetMemberName(), propertyResultFunc));
        return configuration;
    }

    /// <summary>
    /// Maps the source properties, containing the given attribute, in a defined way.
    /// </summary>
    /// <typeparam name="TAttribute">The attribute type.</typeparam>
    /// <param name="func">The function defined to work on the attribute mapping. Passes the attribute found, the source value, and expects the target value in return.</param>
    /// <returns>Current mapConfiguration.</returns>
    public static IConfiguration<TSource, TTarget> TransformSourceOnAttribute<TSource, TTarget, TAttribute>(
       this IConfiguration<TSource, TTarget> configuration,
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
    public static IConfiguration<TSource, TTarget> TransformTargetOnAttribute<TSource, TTarget, TAttribute>(
       this IConfiguration<TSource, TTarget> configuration,
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
    public static IConfiguration<TSource, TTarget> IgnoreCase<TSource, TTarget>(
       this IConfiguration<TSource, TTarget> configuration)
       where TTarget : notnull where TSource : notnull
    {
        Func<PropertyInfo?, PropertyInfo?, bool> equals = (a, b) => a?.Name.Equals(b?.Name, StringComparison.InvariantCultureIgnoreCase) ?? b is null;
        configuration.Comparers.Add(equals);
        return configuration;
    }

    /// <summary>
    /// Sets a rule to ignore case when mapping.
    /// </summary>
    /// <returns>Current Configuration</returns>
    public static IConfiguration<TSource, TTarget> FilterOut<TSource, TTarget>(
       this IConfiguration<TSource, TTarget> configuration, Expression<Func<TSource, TTarget>> expression)
       where TTarget : notnull where TSource : notnull
    {
        configuration.Filters.Add((SkipPropertyNamesFilter<TSource, TTarget>)expression);
        return configuration;
    }
}