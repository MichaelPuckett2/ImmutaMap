namespace ImmutaMap.Filters;

public class IgnoreCaseFilter : IPropertiesFilter
{
    IgnoreCaseFilter() { }
    public static IgnoreCaseFilter Instance { get; } = new();
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
