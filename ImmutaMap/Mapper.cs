using ImmutaMap.Exceptions;
using ImmutaMap.Interfaces;
using Specky.Attributes;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

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

        //public Map Map(Type sourceType, object source) =>
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

            var joinedPropertInfos = GetSourceResultPropeties(sourcePropertyInfos, resultPropertyInfos);
            AddMappedProperties(map, sourcePropertyInfos, resultPropertyInfos, joinedPropertInfos);

            foreach (var (sourcePropertyInfo, resultPropertyInfo) in joinedPropertInfos)
            {
                if (sourcePropertyInfo.PropertyType != typeof(string) 
                && typeof(IEnumerable).IsAssignableFrom(sourcePropertyInfo.PropertyType)
                && typeof(IEnumerable).IsAssignableFrom(resultPropertyInfo.PropertyType))
                {
                    if (sourcePropertyInfo.PropertyType != resultPropertyInfo.PropertyType)
                    {
                        throw new EnumerableTypeMismatchException(sourcePropertyInfo.PropertyType, resultPropertyInfo.PropertyType);
                    }

                    var sourceGenericType = sourcePropertyInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

                    if (sourceGenericType != null)
                    {
                        var genericList = typeof(List<>);
                        var resultListOfType = genericList.MakeGenericType(sourceGenericType);
                        var resultList = (IList)Activator.CreateInstance(resultListOfType);
                        foreach (var sourceValue in (IEnumerable)sourcePropertyInfo.GetValue(map.Source))
                        {
                            //resultList.Add(new Mapper(new TypeFormatter()).Map(sourceValue).Build()); //TODO: This needs to be Mapped also.
                        }
                        resultPropertyInfo.SetValue(result, resultList);
                    }
                }
                else
                {
                    resultPropertyInfo.SetValue(result, sourcePropertyInfo.GetValue(map.Source));
                }
            }
        }

        private void AddMappedProperties<TSource, TResult>(Map<TSource, TResult> map, List<PropertyInfo> sourceProperties, List<PropertyInfo> resultProperties, List<(PropertyInfo sourcePropertyInfo, PropertyInfo resultPropertyInfo)> joinedProperties)
        {
            foreach (var propertyMap in map.PropertyMaps)
            {
                var sourceProperty = sourceProperties.FirstOrDefault(x => x.Name == propertyMap.SourceProperty);
                if (sourceProperty == null) continue;
                var resultProperty = resultProperties.FirstOrDefault(x => x.Name == propertyMap.ResultProperty);
                if (resultProperty == null) continue;
                if (joinedProperties.Any(x => x.sourcePropertyInfo.Name == propertyMap.SourceProperty && x.resultPropertyInfo.Name == propertyMap.ResultProperty)) continue;
                joinedProperties.Add((sourceProperty, resultProperty));
            }
        }

        private List<(PropertyInfo sourceProperty, PropertyInfo resultProperty)> GetSourceResultPropeties(List<PropertyInfo> sourceProperties, List<PropertyInfo> resultProperties)
        {
            return sourceProperties.Join(resultProperties,
                sourceProperty => sourceProperty.Name,
                resultProperty => resultProperty.Name,
                (sourceProperty, resultProperty) => (sourceProperty, resultProperty))
                .ToList();
        }
    }

    public class PropertyMap
    {
        public PropertyInfo SourcePropertyInfo { get; }
        public PropertyInfo ResultPropertyInfo { get; }
        public Func<PropertyInfo, PropertyInfo, object> Map { get; }
    }
}
