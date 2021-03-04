using ImmutaMap.Exceptions;
using ImmutaMap.Interfaces;
using ImmutaMap.Utilities;
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

        public static Mapper GetNewInstance() => new Mapper(new TypeFormatter());

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
            AddPropertyNameMaps(map, sourcePropertyInfos, resultPropertyInfos, joinedPropertInfos);

            foreach (var (sourcePropertyInfo, resultPropertyInfo) in joinedPropertInfos)
            {
                var propertyMapFuncsKey = (sourcePropertyInfo.Name, sourcePropertyInfo.PropertyType);
                if (map.PropertyMapFuncs.Keys.Contains(propertyMapFuncsKey))
                {
                    var func = map.PropertyMapFuncs[propertyMapFuncsKey];
                    var resultValue = func?.Invoke(sourcePropertyInfo.GetValue(map.Source));
                    resultPropertyInfo.SetValue(result, resultValue);
                }
                else if (sourcePropertyInfo.PropertyType != typeof(string)
                && typeof(IEnumerable).IsAssignableFrom(sourcePropertyInfo.PropertyType)
                && typeof(IEnumerable).IsAssignableFrom(resultPropertyInfo.PropertyType))
                {
                    if (sourcePropertyInfo.PropertyType != resultPropertyInfo.PropertyType)
                    {
                        throw new EnumerableTypeMismatchException(sourcePropertyInfo.PropertyType, resultPropertyInfo.PropertyType);
                    }

                    //var sourceGenericType = sourcePropertyInfo.PropertyType.GenericTypeArguments.FirstOrDefault();

                    //if (sourceGenericType != null)
                    //{
                    //    var genericList = typeof(List<>);
                    //    var resultListOfType = genericList.MakeGenericType(sourceGenericType);
                    //    var resultList = (IList)Activator.CreateInstance(resultListOfType);
                    //    foreach (var sourceValue in (IEnumerable)sourcePropertyInfo.GetValue(map.Source))
                    //    {
                    //        resultList.Add(new Mapper(new TypeFormatter()).Map(sourceValue).Build()); //TODO: This needs to be Mapped also.
                    //    }
                    //    resultPropertyInfo.SetValue(result, resultList);
                    //}
                }
                else
                {
                    resultPropertyInfo.SetValue(result, sourcePropertyInfo.GetValue(map.Source));
                }
            }
        }

        private void AddPropertyNameMaps<TSource, TResult>(Map<TSource, TResult> map, List<PropertyInfo> sourceProperties, List<PropertyInfo> resultProperties, List<(PropertyInfo sourcePropertyInfo, PropertyInfo resultPropertyInfo)> joinedPropertyInfos)
        {
            foreach (var (sourcePropertyName, resultPropertyName) in map.PropertyNameMaps)
            {
                var sourcePropertyInfo = sourceProperties.FirstOrDefault(x => x.Name == sourcePropertyName);
                if (sourcePropertyInfo == null) continue;
                var resultPropertyInfo = resultProperties.FirstOrDefault(x => x.Name == resultPropertyName);
                if (resultPropertyInfo == null) continue;
                if (joinedPropertyInfos.Any(x => x.sourcePropertyInfo.Name == sourcePropertyName && x.resultPropertyInfo.Name == resultPropertyName)) continue;
                joinedPropertyInfos.Add((sourcePropertyInfo, resultPropertyInfo));
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
}
