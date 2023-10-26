using ImmutaMap.Builders;

namespace ImmutaMap;

public static partial class TargetExtensions
{
    /// <summary>
    /// A quick update of a Type using an anonymous type to get the values to use.
    /// </summary>
    /// <typeparam name="T">The type this method works against.</typeparam>
    /// <param name="t">The instantiation of the type being mapped from.</param>
    /// <param name="a">The anonymous type used to make the mapping work.</param>
    /// <returns>Instantiated T target value.</returns>
    public static T? With<T>(this T t, dynamic a)
    {
        var properties = new List<(string Name, object Value)>();
        foreach (var prop in a.GetType().GetProperties())
        {
            var foundProp = typeof(T).GetProperty(prop.Name);
            if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
        }
        var configuration = Configuration<T, T>.Empty;
        foreach (var (Name, Value) in properties) configuration.Transformers.Add(new DynamicTransformer(Value.GetType(), Name, () => Value));
        return TargetBuilder.GetNewInstance().Build(configuration, t);
    }

    /// <summary>
    /// A quick update of a Type using an anonymous type to get the values to use.
    /// </summary>
    /// <typeparam name="T">The type this method works against.</typeparam>
    /// <param name="t">The instantiation of the type being mapped from.</param>
    /// <param name="a">The anonymous type used to make the mapping work.</param>
    /// <param name="Map">Map that can be supplied to mapping.</param>
    /// <param name="throwExceptions">Options value that determines if exceptions will be thrown or handled silently.  Default is true to throw exceptoipns.</param>
    /// <returns>Instantiated T target value.</returns>
    public static T? With<T>(this T t, dynamic a, Action<Configuration<T, T>> config)
    {
        var configuration = new Configuration<T, T>();
        config.Invoke(configuration);
        var properties = new List<(string Name, object Value)>();
        foreach (var prop in a.GetType().GetProperties())
        {
            var foundProp = typeof(T).GetProperty(prop.Name);
            if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
        }
        foreach (var (Name, Value) in properties) configuration.Transformers.Add(new DynamicTransformer(Value.GetType(), Name, () => Value));
        return TargetBuilder.GetNewInstance().Build(configuration, t);
    }

    public static T? With<T>(this T source, Action<Configuration<T, T>> config)
        where T : notnull
    {
        var configuration = new Configuration<T, T>();
        config.Invoke(configuration);
        return TargetBuilder.GetNewInstance().Build(configuration, source);
    }

    /// <summary>
    /// Maps a type to itself where an expression binding the property to a map and another function is used to perform the mapping logic.
    /// </summary>
    /// <typeparam name="T">The source type being mapped.</typeparam>
    /// <typeparam name="TSourcePropertyType">The source property type being mapped.</typeparam>
    /// <param name="t">The source object this method sets the mapping against.</param>
    /// <param name="sourceExpression">The expression used to get the source property.</param>
    /// <param name="valueFunc">The function used to get the target value from the source property.</param>
    /// <returns></returns>
    public static T? With<T, TSourcePropertyType>(this T t,
                                                 Expression<Func<T, TSourcePropertyType>> sourceExpression,
                                                 Func<TSourcePropertyType, TSourcePropertyType> valueFunc)
    {
        var configuration = new Configuration<T, T>();
        configuration!.MapPropertyType(sourceExpression, (value) => valueFunc.Invoke(sourceExpression.Compile().Invoke(t))!);
        return TargetBuilder.GetNewInstance().Build(configuration, t); 
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
    public static T? With<T, TSourcePropertyType>(this T t,
                                                 Expression<Func<T, TSourcePropertyType>> sourceExpression,
                                                 TSourcePropertyType value)
    {
        return t.With(sourceExpression, propertyValue => value);
    }

    /// <summary>
    /// For simple one to one mappings from type to type.
    /// </summary>
    /// <typeparam name="T">The type to map to.</typeparam>
    /// <param name="obj">The obejct this method works against.</param>
    /// <returns>Returns an instantiated T with the values from the object used as reference.</returns>
    public static T? To<T>(this object obj)
    {
        return TargetBuilder.GetNewInstance().Build(Configuration<object, T>.Empty, obj);
    }

    /// <summary>
    /// For simple one to one mappings from type to type.
    /// </summary>
    /// <typeparam name="T">The type to map to.</typeparam>
    /// <param name="source">The obejct this method works against.</param>
    /// <param name="config">Sets mapping configurations inline.</param>
    /// <returns>Returns an instantiated T with the values from the object used as reference.</returns>
    public static TTarget? To<TSource, TTarget>(this TSource source, Action<Configuration<TSource, TTarget>> config)
    {
        var configuration = new Configuration<TSource, TTarget>();
        config.Invoke(configuration);
        return TargetBuilder.GetNewInstance().Build(configuration, source);
    }

    public static dynamic ToDynamic<T>(this T t)
    {
        return AnonymousMapBuilder.Build(Configuration<T, dynamic>.Empty, t);
    }

    public static dynamic ToDynamic<T>(this T t, Action<Configuration<T, dynamic>> config)
    {
        var configuration = new Configuration<T, dynamic>();
        config.Invoke(configuration);
        return AnonymousMapBuilder.Build(configuration, t);
    }

    public static void Copy<TSource, TTarget>(this TTarget target, TSource source)
    {
        TargetBuilder.GetNewInstance().ReverseCopy(Configuration<object, TTarget>.Empty, source!, target);
    }

    public static void Copy<TSource, TTarget>(this TTarget target, TSource source, Action<Configuration<TSource, TTarget>> config)
    {
        var configuration = new Configuration<TSource, TTarget>();
        config.Invoke(configuration);
        TargetBuilder.GetNewInstance().ReverseCopy(configuration, source!, target);
    }
}

public static partial class TargetExtensions
{
    public static Task<T?> ToAsync<T>(this object obj)
    {
        return AsyncTargetBuilder.GetNewInstance().BuildAsync(AsyncConfiguration<object, T>.Empty, obj);
    }

    public static Task<TTarget?> ToAsync<TSource, TTarget>(this TSource source, Action<AsyncConfiguration<TSource, TTarget>> config)
    {
        var configuration = new AsyncConfiguration<TSource, TTarget>();
        config.Invoke(configuration);
        return AsyncTargetBuilder.GetNewInstance().BuildAsync(configuration, source);
    }

    public static Task CopyAsync<TSource, TTarget>(this TTarget target, TSource source)
    {
        return AsyncTargetBuilder.GetNewInstance().ReverseCopyAsync(AsyncConfiguration<object, TTarget>.Empty, source!, target);
    }

    public static Task CopyAsync<TSource, TTarget>(this TTarget target, TSource source, Action<AsyncConfiguration<TSource, TTarget>> config)
    {
        var configuration = new AsyncConfiguration<TSource, TTarget>();
        config.Invoke(configuration);
        return AsyncTargetBuilder.GetNewInstance().ReverseCopyAsync(configuration, source!, target);
    }
}
