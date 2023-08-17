namespace ImmutaMap.ProprtyInfoRules;

public class JoinPropertyNamesRule<TSource, TTarget> : IPropertyInfoRule<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    JoinPropertyNamesRule() { }
    public static JoinPropertyNamesRule<TSource, TTarget> Instance { get; } = new();
    public void Set(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos)
    {
        var joined = sourcePropertyInfos.Join(targetPropertyInfos,
            sourceProperty => sourceProperty.Name,
            resultProperty => resultProperty.Name,
            (sourceProperty, targetProperty) => (sourceProperty, targetProperty));
        sourcePropertyInfos = joined.Select(x => x.sourceProperty);
        targetPropertyInfos = joined.Select(x => x.targetProperty);
    }
}
