namespace ImmutaMap.ProprtyInfoRules;
/// <summary>
/// Properties that will skip mappingp.
/// </summary>
public class SkipPropertyNamesFilter<TSource, TTarget> : IPropertiesFilter<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    private readonly Expression<Func<TSource, TTarget>> filterOutFunc;

    public SkipPropertyNamesFilter(Expression<Func<TSource, TTarget>> filterOutFunc)
    {
        this.filterOutFunc = filterOutFunc;
    }

    public void Filter(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos)
    {
        sourcePropertyInfos = sourcePropertyInfos.Where(x => x.Name == filterOutFunc.GetMemberName());
    }
    public static implicit operator SkipPropertyNamesFilter<TSource, TTarget>(Expression<Func<TSource, TTarget>> filterOutFunc) => new(filterOutFunc);
}
