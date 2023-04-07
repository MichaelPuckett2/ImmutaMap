using ImmutaMap.Exceptions;
using ImmutaMap.Mappings;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using System.Reflection;

namespace ImmutaMap;

public static class Extensions
{
    /// <summary>
    /// A quick update of a Type using an anonymous type to get the values to use.
    /// </summary>
    /// <typeparam name="T">The type this method works against.</typeparam>
    /// <param name="t">The instantiation of the type being mapped from.</param>
    /// <param name="a">The anonymous type used to make the mapping work.</param>
    /// <returns>Instantiated T target value.</returns>
    public static T With<T>(this T t, dynamic a) where T : notnull
    {
        var properties = new List<(string Name, object Value)>();
        foreach (var prop in a.GetType().GetProperties())
        {
            var foundProp = typeof(T).GetProperty(prop.Name);
            if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
        }
        var map = Map<T, T>.Empty;

        foreach (var (Name, Value) in properties) map.Mappings.Add(new DynamicMapping(Value.GetType(), Name, () => Value));
        return MapBuilder.GetNewInstance().Build(map, t);
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
    public static T With<T>(this T t, dynamic a, Action<Map<T, T>> mapAction) where T : notnull
    {
        var map = new Map<T, T>();
        mapAction.Invoke(map);
        var properties = new List<(string Name, object Value)>();
        foreach (var prop in a.GetType().GetProperties())
        {
            var foundProp = typeof(T).GetProperty(prop.Name);
            if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
        }
        foreach (var (Name, Value) in properties) map.Mappings.Add(new DynamicMapping(Value.GetType(), Name, () => Value));
        return MapBuilder.GetNewInstance().Build(map, t);
    }

    public static T With<T>(this T source, Action<Map<T, T>> mapAction)
        where T : notnull
    {
        var map = new Map<T, T>();
        mapAction.Invoke(map);
        return MapBuilder.GetNewInstance().Build(map, source);
    }

    /// <summary>
    /// Maps a type to itself where an expression binding the property to a map and another function is used to perform the mapping logic.
    /// </summary>
    /// <typeparam name="T">The source type being mapped.</typeparam>
    /// <typeparam name="TSourcePropertyType">The source property type being mapped.</typeparam>
    /// <param name="t">The source object this method sets the mapping against.</param>
    /// <param name="sourceExpression">The expression used to get teh source property.</param>
    /// <param name="valueFunc">The function used to get the target value from the source property.</param>
    /// <returns></returns>
    public static T With<T, TSourcePropertyType>(this T t,
                                                 Expression<Func<T, TSourcePropertyType>> sourceExpression,
                                                 Func<TSourcePropertyType, TSourcePropertyType> valueFunc)
        where T : notnull where TSourcePropertyType : notnull
    {
        var mapper = t.BeginMap<T, T>();
        mapper.Configuration.MapPropertyType(sourceExpression, (value) => valueFunc.Invoke(sourceExpression.Compile().Invoke(t))!);
        return mapper.Build();
    }

    /// <summary>
    /// For simple one to one mappings from type to type.
    /// </summary>
    /// <typeparam name="T">The type to map to.</typeparam>
    /// <param name="obj">The obejct this method works against.</param>
    /// <returns>Returns an instantiated T with the values from the object used as reference.</returns>
    public static T As<T>(this object obj) where T : notnull
    {
        var map = Microsoft.Extensions.DependencyInjection.ServiceProviderServiceExtensions.GetService<Map<object, T>>();
        return MapBuilder.GetNewInstance().Build(Map<object, T>.Empty, obj);
    }

    /// <summary>
    /// For simple one to one mappings from type to type.
    /// </summary>
    /// <typeparam name="T">The type to map to.</typeparam>
    /// <param name="source">The obejct this method works against.</param>
    /// <param name="mapAction">Sets mapping configurations inline.</param>
    /// <returns>Returns an instantiated T with the values from the object used as reference.</returns>
    public static TTarget As<TSource, TTarget>(this TSource source, Action<Map<TSource, TTarget>> mapAction)
        where TSource : notnull where TTarget : notnull
    {
        return source.BeginMap(mapAction).Build();
    }

    internal static string GetMemberName(this Expression expression)
    {
        var result = expression switch
        {
            MemberExpression memberExpression => memberExpression.Member.Name,
            MethodCallExpression methodCallExpression => methodCallExpression.Method.Name,
            UnaryExpression unaryExpression => unaryExpression.Operand switch
            {
                MethodCallExpression methodCallExpression => methodCallExpression.Method.Name,
                _ => ((MemberExpression)unaryExpression.Operand).Member.Name
            },
            _ => string.Empty
        };

        if (result == string.Empty)
        {
            try
            {
                result = ((dynamic)expression).Body.Member.Name;
            }
            catch
            {
                throw new ArgumentException(null, nameof(expression));
            }
        }

        return result;
    }

    /// <summary>
    /// Starts the mapping between types. Once mapping is complete call build to get the result or start the Map for another time.
    /// If storing the Map you can call Mapper.GetNewInstance().Build(map) to get the result.
    /// </summary>
    /// <typeparam name="TSource">Type being mapped from.</typeparam>
    /// <typeparam name="TTarget">Type being mapped to.</typeparam>
    /// <param name="source">The source values used during the build process after mapping.</param>
    /// <returns>returns a Map that can be modified, stored, or built for a result.</returns>
    internal static Mapper<TSource, TTarget> BeginMap<TSource, TTarget>(this TSource source)
        where TSource : notnull where TTarget : notnull
    {
        return new Mapper<TSource, TTarget>(source, Map<TSource, TTarget>.Empty);
    }

    /// <summary>
    /// Starts the mapping between types. Once mapping is complete call build to get the result or start the Map for another time.
    /// If storing the Map you can call Mapper.GetNewInstance().Build(map) to get the result.
    /// </summary>
    /// <typeparam name="TSource">Type being mapped from.</typeparam>
    /// <typeparam name="TTarget">Type being mapped to.</typeparam>
    /// <param name="source">The source values used during the build process after mapping.</param>
    /// <param name="mapAction">Sets mapping configurations inline.</param>
    /// <returns>returns a Map that can be modified, stored, or built for a result.</returns>
    internal static Mapper<TSource, TTarget> BeginMap<TSource, TTarget>(this TSource source, Action<Map<TSource, TTarget>> mapAction)
        where TSource : notnull where TTarget : notnull
    {
        var map = new Map<TSource, TTarget>();
        mapAction.Invoke(map);
        return new Mapper<TSource, TTarget>(source, map);
    }
}

public static class ScanExtensions
{
    /// <summary>
    /// Scans the calling assembly and injects specks into the IServiceCollection.
    /// </summary>
    /// <param name="serviceCollection">The IServiceCollection in use.</param>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <returns>The same IServiceCollection in use.</returns>
    public static IServiceCollection AddImmutaMaps(this IServiceCollection serviceCollection)
    {
        Assembly.GetCallingAssembly()
            .GetTypes()
            .ExecuteMapping(type => ScanTypeAndInjectMappings(serviceCollection, type));
        return serviceCollection;
    }

    /// <summary>
    /// Scans the requested assemblies and injects specks into the IServiceCollection.
    /// </summary>
    /// <param name="serviceCollection">The IServiceCollection in use.</param>
    /// <param name="assemblies">The assemblies to scan.</param>
    /// <returns>The same IServiceCollection in use.</returns>
    public static IServiceCollection AddImmutaMaps(this IServiceCollection serviceCollection, IEnumerable<Assembly> assemblies)
    {
        assemblies
            .SelectMany(x => x.GetTypes())
            .ExecuteMapping(type => ScanTypeAndInjectMappings(serviceCollection, type));
        return serviceCollection;
    }

    internal static IServiceCollection WithTypesAs<T>(this IServiceCollection serviceCollection, Type type, Action<T> action) where T : Attribute
    {
        var attributes = type.GetCustomAttributes().OfType<T>();
        foreach (var attribute in attributes) action.Invoke(attribute);
        return serviceCollection;
    }

    internal static void ExecuteMapping<T>(this IEnumerable<T> enumerable, Action<T> action) where T : Type
    {
        foreach (var item in enumerable) action.Invoke(item);
    }

    internal static void ScanTypeAndInjectMappings(IServiceCollection serviceCollection, Type type)
    {
        serviceCollection.WithTypesAs<MapAttribute>(type, x => serviceCollection.AddSingleton(type));

        if (type.IsInterface)
        {
            serviceCollection.WithTypesAs(type, (Action<MapAttribute>)(mapAttribute =>
            {
                ScanPropertiesAndInject(serviceCollection, type);
                ScanFieldsAndInject(serviceCollection, type);
                ScanMethodsAndInject(serviceCollection, type);
            }));
        }
    }

    private static void ScanMethodsAndInject(IServiceCollection serviceCollection, Type type)
    {
        type.GetMethods().Where(x => x.ReturnType != typeof(void)).ToList().ForEach(methodInfo =>
        {
            methodInfo.GetCustomAttributes<MapAttribute>().ToList().ForEach(mapAttribute =>
            {
                _ = mapAttribute switch
                {
                    MapAttribute singletonAttribute => serviceCollection.AddSingleton(methodInfo.ReturnType),
                    _ => throw new ImmutaMapAttributeUnknownException(mapAttribute.GetType())
                };
            });
        });
    }

    private static void ScanFieldsAndInject(IServiceCollection serviceCollection, Type type)
    {
        type.GetFields().ToList().ForEach(fieldInfo =>
        {
            fieldInfo.GetCustomAttributes<MapAttribute>().ToList().ForEach(mapAttribute =>
            {
                _ = mapAttribute switch
                {
                    MapAttribute singletonAttribute => serviceCollection.AddSingleton(fieldInfo.FieldType),
                    _ => throw new ImmutaMapAttributeUnknownException(mapAttribute.GetType())
                };
            });
        });
    }

    private static void ScanPropertiesAndInject(IServiceCollection serviceCollection, Type type)
    {
        type.GetProperties().ToList().ForEach(propertyInfo =>
        {
            propertyInfo.GetCustomAttributes<MapAttribute>().ToList().ForEach(mapAttribute =>
            {
                _ = mapAttribute switch
                {
                    MapAttribute singletonAttribute => serviceCollection.AddSingleton(propertyInfo.PropertyType),
                    _ => throw new ImmutaMapAttributeUnknownException(mapAttribute.GetType())
                };
            });
        });
    }
}