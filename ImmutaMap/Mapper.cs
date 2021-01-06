using ImmutaMap.Interfaces;
using System;
using System.Collections.Generic;

namespace ImmutaMap
{
    public class Mapper : IMapper
    {
        private readonly IDictionary<Type, Func<Attribute, object, object>> attributeFunctions = new Dictionary<Type, Func<Attribute, object, object>>();
        private readonly IDictionary<string, Func<string, object, object>> sourcePropertyFunctions = new Dictionary<string, Func<string, object, object>>();
        private readonly IList<PropertyMap> maps = new List<PropertyMap>();

        public IEnumerable<PropertyMap> Maps => maps;
        public IDictionary<Type, Func<Attribute, object, object>> AttributeFunctions => attributeFunctions;
        public IDictionary<string, Func<string, object, object>> SourcePropertyFunctions => sourcePropertyFunctions;

        public Mapper MapProperty(string sourcePropertyName, string resultPropertyName)
        {
            maps.Add(new PropertyMap { SourcePropertyName = sourcePropertyName, ResultPropertyName = resultPropertyName });
            return this;
        }

        public Mapper WithAttribute<T>(Func<T, object, object> func) where T : Attribute
        {
            attributeFunctions.Add(typeof(T), new Func<Attribute, object, object>((attribute, target) => func.Invoke((T)attribute, target)));
            return this;
        }

        public Mapper WithSourceProperty(string propertyName, Func<string, object, object> func)
        {
            sourcePropertyFunctions.Add(propertyName, new Func<string, object, object>((property, target) => func.Invoke(property, target)));
            return this;
        }
    }
}
