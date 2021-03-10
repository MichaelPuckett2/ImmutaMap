using System;
using System.Collections.Generic;
using System.Linq;

namespace ImmutaMap
{
    public class Map<TSource, TResult>
    {
        private readonly List<(string SourceProperty, string ResultProperty)> propertyNameMaps = new List<(string SourceProperty, string ResultProperty)>();
        private readonly IDictionary<(string, Type), Func<object, object>> propertyMapFuncs = new Dictionary<(string, Type), Func<object, object>>();

        public Map(TSource source)
        {
            Source = source;
        }

        internal IEnumerable<(string SourceProperty, string ResultProperty)> PropertyNameMaps => propertyNameMaps.ToList();
        internal IDictionary<(string, Type), Func<object, object>> PropertyMapFuncs => propertyMapFuncs;

        public TSource Source { get; }

        public void MapPropertyName(string sourcePropertyName, string targetPropertyName)
        {
            propertyNameMaps.Add((sourcePropertyName, targetPropertyName));
        }

        public void MapProperty<TSourcePropertyType>(string name, Func<TSourcePropertyType, object> propertyResultFunc)
        {
            var key = (name, typeof(TSourcePropertyType));
            propertyMapFuncs.Add(key, new Func<object, object>(sourceValue => propertyResultFunc.Invoke((TSourcePropertyType)sourceValue)));
        }

        //public void MapProperty<TSource, TResult, TSourcePropertyType>(this Map<TSource, TResult> map, Expression<Func<TSource, TSourcePropertyType>> sourceExpression, Func<TSourcePropertyType, object> propertyResultFunc)
        //{
        //    if (sourceExpression.Body is MemberExpression sourceMemberExpression)
        //    {
        //        map.MapProperty(sourceMemberExpression.Member.Name, propertyResultFunc);
        //    }
        //}

        public Map<TSource, TResult> MapDynamicProperty(Type type, string propertyName, Func<object> propertyResultFunc)
        {
            var key = (propertyName, type);
            propertyMapFuncs.Add(key, new Func<object, object>(sourceValue => propertyResultFunc.Invoke()));
            return this;
        }
    }
}
