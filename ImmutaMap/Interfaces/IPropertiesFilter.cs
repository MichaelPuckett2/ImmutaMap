namespace ImmutaMap.Interfaces;

public interface IPropertiesFilter<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    private const BindingFlags PropertyBindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    void Filter(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos);
    public static (IEnumerable<PropertyInfo> SourcePropertyInfos, IEnumerable<PropertyInfo> TargetPropertyInfos) GetDefaultFilter(TSource source, TTarget target)
    {
        return (source.GetType().GetProperties(PropertyBindingFlag), target.GetType().GetProperties(PropertyBindingFlag));
    }
}
