namespace ImmutaMap.Config;

/// <summary>
/// Configurations used for mapping.
/// </summary>
public class Configuration : IConfiguration
{
    public IList<G3EqualityComparer<PropertyInfo>> Comparers { get; } = new List<G3EqualityComparer<PropertyInfo>>
    {
        new G3EqualityComparer<PropertyInfo>((a, b) => a?.Name == b?.Name)
    };

    public IList<IPropertiesFilter> Filters { get; } = new List<IPropertiesFilter>();

    public IList<ITransformer> Transformers { get; } = new List<ITransformer>();

    /// <summary>
    /// When true mapping will throw valid exceptions, otherwise they are silently handled and ignored.
    /// </summary>
    public bool WillNotThrowExceptions { get; set; }

    /// <summary>
    /// Static empty Map.
    /// </summary>
    public static Configuration Empty { get; } = new Configuration();
}
