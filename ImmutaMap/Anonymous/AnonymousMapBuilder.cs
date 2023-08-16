using System.Dynamic;

namespace ImmutaMap.Anonymous;
public class AnonymousMapBuilder
{
    private const BindingFlags PropertyBindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private bool willNotThrowExceptions;
    private bool ignoreCase;

    /// <summary>
    /// Builds the target value from the source value using the default mappings and any custom mappings put in place.
    /// </summary>
    /// <typeparam name="TSource">The source type mapped from.</typeparam>
    /// <typeparam name="TTarget">The target type mapped to.</typeparam>
    /// <param name="mapper">The Map used to build.</param>
    /// <returns>An instance of the target type with values mapped from the source instance.</returns>
    public dynamic Build<TSource, TTarget>(Configuration<TSource, TTarget> map, TSource source)
        where TSource : notnull where TTarget : notnull
    {
        willNotThrowExceptions = map.WillNotThrowExceptions;
        ignoreCase = map.IgnoreCase;
        var target = InstantiateAnonymous(map, source);
        return target;
    }

    private dynamic InstantiateAnonymous<TSource, TTarget>(Configuration<TSource, TTarget> map, TSource source)
        where TSource : notnull
        where TTarget : notnull
    {
        var sourcePropertyInfos = source.GetType().GetProperties(PropertyBindingFlag);
        dynamic target = new ExpandoObject();
        foreach (var propertyInfo in sourcePropertyInfos.Where(x => !map.SkipPropertyNames.Contains(x.Name)))
        {
            ((IDictionary<string, object?>)target).Add(propertyInfo.Name, propertyInfo.GetValue(source));
        }
        return target;
    }
}
