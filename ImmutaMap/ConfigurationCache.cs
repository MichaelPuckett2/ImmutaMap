namespace ImmutaMap;

internal static class ConfigurationCache
{
    private static readonly Dictionary<(Type SourceType, Type TargetType), object> Cache = new();

    public static bool TryGetConfiguration<TSource, TTarget>(out IConfiguration<TSource, TTarget>? configuration)
    {
        var key = (typeof(TSource), typeof(TTarget));
        configuration = Cache.TryGetValue(key, out var value)
                      ? value as IConfiguration<TSource, TTarget>
                      : null;
        return configuration != null;
    }

    public static void Add<TSource, TTarget>(IConfiguration<TSource, TTarget> configuration)
    {
        var key = (typeof(TSource), typeof(TTarget));
        if (!Cache.ContainsKey(key)) Cache.Add(key, configuration);
    }
}
