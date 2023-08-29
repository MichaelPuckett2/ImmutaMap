namespace ImmutaMap.Config;

public interface IConfiguration //: ISourceTargetType
{
    IList<ITransformer> Transformers { get; }
    IList<IPropertiesFilter> Filters { get; }
    IList<G3EqualityComparer<PropertyInfo>> Comparers { get; }
}