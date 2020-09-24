﻿using ImmutaMap.Exceptions;
using ImmutaMap.Interfaces;
using ImmutaMap.ReflectionTools;
using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace ImmutaMap
{
    public class ImmutaMapper
    {
        public ICustomActivator CustomActivator { get; }
        public IMapper Mapper { get; } 

        private ImmutaMapper(
            ICustomActivator customActivator,
            IMapper mapper)
        {
            CustomActivator = customActivator;
            Mapper = mapper;
        }

        public static ImmutaMapper Build(Action<IMapper> getMap = null)
        {
            var customActivator = new CustomActivator();
            var mapper = new Mapper();
            var immutaMapper = new ImmutaMapper(customActivator, mapper);
            getMap?.Invoke(immutaMapper.Mapper);
            return immutaMapper;
        }

        public TResult Map<TResult>(object source) => (TResult)Map(source, typeof(TResult));
        public object Map(object source, Type resultType)
        {
            var sourceType = source.GetType();
            var sourceProperties = sourceType.GetProperties().ToList();
            object result = CustomActivator.GetInstanceIgnoringCustomConstructors(resultType);

            var resultProperties = resultType.GetProperties().ToList();

            var joinedProperties = sourceProperties.Join(
                resultProperties,
                sourceProperty => sourceProperty.Name.ToLower(),
                resultProperty => resultProperty.Name.ToLower(),
                (sourceProperty, resultProperty) => new { SourceProperty = sourceProperty, ResultProperty = resultProperty })
                .ToList();

            foreach (var map in Mapper.Maps)
            {
                var sourceProperty = sourceProperties.FirstOrDefault(x => x.Name == map.SourcePropertyName);
                if (sourceProperty == null) continue;
                var resultProperty = resultProperties.FirstOrDefault(x => x.Name == map.ResultPropertyName);
                if (resultProperty == null) continue;
                if (joinedProperties.Any(x => x.SourceProperty == sourceProperty && x.ResultProperty == resultProperty)) continue;
                joinedProperties.Add(new { SourceProperty = sourceProperty, ResultProperty = resultProperty });
            }

            foreach (var join in joinedProperties.ToList())
            {
                if (join.ResultProperty.CanWrite)
                {
                    var sourceValue = join.SourceProperty.GetValue(source);

                    if (join.SourceProperty.PropertyType.IsArray)
                    {
                        var array = (Array)sourceValue;
                        if (array == null)
                        {
                            join.ResultProperty.SetValue(result, null);
                        }
                        else
                        {
                            var elementType = join.ResultProperty.PropertyType.GetElementType();
                            var mappedArray = (Array)Activator.CreateInstance(elementType.MakeArrayType(array.Length), array.Length);
                            for (var i = 0; i < array.Length; i++)
                            {
                                var mappedType = Map(array.GetValue(i), elementType);
                                mappedArray.SetValue(mappedType, i);
                            }
                            join.ResultProperty.SetValue(result, mappedArray);
                        }
                    }
                    //else if (!(sourceValue is string) && sourceValue is IEnumerable enumerable)
                    //{
                    //    switch (enumerable)
                    //    {
                    //        case Array array:
                    //            var mappedType = Map(sourceValue, join.ResultProperty.PropertyType);

                    //            break;

                    //        default:
                    //            break;
                    //    }
                    //}
                    else
                    {
                        if (join.SourceProperty.PropertyType.IsClass && join.SourceProperty.PropertyType != typeof(string))
                        {
                            var mappedType = Map(sourceValue, join.ResultProperty.PropertyType);
                            join.ResultProperty.SetValue(result, mappedType);
                        }
                        else
                        {
                            join.ResultProperty.SetValue(result, sourceValue);
                        }
                        joinedProperties.Remove(join);
                    }
                }
                else
                {
                    var fields = resultType
                        .GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                    var backingField = fields.FirstOrDefault(x => x.Name == $"<{join.ResultProperty.Name}>k__BackingField");
                    if (backingField != null)
                    {
                        var sourceValue = join.SourceProperty.GetValue(source);
                        if (join.SourceProperty.PropertyType.IsArray)
                        {
                            var array = (Array)sourceValue;
                            if (array == null)
                            {
                                backingField.SetValue(result, null);
                            }
                            else
                            {
                                var elementType = join.ResultProperty.PropertyType.GetElementType();
                                var mappedArray = (Array)Activator.CreateInstance(elementType.MakeArrayType(array.Rank), array.Length);

                                if (array.Rank > 1) throw new MultiDimensionMergeException();

                                for (var index = 0; index < array.Length; index++)
                                {
                                    var mappedType = Map(array.GetValue(index), elementType);
                                    mappedArray.SetValue(mappedType, index);
                                }

                                backingField.SetValue(result, mappedArray);
                            }
                        }
                        else if (join.SourceProperty.PropertyType.IsClass && join.SourceProperty.PropertyType != typeof(string))
                        {
                            var mergeResult = Map(sourceValue, join.ResultProperty.PropertyType);
                            backingField.SetValue(mergeResult, sourceValue);
                        }
                        else
                        {
                            backingField.SetValue(result, sourceValue);
                        }
                        joinedProperties.Remove(join);
                    }
                }
            }

            foreach (var join in joinedProperties)
            {
                if (!string.IsNullOrWhiteSpace(join.SourceProperty?.Name))
                    Trace.WriteLine($"{join.SourceProperty.Name} was not mapped from source.");
                if (!string.IsNullOrWhiteSpace(join.SourceProperty?.Name))
                    Trace.WriteLine($"{join.SourceProperty.Name} was not mapped to result.");
            }

            return result;
        }
    }
}