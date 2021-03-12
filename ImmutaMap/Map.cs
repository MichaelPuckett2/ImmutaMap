using ImmutaMap.Interfaces;
using System.Collections.Generic;
using System.Linq;

namespace ImmutaMap
{
    public class Map<TSource, TTarget>
    {
        private readonly List<(string SourceProperty, string ResultProperty)> propertyNameMaps = new List<(string SourceProperty, string ResultProperty)>();
        private readonly IList<IMapping> mappings = new List<IMapping>();

        public Map() : this(default) { }
        public Map(TSource source) => Source = source;

        public void AddMapping(IMapping mapping) => mappings.Add(mapping);
        internal IEnumerable<IMapping> Mappings => mappings;

        internal IEnumerable<(string SourceProperty, string ResultProperty)> PropertyNameMaps => propertyNameMaps.ToList();

        public TSource Source { get; internal set; }

        public void AddPropertyNameMapping(string sourcePropertyName, string targetPropertyName)
        {
            propertyNameMaps.Add((sourcePropertyName, targetPropertyName));
        }
    }
}
