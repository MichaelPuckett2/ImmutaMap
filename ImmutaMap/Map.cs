using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

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

        public Map<TSource, TResult> MapPropertyName(Expression<Func<TSource, object>> sourceExpression, Expression<Func<TResult, object>> resultExpression)
        {
            if (sourceExpression.Body is MemberExpression sourceMemberExpression
            && resultExpression.Body is MemberExpression resultMemberExpression)
                propertyNameMaps.Add((sourceMemberExpression.Member.Name, resultMemberExpression.Member.Name));
            return this;
        }

        public Map<TSource, TResult> MapProperty<TSourcePropertyType>(Expression<Func<TSource, TSourcePropertyType>> sourceExpression, Func<TSourcePropertyType, object> propertyResultFunc)
        {
            if (sourceExpression.Body is MemberExpression sourceMemberExpression)
            {
                var key = (sourceMemberExpression.Member.Name, typeof(TSourcePropertyType));
                propertyMapFuncs.Add(key, new Func<object, object>(sourceValue => propertyResultFunc.Invoke((TSourcePropertyType)sourceValue)));
            }
            return this;
        }

        public Map<TSource, TResult> MapDynamicProperty(Type type, string propertyName, Func<object> propertyResultFunc)
        {
            var key = (propertyName, type);
            propertyMapFuncs.Add(key, new Func<object, object>(sourceValue => propertyResultFunc.Invoke()));
            return this;
        }
    }
}
