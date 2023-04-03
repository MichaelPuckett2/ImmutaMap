namespace ImmutaMap;

public class Mapper<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    internal Mapper(TSource source, Map<TSource, TTarget> configuration)
    {
        Configuration = configuration;
        Source = source;
    }

    public Map<TSource, TTarget> Configuration { get; }
    public TSource Source { get; }

    /// <summary>
    /// Builds the Map, invoking all mappings added.  Takes the source values and places them in the targets values.
    /// </summary>
    /// <param name="map">The Map this method works against.</param>
    /// <returns>The result of the mapping as an instantiated TTarget Type.</returns>
    public TTarget Build()
    {
        return MapBuilder.GetNewInstance().Build(Configuration, Source);
    }
}