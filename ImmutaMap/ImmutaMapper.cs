using ImmutaMap.Exceptions;
using ImmutaMap.Interfaces;
using ImmutaMap.ReflectionTools;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

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
            var result = CustomActivator.GetInstanceIgnoringCustomConstructors(resultType);
            var resultProperties = resultType.GetProperties().ToList();
            var joinedProperties = GetSourceResultPropeties(sourceProperties, resultProperties);
            AddMappedProperties(sourceProperties, resultProperties, joinedProperties);

            foreach (var join in joinedProperties.ToList())
            {
                if (join.ResultProperty.CanWrite)
                {
                    var sourceValue = join.SourceProperty.GetValue(source);

                    var attributeType = join
                        .ResultProperty
                        .GetCustomAttributes()
                        .Where(x => Mapper.AttributeFunctions.Where(attributeFunction => attributeFunction.Key == x.GetType())
                        .Any())
                        .Select(x => new { Attribute = x, AttributeType = x.GetType() })
                        .FirstOrDefault();

                    if (attributeType != null)
                    {
                        var func = Mapper.AttributeFunctions[attributeType.AttributeType];
                        join.SourceProperty.SetValue(result, func.Invoke(attributeType.Attribute, sourceValue));

                        if (Mapper.SourcePropertyFunctions.ContainsKey(join.SourceProperty.Name))
                        {
                            join.ResultProperty.SetValue(result, Mapper.SourcePropertyFunctions[join.SourceProperty.Name].Invoke(join.SourceProperty.Name, sourceValue));
                        }
                    }
                    else
                    {
                        if (Mapper.SourcePropertyFunctions.ContainsKey(join.SourceProperty.Name))
                        {
                            join.ResultProperty.SetValue(result, Mapper.SourcePropertyFunctions[join.SourceProperty.Name].Invoke(join.SourceProperty.Name, sourceValue));
                        }
                        else if(sourceValue == null)
                        {
                            join.ResultProperty.SetValue(result, null);
                        }                         
                        else if (join.SourceProperty.PropertyType.IsArray)
                        {
                            MapSourceArray(result, join, (Array)sourceValue);
                        }
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
                        }
                        joinedProperties.Remove(join);
                    }
                }
                else
                {
                    var fields = resultType.GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                    var backingField = fields.FirstOrDefault(x => x.Name == $"<{join.ResultProperty.Name}>k__BackingField");

                    if (backingField != null)
                    {
                        var sourceValue = join.SourceProperty.GetValue(source);

                        var attributeType = join
                            .ResultProperty
                            .GetCustomAttributes()
                            .Where(x => Mapper.AttributeFunctions.Where(attributeFunction => attributeFunction.Key == x.GetType())
                            .Any())
                            .Select(x => new { Attribute = x, AttributeType = x.GetType() })
                            .FirstOrDefault();

                        if (attributeType != null)
                        {
                            var func = Mapper.AttributeFunctions[attributeType.AttributeType];
                            backingField.SetValue(result, func.Invoke(attributeType.Attribute, sourceValue));

                            if (Mapper.SourcePropertyFunctions.ContainsKey(join.SourceProperty.Name))
                            {
                                join.ResultProperty.SetValue(result, Mapper.SourcePropertyFunctions[join.SourceProperty.Name].Invoke(join.SourceProperty.Name, sourceValue));
                            }
                        }
                        else if (Mapper.SourcePropertyFunctions.ContainsKey(join.SourceProperty.Name))
                        {
                            join.ResultProperty.SetValue(result, Mapper.SourcePropertyFunctions[join.SourceProperty.Name].Invoke(join.SourceProperty.Name, sourceValue));
                        }
                        else
                        {
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

        private void MapSourceArray(object result, SourceResultProperty join, Array array)
        {
            var elementType = join.ResultProperty.PropertyType.GetElementType();
            var mappedArray = (Array)Activator.CreateInstance(elementType.MakeArrayType(array.Rank), array.Length);
            for (var i = 0; i < array.Length; i++)
            {
                var mappedType = Map(array.GetValue(i), elementType);
                mappedArray.SetValue(mappedType, i);
            }
            join.ResultProperty.SetValue(result, mappedArray);
        }

        private void AddMappedProperties(List<PropertyInfo> sourceProperties, List<PropertyInfo> resultProperties, List<SourceResultProperty> joinedProperties)
        {
            foreach (var map in Mapper.Maps)
            {
                var sourceProperty = sourceProperties.FirstOrDefault(x => x.Name == map.SourcePropertyName);
                if (sourceProperty == null) continue;
                var resultProperty = resultProperties.FirstOrDefault(x => x.Name == map.ResultPropertyName);
                if (resultProperty == null) continue;
                if (joinedProperties.Any(x => x.SourceProperty == sourceProperty && x.ResultProperty == resultProperty)) continue;
                joinedProperties.Add(new SourceResultProperty(sourceProperty, resultProperty));
            }
        }

        private static List<SourceResultProperty> GetSourceResultPropeties(List<PropertyInfo> sourceProperties, List<PropertyInfo> resultProperties)
        {
            return sourceProperties.Join(
                resultProperties,
                sourceProperty => sourceProperty.Name.ToLower(),
                resultProperty => resultProperty.Name.ToLower(),
                (sourceProperty, resultProperty) => new SourceResultProperty(sourceProperty, resultProperty))
                .ToList();
        }
    }
}
