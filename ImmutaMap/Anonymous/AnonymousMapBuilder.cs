using System.Dynamic;
using ImmutaMap.Config;

namespace ImmutaMap.Anonymous;
public class AnonymousMapBuilder
{
    private const BindingFlags PropertyBindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// Builds the target value from the source value using the default configuration and any custom configurations put in place.
    /// </summary>
    /// <typeparam name="TSource">The source type mapped from.</typeparam>
    /// <typeparam name="TTarget">The target type mapped to.</typeparam>
    /// <param name="mapper">The configuration used to build.</param>
    /// <returns>An instance of the target type with values mapped from the source instance.</returns>
    public dynamic Build<TSource>(IConfiguration config, TSource source)
        where TSource : notnull 
    {
        var target = InstantiateAnonymous(config, source);
        return target;
    }

    private dynamic InstantiateAnonymous<TSource>(IConfiguration config, TSource source)
        where TSource : notnull
    {
        dynamic target = new ExpandoObject();
        foreach (var propertyInfo in source.GetType().GetProperties(PropertyBindingFlag))
        {
            ((IDictionary<string, object?>)target).Add(propertyInfo.Name, propertyInfo.GetValue(source));
        }
        return target;
    }
}
