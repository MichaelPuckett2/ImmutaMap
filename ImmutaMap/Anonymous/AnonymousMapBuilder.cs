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
    public static dynamic Build<TSource, TTarget>(IConfiguration<TSource, TTarget> map, TSource source)
    {
        var target = AnonymousMapBuilder.InstantiateAnonymous(map, source);
        return target;
    }

    private static dynamic InstantiateAnonymous<TSource, TTarget>(IConfiguration<TSource, TTarget> map, TSource source)
    {
        var sourcePropertyInfos = source == null
            ? typeof(TSource).GetType().GetProperties(PropertyBindingFlag)
            : source.GetType().GetProperties(PropertyBindingFlag);
        dynamic target = new ExpandoObject();
        foreach (var propertyInfo in sourcePropertyInfos.Where(x => !map.SkipPropertyNames.Contains(x.Name)))
        {
            ((IDictionary<string, object?>)target).Add(propertyInfo.Name, propertyInfo.GetValue(source));
        }
        return target;
    }
}
