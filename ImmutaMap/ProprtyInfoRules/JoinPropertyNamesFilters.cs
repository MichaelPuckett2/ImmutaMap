namespace ImmutaMap.ProprtyInfoRules;

public class JoinPropertyNamesFilters<TSource, TTarget> : IPropertiesFilter<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    JoinPropertyNamesFilters() { }
    public static JoinPropertyNamesFilters<TSource, TTarget> Instance { get; } = new();
    public void Filter(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos)
    {
        var joined = sourcePropertyInfos.Join(targetPropertyInfos,
            sourceProperty => sourceProperty.Name,
            resultProperty => resultProperty.Name,
            (sourceProperty, targetProperty) => (sourceProperty, targetProperty));
        sourcePropertyInfos = joined.Select(x => x.sourceProperty);
        targetPropertyInfos = joined.Select(x => x.targetProperty);
    }
}
