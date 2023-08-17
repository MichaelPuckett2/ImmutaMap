namespace ImmutaMap;

/// <summary>
/// Configurations used for mapping.
/// </summary>
public class Configuration<TSource, TTarget> : IConfiguration<TSource, TTarget>
    where TSource : notnull where TTarget : notnull
{
    private ImmutableList<ITransformer> transformers = ImmutableList.Create<ITransformer>();
    private ImmutableList<IPropertyInfoRule<TSource, TTarget>> rules 
        = ImmutableList.Create<IPropertyInfoRule<TSource, TTarget>>(SkipPropertyNamesRule<TSource, TTarget>.Instance,
                                                                    JoinPropertyNamesRule<TSource, TTarget>.Instance,
                                                                    MatchPropertyNameRule<TSource, TTarget>.Instance);

    public IEnumerable<IPropertyInfoRule<TSource, TTarget>> Rules => rules;
    public IEnumerable<ITransformer> Transformers => transformers;

    public void AddTransformer(ITransformer transformer)
    {
        transformers = transformers.Add(transformer);
    }

    public void AddRule(IPropertyInfoRule<TSource, TTarget> rule)
    {
        if (rules.Contains(rule)) return;
        rules = rules.Add(rule);
    }

    /// <summary>
    /// When true mapping will throw valid exceptions, otherwise they are silently handled and ignored.
    /// </summary>
    public bool WillNotThrowExceptions { get; set; }

    /// <summary>
    /// Static empty Map.
    /// </summary>
    public static Configuration<TSource, TTarget> Empty { get; } = new Configuration<TSource, TTarget>();
}
