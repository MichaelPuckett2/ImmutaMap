namespace ImmutaMap.Filters;
/// <summary>
/// Properties that will skip mappingp.
/// </summary>
public class SkipPropertyNamesFilter<TSource> : IPropertiesFilter
{
    private readonly Expression<Func<TSource, object>> filterOutFunc;

    public SkipPropertyNamesFilter(Expression<Func<TSource, object>> filterOutFunc)
    {
        this.filterOutFunc = filterOutFunc;
    }

    public void Filter(ref IEnumerable<PropertyInfo> sourcePropertyInfos, ref IEnumerable<PropertyInfo> targetPropertyInfos)
    {
        sourcePropertyInfos = sourcePropertyInfos.Where(x => x.Name == filterOutFunc.GetMemberName());
    }
    public static implicit operator SkipPropertyNamesFilter<TSource>(Expression<Func<TSource, object>> filterOutFunc) => new(filterOutFunc);
}
