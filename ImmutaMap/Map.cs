using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ImmutaMap
{
    public class Map<TSource, TResult>
    {
        private readonly List<(string SourceProperty, string ResultProperty)> propertyNameMaps = new List<(string SourceProperty, string ResultProperty)>();
        public Map(TSource source)
        {
            Source = source;
        }

        internal IEnumerable<(string SourceProperty, string ResultProperty)> PropertyMaps => propertyNameMaps.ToList();

        public TSource Source { get; }

        public void MapPropertyName(Expression<Func<TSource, object>> sourceExpression, Expression<Func<TResult, object>> resultExpression)
        {
            if (sourceExpression.Body is MemberExpression sourceMemberExpression
            && resultExpression.Body is MemberExpression resultMemberExpression)
                propertyNameMaps.Add((sourceMemberExpression.Member.Name, resultMemberExpression.Member.Name));
        }

        public void MapProperty<TSourcePropertyType>(Expression<Func<TSource, TSourcePropertyType>> sourceExpression, Func<TSourcePropertyType, object> propertyResultFunc)
        {
            if (sourceExpression.Body is MemberExpression sourceMemberExpression)
            {

            }
        }
    }
}
