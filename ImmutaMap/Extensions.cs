using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ImmutaMap
{
    public static class Extensions
    {
        public static T With<T, TSourcePropertyType>(this T t, Expression<Func<T, TSourcePropertyType>> sourceExpression, Func<TSourcePropertyType, TSourcePropertyType> valueFunc)
        {
            var mapper = Mapper.GetNewInstance();
            var map = mapper.Map<T, T>(t).MapProperty(sourceExpression, (value) => valueFunc.Invoke(sourceExpression.Compile().Invoke(t)));
            return mapper.Build(map);
        }

        public static T With<T>(this T t, dynamic a)
        {
            var properties = new List<(string Name, object Value)>();
            foreach (var prop in a.GetType().GetProperties())
            {
                var foundProp = typeof(T).GetProperty(prop.Name);
                if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
            }

            var mapper = Mapper.GetNewInstance();
            var map = mapper.Map<T, T>(t);
            foreach (var (Name, Value) in properties) map.MapDynamicProperty(Value.GetType(), Name, () => Value);
            return mapper.Build(map);
        }
    }
}
