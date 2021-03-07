using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ImmutaMap
{
    public static class Extensions
    {
        public static Map<TSource, TTarget> Map<TSource, TTarget, TSourceProperty>(this Map<TSource, TTarget> map, Expression<Func<TSource, TSourceProperty>> sourceExpression, Func<TSourceProperty, TSourceProperty> valueFunc)
        {
            map.MapProperty(sourceExpression, (value) => valueFunc.Invoke(sourceExpression.Compile().Invoke(map.Source)));
            return map;
        }

        public static Map<T, TSourceProperty> Map<T, TSourceProperty>(this Map<T, TSourceProperty> map, dynamic a)
        {
            var properties = new List<(string Name, object Value)>();
            foreach (var prop in a.GetType().GetProperties())
            {
                var foundProp = typeof(T).GetProperty(prop.Name);
                if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
            }

            foreach (var (Name, Value) in properties) map.MapDynamicProperty(Value.GetType(), Name, () => Value);
            return map;
        }

        public static Map<T, T> Map<T, TSourcePropertyType>(this T t, Expression<Func<T, TSourcePropertyType>> sourceExpression, Func<TSourcePropertyType, TSourcePropertyType> valueFunc)
        {
            var map = new Map<T, T>(t);
            map.MapProperty(sourceExpression, (value) => valueFunc.Invoke(sourceExpression.Compile().Invoke(t)));
            return map;
        }

        public static T With<T>(this T t, dynamic a)
        {
            var properties = new List<(string Name, object Value)>();
            foreach (var prop in a.GetType().GetProperties())
            {
                var foundProp = typeof(T).GetProperty(prop.Name);
                if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
            }

            var map = new Map<T, T>(t);
            foreach (var (Name, Value) in properties) map.MapDynamicProperty(Value.GetType(), Name, () => Value);
            return Mapper.GetNewInstance().Build(map);
        }

        public static TTarget Build<TSource, TTarget>(this Map<TSource, TTarget> map)
        {
            return Mapper.GetNewInstance().Build(map);
        }
    }
}
