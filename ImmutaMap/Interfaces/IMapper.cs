using System.Collections.Generic;

namespace ImmutaMap.Interfaces
{
    public interface IMapper
    {
        IEnumerable<PropertyMap> Maps { get; }
        Mapper MapProperty(string sourcePropertyName, string resultPropertyName);
    }
}
