using ImmutaMap.Interfaces;

namespace ImmutaMap;

public class Map<TSource, TTarget> where TSource : notnull where TTarget : notnull
{
    private readonly List<(string SourceProperty, string ResultProperty)> propertyNameMaps = new();
    private readonly IList<IMapping> mappings = new List<IMapping>();

    public Map(TSource source, MapConfiguration<TSource, TTarget> mapConfiguration) 
    {
        Source = source;
        MapConfiguration = mapConfiguration;
        IgnoringCase = mapConfiguration.IgnoreCase;
        WillNotThrowExceptions = mapConfiguration.WillNotThrowExceptions;
    }

    public void AddMapping(IMapping mapping) => mappings.Add(mapping);
    internal IEnumerable<IMapping> Mappings => mappings;

    internal IEnumerable<(string SourceProperty, string ResultProperty)> PropertyNameMaps => propertyNameMaps.ToList();
    internal MapConfiguration<TSource, TTarget> MapConfiguration { get; }

    public TSource Source { get; internal set; }

    /// <summary>
    /// When true the MapBuilder will ignore property case sensitivity while mapping the types.
    /// </summary>
    public bool IgnoringCase { get; }

    /// <summary>
    /// When true mapping will throw exceptions on properties that cannot be mapped, otherwise they are silently skipped and mapping continues.
    /// </summary>
    public bool WillNotThrowExceptions { get; }

    internal void AddPropertyNameMapping(string sourcePropertyName, string targetPropertyName)
    {
        propertyNameMaps.Add((sourcePropertyName, targetPropertyName));
    }
}
