namespace ImmutaMap.ProprtyInfoRules;

public class MatchPropertyNameRule<TSource, TTarget> : IPropertyInfoRule<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    MatchPropertyNameRule() { }
    public static MatchPropertyNameRule<TSource, TTarget> Instance { get; } = new();
    public HashSet<(string SourcePropertyName, string TargetPropertyName)> PropertyNameMaps { get; } = new();

    public void Set(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos)
    {
        foreach (var (sourcePropertyMapName, resultPropertyMapName) in PropertyNameMaps)
        {
            var sourcePropertyInfo = sourcePropertyInfos.FirstOrDefault(x => x.Name == sourcePropertyMapName);
            if (sourcePropertyInfo == null) continue;
            var targetPropertyInfo = targetPropertyInfos.FirstOrDefault(x => x.Name == resultPropertyMapName);
            if (targetPropertyInfo == null) continue;

            if (sourcePropertyInfos.EnumerateWith(targetPropertyInfos).Any(x => x.Item1.Name == sourcePropertyMapName && x.Item2.Name == resultPropertyMapName))
                continue;

            sourcePropertyInfos = sourcePropertyInfos.Where(x => x.Name != sourcePropertyInfo.Name);
            targetPropertyInfos = targetPropertyInfos.Where(x => x.Name != targetPropertyInfo.Name);

            sourcePropertyInfos = sourcePropertyInfos.Append(sourcePropertyInfo);
            targetPropertyInfos = targetPropertyInfos.Append(targetPropertyInfo);
        }
    }
}
