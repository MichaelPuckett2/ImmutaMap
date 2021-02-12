using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ImmutaMap
{
    public class Map<TSource, TResult>
    {
        private readonly List<(string SourceProperty, string ResultProperty)> propertyMaps = new List<(string SourceProperty, string ResultProperty)>();
        public Map(TSource source)
        {
            Source = source;
        }

        internal IEnumerable<(string SourceProperty, string ResultProperty)> PropertyMaps => propertyMaps.ToList();

        public TSource Source { get; }

        public void MapProperty(Expression<Func<TSource, object>> sourceExpression, Expression<Func<TResult, object>> resultExpression)
        {
            if (sourceExpression.Body is MemberExpression sourceMemberExpression
            && resultExpression.Body is MemberExpression resultMemberExpression)
                propertyMaps.Add((sourceMemberExpression.Member.Name, resultMemberExpression.Member.Name));
        }
    }
}
