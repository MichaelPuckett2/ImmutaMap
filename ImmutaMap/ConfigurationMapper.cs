namespace ImmutaMap;

internal static class ConfigurationMapper
{
    public static void AddConfig<TSource, TTarget>(Action<IConfiguration<TSource, TTarget>> config)
    {
        var configuration = new Configuration<TSource, TTarget>();
        config.Invoke(configuration);
        ConfigurationCache.Add(configuration);
    }

    //Try Get congifuration from cache
    public static bool TryGetConfig<TSource, TTarget>(out IConfiguration<TSource, TTarget>? config)
    {
        return ConfigurationCache.TryGetConfiguration(out config);
    }
}