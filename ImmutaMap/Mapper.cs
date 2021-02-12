using ImmutaMap.Interfaces;
using Specky.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace ImmutaMap
{
    [Speck]
    public class Mapper
    {
        private readonly ITypeFormatter typeFormatter;

        public Mapper(ITypeFormatter typeFormatter)
        {
            this.typeFormatter = typeFormatter;
        }

        public Map<TSource, TResult> Map<TSource, TResult>(TSource source) => new Map<TSource, TResult>(source);

        public TResult Build<TSource, TResult>(Map<TSource, TResult> map, Func<object[]> args = null)
        {
            TResult result;
            result = typeFormatter.GetInstance<TResult>(args);

            Copy(map, result);

            return result;
        }

        private void Copy<TSource, TResult>(Map<TSource, TResult> map, TResult result)
        {
            var sourcePropertyInfos = typeof(TSource).GetProperties().ToList();
            var resultPropertyInfos = typeof(TResult).GetProperties().ToList();

            foreach (var sourcePropertyInfo in sourcePropertyInfos)
            {
                foreach (var resultPropertyInfo in resultPropertyInfos)
                {           
                    foreach (var propertyMap in map.PropertyMaps)
                    {
                        if (propertyMap.SourceProperty == sourcePropertyInfo.Name
                        && propertyMap.ResultProperty == resultPropertyInfo.Name)
                        {

                        }
                    }
                    if (sourcePropertyInfo.Name == resultPropertyInfo.Name)
                    {
                        resultPropertyInfo.SetValue(result, sourcePropertyInfo.GetValue(map.Source));
                        break;
                    }


                }
            }
        }
    }

    public class Map<TSource, TResult>
    {
        private readonly List<(string SourceProperty, string ResultProperty)> propertyMaps = new List<(string SourceProperty, string ResultProperty)>();
        public Map(TSource source)
        {
            Source = source;
        }

        public IEnumerable<(string SourceProperty, string ResultProperty)> PropertyMaps => propertyMaps.ToList();

        public TSource Source { get; }

        public void MapProperty(Expression<Func<TSource, object>> sourceExpression, Expression<Func<TResult, object>> resultExpression)
        {
            if (sourceExpression.Body is MemberExpression sourceMemberExpression
            && resultExpression.Body is MemberExpression resultMemberExpression)
            propertyMaps.Add((sourceMemberExpression.Member.Name, resultMemberExpression.Member.Name));
        }
    }
}
