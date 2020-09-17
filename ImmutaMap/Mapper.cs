using System.Collections.Generic;

namespace ImmutaMap
{
    public class Mapper
    {
        public IEnumerable<PropertyMap> Maps { get; } = new List<PropertyMap>();
        public Mapper MapProperty(string sourcePropertyName, string resultPropertyName)
        {
            ((List<PropertyMap>)Maps).Add(new PropertyMap { SourcePropertyName = sourcePropertyName, ResultPropertyName = resultPropertyName });
            return this;
        }
    }
}
