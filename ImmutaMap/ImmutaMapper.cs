using System;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization;

namespace ImmutaMap
{
    public class ImmutaMapper
    {
        private Mapper Mapper { get; } = new Mapper();

        public TResult Merge<TResult>(object source, Action<Mapper> getMap = null)
        {
            getMap?.Invoke(Mapper);
            return (TResult)Merge(source, typeof(TResult));
        }

        public object Merge(object source, Type resultType)
        {
            var sourceType = source.GetType();
            var sourceProperties = sourceType.GetProperties().ToList();

            object result;
            if (!resultType.GetConstructors().Where(x => !x.GetParameters().Any()).Any())
            {
                result = FormatterServices.GetUninitializedObject(resultType);
            }
            else
            {
                result = Activator.CreateInstance(resultType);
            }
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
                    if (join.SourceProperty.PropertyType.IsClass && join.SourceProperty.PropertyType != typeof(string))
                    {
                        var mergeResult = Merge(sourceValue, join.ResultProperty.PropertyType);
                        join.ResultProperty.SetValue(result, mergeResult);
                    }
                    else
                    {
                        join.ResultProperty.SetValue(result, sourceValue);
                    }
                    joinedProperties.Remove(join);
                }
                else
                {
                    var fields = resultType
                        .GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
                    var backingField = fields.FirstOrDefault(x => x.Name == $"<{join.ResultProperty.Name}>k__BackingField");
                    if (backingField != null)
                    {
                        var sourceValue = join.SourceProperty.GetValue(source);
                        if (join.SourceProperty.PropertyType.IsClass && join.SourceProperty.PropertyType != typeof(string))
                        {
                            var mergeResult = Merge(sourceValue, join.ResultProperty.PropertyType);
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
