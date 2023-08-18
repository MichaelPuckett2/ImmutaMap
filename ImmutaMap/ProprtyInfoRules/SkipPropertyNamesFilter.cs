using System.Collections.ObjectModel;

namespace ImmutaMap.ProprtyInfoRules;
/// <summary>
/// Properties that will skip mappingp.
/// </summary>
public class SkipPropertyNamesFilter<TSource, TTarget> : IPropertiesFilter<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    SkipPropertyNamesFilter() { }
    public static SkipPropertyNamesFilter<TSource, TTarget> Instance { get; } = new();
    public ICollection<Expression<Func<TSource, TTarget>>> Skips { get; } = new Collection<Expression<Func<TSource, TTarget>>>();

    public void Filter(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos)
    {
        sourcePropertyInfos = sourcePropertyInfos.Where(x => !Skips.Any(xx => xx.GetMemberName() == xx.Name));
        targetPropertyInfos = targetPropertyInfos.Where(x => !Skips.Any(xx => xx.GetMemberName() == xx.Name));
    }
}
