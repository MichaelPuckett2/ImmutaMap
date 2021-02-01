using ImmutaMap.ReflectionTools;
using System;
using System.Collections.Generic;

namespace ImmutaMap
{
    public static class ImmutaMapperExtensions
    {
        public static T With<T>(this T t, dynamic a)
        {
            var properties = new List<(string Name, object Value)>();
            foreach (var prop in a.GetType().GetProperties())
            {
                var foundProp = typeof(T).GetProperty(prop.Name);
                if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
            }

            return ImmutaMapper.Build(mapper =>
            {
                foreach (var (Name, Value) in properties) mapper.WithSourceProperty(Name, () => Value);
            })
            .Map<T>(t);
        }
    }
}
