using System.Dynamic;
using ImmutaMap.Config;

namespace ImmutaMap;

public static class MappingExtensions
{
    /// <summary>
    /// A quick update of a Type using an anonymous type to get the values to use.
    /// </summary>
    /// <typeparam name="T">The type this method works against.</typeparam>
    /// <param name="t">The instantiation of the type being mapped from.</param>
    /// <param name="a">The anonymous type used to make the mapping work.</param>
    /// <returns>Instantiated T target value.</returns>
    public static T With<T>(this T t,
                            dynamic a) where T : notnull
    {
        var configuration = new Configuration();
        foreach (var propertyInfo in (PropertyInfo[])a.GetType().GetProperties()!)
        {
            configuration
                .Transformers
                .Add(new DynamicTransformer(propertyInfo.PropertyType, propertyInfo.Name, () => propertyInfo.GetValue(a)));
        }
        return MapBuilder.GetNewInstance().Build<T, T>(configuration, t);
    }

    public static T With<T>(this T source,
                            Action<IConfiguration> config)
        where T : notnull
    {
        var configuration = new Configuration();
        config(configuration);
        return MapBuilder.GetNewInstance().Build<T, T>(configuration, source);
    }

    /// <summary>
    /// Maps a type to itself where an expression binding the property to a map and another function is used to perform the mapping logic.
    /// </summary>
    /// <typeparam name="TSource">The source type being mapped.</typeparam>
    /// <typeparam name="TPropertyType">The source property type being mapped.</typeparam>
    /// <param name="source">The source object this method sets the mapping against.</param>
    /// <param name="property">The expression used to get the source property.</param>
    /// <param name="targetValue">The function used to get the target value from the source property.</param>
    /// <returns></returns>
    public static TSource With<TSource, TProperty>(this TSource source,
                                                 Expression<Func<TSource, TProperty>> property,
                                                 Func<TProperty, object> targetValue)
        where TSource : notnull where TProperty : notnull
    {
        if (!Utilities.Cache.Configurations.TryGetValue((typeof(TSource), typeof(TProperty)), out IConfiguration? configuration))
        {
            configuration = new Configuration();
            configuration.TransformType(property, (sourceValue) => targetValue.Invoke(property.Compile().Invoke(source))!);
        }
        return MapBuilder.GetNewInstance().Build<TSource, TSource>(configuration, source);
    }

    /// <summary>
    /// Maps a type to itself where an expression binding the property to a map and another function is used to perform the mapping logic.
    /// </summary>
    /// <typeparam name="T">The source type being mapped.</typeparam>
    /// <typeparam name="TSourcePropertyType">The source property type being mapped.</typeparam>
    /// <param name="t">The source object this method sets the mapping against.</param>
    /// <param name="sourceExpression">The expression used to get the source property.</param>
    /// <param name="value">The function used to get the target value from the source property.</param>
    /// <returns></returns>
    public static T With<T, TSourcePropertyType>(this T t,
                                                 Expression<Func<T, TSourcePropertyType>> sourceExpression,
                                                 TSourcePropertyType value)
        where T : notnull where TSourcePropertyType : notnull
    {
        return t.With(sourceExpression, propertyValue => value);
    }

    /// <summary>
    /// For simple one to one mappings from type to type.
    /// </summary>
    /// <typeparam name="T">The type to map to.</typeparam>
    /// <param name="obj">The obejct this method works against.</param>
    /// <returns>Returns an instantiated T with the values from the object used as reference.</returns>
    public static T As<T>(this object obj) where T : notnull
    {
        if (!Utilities.Cache.Configurations.TryGetValue((obj.GetType(), typeof(T)), out IConfiguration? configuration))
        {
            configuration = new Configuration();
        }
        return MapBuilder.GetNewInstance().Build<object, T>(configuration, obj);
    }

    /// <summary>
    /// For simple one to one mappings from type to type.
    /// </summary>
    /// <typeparam name="T">The type to map to.</typeparam>
    /// <param name="source">The obejct this method works against.</param>
    /// <param name="config">Sets mapping configurations inline.</param>
    /// <returns>Returns an instantiated T with the values from the object used as reference.</returns>
    public static TTarget As<TSource, TTarget>(this TSource source,
                                               Action<IConfiguration> config)
        where TSource : notnull where TTarget : notnull
    {
        var configuration = new Configuration();
        config(configuration);
        return MapBuilder.GetNewInstance().Build<TSource, TTarget>(configuration, source);
    }

    public static TSource Cache<TSource, TTarget>(this TSource source,
                                               Action<IConfiguration> config)
        where TSource : notnull where TTarget : notnull
    {
        var key = (typeof(TSource), typeof(TTarget));
        if (Utilities.Cache.Configurations.Keys.Contains(key))
        {
            return source;
        }
        var configuration = new Configuration();
        config(configuration);
        Utilities.Cache.Configurations.Add(key, configuration);
        return source;
    }

    public static dynamic AsDynamic<T>(this T t) where T : notnull
    {
        if (!Utilities.Cache.Configurations.TryGetValue((typeof(T), typeof(ExpandoObject)), out IConfiguration? configuration))
        {
            configuration = new Configuration();
        }
        return new AnonymousMapBuilder().Build(configuration, t);
    }

    public static dynamic AsDynamic<T>(this T t,
                                       Action<IConfiguration> config) where T : notnull
    {
        var configuration = new Configuration();
        config(configuration);
        return new AnonymousMapBuilder().Build(configuration, t);
    }
}
