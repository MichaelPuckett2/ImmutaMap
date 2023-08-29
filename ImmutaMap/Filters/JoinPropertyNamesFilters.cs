namespace ImmutaMap.Filters;

public class JoinPropertyNamesFilters : IPropertiesFilter
{
    JoinPropertyNamesFilters() { }
    public static JoinPropertyNamesFilters Instance { get; } = new();
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
