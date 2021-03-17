using ImmutaMap.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;

namespace ImmutaMap
{
    public class Map<TSource, TTarget>
    {
        private readonly List<(string SourceProperty, string ResultProperty)> propertyNameMaps = new List<(string SourceProperty, string ResultProperty)>();
        private readonly IList<IMapping> mappings = new List<IMapping>();

        public Map(bool ignoreCase = false, bool throwExceptions = true) : this(default, ignoreCase, throwExceptions) { }
        public Map(TSource source, bool ignoreCase = false, bool throwExceptions = true)
        {
            Source = source;
            IsIgnoringCase = ignoreCase;
            IsThrowingExceptions = throwExceptions;
        }

        public void AddMapping(IMapping mapping) => mappings.Add(mapping);
        internal IEnumerable<IMapping> Mappings => mappings;

        internal IEnumerable<(string SourceProperty, string ResultProperty)> PropertyNameMaps => propertyNameMaps.ToList();

        public TSource Source { get; internal set; }

        /// <summary>
        /// When true the MapBuilder will ignore property case sensitivity while mapping the types.
        /// </summary>
        public bool IsIgnoringCase { get; }

        /// <summary>
        /// Is set to true the MapBuilder will ignore exceptions, and map only the properties that it can.
        /// If false, MapBuilder will throw an exception if the property cannot be mapped.
        /// </summary>
        public bool IsThrowingExceptions { get; }

        public void AddPropertyNameMapping(string sourcePropertyName, string targetPropertyName)
        {
            propertyNameMaps.Add((sourcePropertyName, targetPropertyName));
        }
    }
}
