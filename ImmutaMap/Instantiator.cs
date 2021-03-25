using ImmutaMap.Mappings;
using ImmutaMap.Utilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ImmutaMap
{
    public static class Instantiator
    {
        public static T New<T>(dynamic a, bool throwExceptions = true) where T : class
        {
            var target = new TypeFormatter().GetInstance<T>();
            var properties = new List<(string Name, object Value)>();
            foreach (var prop in a.GetType().GetProperties())
            {
                var foundProp = typeof(T).GetProperty(prop.Name);
                if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
            }
            var map = new Map<T, T>(throwExceptions: throwExceptions);

            foreach (var (Name, Value) in properties) map.AddMapping(new DynamicMapping(Value.GetType(), Name, () => Value));
            return MapBuilder.GetNewInstance().Build(map, target);
        }
    }
}
