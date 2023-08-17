namespace ImmutaMap.ProprtyInfoRules;

public class IgnoreCaseRule<TSource, TTarget> : IPropertyInfoRule<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    IgnoreCaseRule() { }
    public static IgnoreCaseRule<TSource, TTarget> Instance { get; } = new();
    public void Set(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos)
    {
        var joined = sourcePropertyInfos.Join(targetPropertyInfos,
            sourceProperty => sourceProperty.Name.ToLowerInvariant(),
            resultProperty => resultProperty.Name.ToLowerInvariant(),
            (sourceProperty, targetProperty) => (sourceProperty, targetProperty));
        sourcePropertyInfos = joined.Select(x => x.sourceProperty);
        targetPropertyInfos = joined.Select(x => x.targetProperty);
    }
}
