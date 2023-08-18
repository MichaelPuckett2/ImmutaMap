namespace ImmutaMap.ProprtyInfoRules;

public class IgnoreCaseFilter<TSource, TTarget> : IPropertiesFilter<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    IgnoreCaseFilter() { }
    public static IgnoreCaseFilter<TSource, TTarget> Instance { get; } = new();
    public void Filter(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos)
    {
        var joined = sourcePropertyInfos.Join(targetPropertyInfos,
            sourceProperty => sourceProperty.Name.ToLowerInvariant(),
            resultProperty => resultProperty.Name.ToLowerInvariant(),
            (sourceProperty, targetProperty) => (sourceProperty, targetProperty));
        sourcePropertyInfos = joined.Select(x => x.sourceProperty);
        targetPropertyInfos = joined.Select(x => x.targetProperty);
    }
}
