namespace ImmutaMap.ProprtyInfoRules;

public class MatchPropertyNameFilter<TSource, TTarget> : IPropertiesFilter<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    MatchPropertyNameFilter() { }
    public static MatchPropertyNameFilter<TSource, TTarget> Instance { get; } = new();
    public HashSet<(string SourceName, string TargetName)> SourceNameToTargetNames { get; } = new();

    public void Filter(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos)
    {
        foreach (var (SourceName, PropertyName) in SourceNameToTargetNames)
        {
            var sourcePropertyInfo = sourcePropertyInfos.FirstOrDefault(x => x.Name == SourceName);
            if (sourcePropertyInfo == null) continue;
            var targetPropertyInfo = targetPropertyInfos.FirstOrDefault(x => x.Name == PropertyName);
            if (targetPropertyInfo == null) continue;

            if (sourcePropertyInfos.EnumerateWith(targetPropertyInfos).Any(x => x.Item1.Name == SourceName && x.Item2.Name == PropertyName))
                continue;

            sourcePropertyInfos = sourcePropertyInfos.Where(x => x.Name != sourcePropertyInfo.Name);
            targetPropertyInfos = targetPropertyInfos.Where(x => x.Name != targetPropertyInfo.Name);

            sourcePropertyInfos = sourcePropertyInfos.Append(sourcePropertyInfo);
            targetPropertyInfos = targetPropertyInfos.Append(targetPropertyInfo);
        }
    }
}
