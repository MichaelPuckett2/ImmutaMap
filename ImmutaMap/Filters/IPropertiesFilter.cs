namespace ImmutaMap.Filters;

public interface IPropertiesFilter
{
    private const BindingFlags PropertyBindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    void Filter(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos);
    public static (IEnumerable<PropertyInfo> SourcePropertyInfos, IEnumerable<PropertyInfo> TargetPropertyInfos) GetDefaultFilter(Type source, Type target)
    {
        return (source.GetProperties(PropertyBindingFlag), target.GetProperties(PropertyBindingFlag));
    }
}

//public interface IPropertiesFilter : ISourceTargetType
//{
//    private const BindingFlags PropertyBindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
//    void Filter(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos);
//}
