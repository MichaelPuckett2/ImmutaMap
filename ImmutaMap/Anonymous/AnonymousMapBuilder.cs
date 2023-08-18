using System.Dynamic;

namespace ImmutaMap.Anonymous;
public class AnonymousMapBuilder
{
    private const BindingFlags PropertyBindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;

    /// <summary>
    /// Builds the target value from the source value using the default mappings and any custom mappings put in place.
    /// </summary>
    /// <typeparam name="TSource">The source type mapped from.</typeparam>
    /// <typeparam name="TTarget">The target type mapped to.</typeparam>
    /// <param name="mapper">The Map used to build.</param>
    /// <returns>An instance of the target type with values mapped from the source instance.</returns>
    public dynamic Build<TSource>(IConfiguration<TSource, object> map, TSource source)
        where TSource : notnull 
    {
        var target = InstantiateAnonymous(map, source);
        return target;
    }

    private dynamic InstantiateAnonymous<TSource>(IConfiguration<TSource, object> map, TSource source)
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
