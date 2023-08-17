namespace ImmutaMap.Interfaces;

public interface IPropertyInfoRule<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    private const BindingFlags PropertyBindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    void Set(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos);
    public static (IEnumerable<PropertyInfo> SourcePropertyInfos, IEnumerable<PropertyInfo> TargetPropertyInfos) GetDefaultRule(TSource source, TTarget target)
    {
        return (source.GetType().GetProperties(PropertyBindingFlag).ToList(), target.GetType().GetProperties(PropertyBindingFlag).ToList());
    }
}
