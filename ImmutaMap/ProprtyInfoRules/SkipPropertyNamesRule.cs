using System.Collections.ObjectModel;

namespace ImmutaMap.ProprtyInfoRules;
/// <summary>
/// Properties that will skip mappingp.
/// </summary>
public class SkipPropertyNamesRule<TSource, TTarget> : IPropertyInfoRule<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    SkipPropertyNamesRule() { }
    public static SkipPropertyNamesRule<TSource, TTarget> Instance { get; } = new();
    public ICollection<Expression<Func<TSource, TTarget>>> Skips { get; } = new Collection<Expression<Func<TSource, TTarget>>>();

    public void Set(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos)
    {
        sourcePropertyInfos = sourcePropertyInfos.Where(x => !Skips.Any(xx => xx.GetMemberName() == xx.Name));
        targetPropertyInfos = targetPropertyInfos.Where(x => !Skips.Any(xx => xx.GetMemberName() == xx.Name));
    }
}
