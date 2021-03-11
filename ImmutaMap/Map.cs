using System;
using System.Collections.Generic;
using System.Linq;

namespace ImmutaMap
{
    public class Map<TSource, TTarget>
    {
        private readonly List<(string SourceProperty, string ResultProperty)> propertyNameMaps = new List<(string SourceProperty, string ResultProperty)>();
        private readonly IDictionary<(string, Type), Func<object, object>> propertyMapFuncs = new Dictionary<(string, Type), Func<object, object>>();
        public Map(TSource source) : this(source, true) { }
        public Map(TSource source, bool useRecursion)
        {
            Source = source;
            UseRecursion = useRecursion;
        }

        internal IEnumerable<(string SourceProperty, string ResultProperty)> PropertyNameMaps => propertyNameMaps.ToList();
        internal IDictionary<(string, Type), Func<object, object>> PropertyMapFuncs => propertyMapFuncs;

        public TSource Source { get; }
        public bool UseRecursion { get; }

        public void MapPropertyName(string sourcePropertyName, string targetPropertyName)
        {
            propertyNameMaps.Add((sourcePropertyName, targetPropertyName));
        }

        public void MapProperty<TSourcePropertyType>(string name, Func<TSourcePropertyType, object> propertyResultFunc)
        {
            var key = (name, typeof(TSourcePropertyType));
            propertyMapFuncs.Add(key, new Func<object, object>(sourceValue => propertyResultFunc.Invoke((TSourcePropertyType)sourceValue)));
        }

        public Map<TSource, TTarget> MapDynamicProperty(Type type, string propertyName, Func<object> propertyResultFunc)
        {
            var key = (propertyName, type);
            propertyMapFuncs.Add(key, new Func<object, object>(sourceValue => propertyResultFunc.Invoke()));
            return this;
        }
    }
}
