namespace ImmutaMap;

public interface IConfiguration<TSource, TTarget>
    where TSource : notnull
    where TTarget : notnull
{
    IEnumerable<ITransformer> Transformers { get; }
    IEnumerable<IPropertyInfoRule<TSource, TTarget>> Rules { get; }
    void AddTransformer(ITransformer transformer);
    void AddRule(IPropertyInfoRule<TSource, TTarget> rule);
    bool WillNotThrowExceptions { get; set; }
}