using ImmutaMap.Config;

namespace ImmutaMap.Utilities;
internal static class Cache
{
    internal static IDictionary<(Type Source, Type Target), IConfiguration> Configurations { get; } = new Dictionary<(Type Source, Type Target), IConfiguration>();
}
