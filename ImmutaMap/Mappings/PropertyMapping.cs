using ImmutaMap.Exceptions;
using ImmutaMap.Interfaces;
using System;
using System.Reflection;

namespace ImmutaMap.Mappings
{
    public class PropertyMapping<TSourcePropertyType> : IMapping
    {
        private readonly (string Name, Type type) key;
        private readonly Func<object, object> func;

        public PropertyMapping(string name, Func<TSourcePropertyType, object> propertyResultFunc)
        {
            key = (name, typeof(TSourcePropertyType));
            func = new Func<object, object>(sourceValue => propertyResultFunc.Invoke((TSourcePropertyType)sourceValue));
        }

        public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object result)
        {
            var propertyMapFuncsKey = (sourcePropertyInfo.Name, sourcePropertyInfo.PropertyType);
            if (key == propertyMapFuncsKey)
            {
                var targetValue = func?.Invoke(sourcePropertyInfo.GetValue(source));
                if (!targetPropertyInfo.PropertyType.IsAssignableFrom(targetValue.GetType()))
                {
                    throw new BuildException(targetPropertyInfo.PropertyType, targetValue.GetType());
                }
                result = targetValue;
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
    }
}
