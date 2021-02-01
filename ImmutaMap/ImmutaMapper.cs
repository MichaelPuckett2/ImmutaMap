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

        /// <summary>
        /// Creates a new empty type ignoring constructors.  All fields / properties are set to their inherit default values.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns>Instantiated T with all properties as default value.</returns>
        public static T New<T>()
        {
            var customActivator = new CustomActivator();
            var t = (T)customActivator.GetInstanceIgnoringCustomConstructors(typeof(T));
            return t;
        }

        /// <summary>
        /// Creates a new empty type ignoring constructors.  All fields / properties are set to their inherit default values except for properties found in any passed in anonymous type properties.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="a">anonymous type used to initialize any matching properties of T.</param>
        /// <returns>Instantiated T with all propeties as default value except for properies found in passed in anonymous type.</returns>
        public static T New<T>(dynamic a)
        {
            var customActivator = new CustomActivator();
            var t = (T)customActivator.GetInstanceIgnoringCustomConstructors(typeof(T));
            var properties = new List<(string Name, object Value)>();
            foreach (var prop in a.GetType().GetProperties())
            {
                var foundProp = typeof(T).GetProperty(prop.Name);
                if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
            }

            return ImmutaMapper.Build(mapper =>
            {
                foreach (var (Name, Value) in properties) mapper.WithSourceProperty(Name, () => Value);
            })
            .Map<T>(t);
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
                            join.ResultProperty.SetValue(result, Mapper.SourcePropertyFunctions[join.SourceProperty.Name].Invoke(sourceValue));
                        }
                        else if (Mapper.SourcePropertyFunctions2.ContainsKey(join.SourceProperty.Name))
                        {
                            join.ResultProperty.SetValue(result, Mapper.SourcePropertyFunctions2[join.SourceProperty.Name].Invoke());
                        }
                    }
                    else
                    {
                        if (Mapper.SourcePropertyFunctions.ContainsKey(join.SourceProperty.Name))
                        {
                            join.ResultProperty.SetValue(result, Mapper.SourcePropertyFunctions[join.SourceProperty.Name].Invoke(sourceValue));
                        }
                        else if (Mapper.SourcePropertyFunctions2.ContainsKey(join.SourceProperty.Name))
                        {
                            join.ResultProperty.SetValue(result, Mapper.SourcePropertyFunctions2[join.SourceProperty.Name].Invoke());
                        }
                        else if (sourceValue == null)
                        {
                            join.ResultProperty.SetValue(result, null);
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
                                backingField.SetValue(result, Mapper.SourcePropertyFunctions[join.SourceProperty.Name].Invoke(sourceValue));
                            }
                        }
                        else if (Mapper.SourcePropertyFunctions.ContainsKey(join.SourceProperty.Name))
                        {
                            backingField.SetValue(result, Mapper.SourcePropertyFunctions[join.SourceProperty.Name].Invoke(sourceValue));
                        }
                        else if (Mapper.SourcePropertyFunctions2.ContainsKey(join.SourceProperty.Name))
                        {
                            backingField.SetValue(result, Mapper.SourcePropertyFunctions2[join.SourceProperty.Name].Invoke());
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
