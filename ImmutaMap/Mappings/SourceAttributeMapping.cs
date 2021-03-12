using ImmutaMap.Interfaces;
using System;
using System.Reflection;

namespace ImmutaMap.Mappings
{
    public class SourceAttributeMapping<TAttribute> : IMapping where TAttribute : Attribute
    {
        private readonly Func<Attribute, object, object> func;

        public SourceAttributeMapping(Func<Attribute, object, object> func)
        {
            this.func = func;
        }

        public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object result)
        {
            var attribute = sourcePropertyInfo.GetCustomAttribute<TAttribute>();
            if (attribute != null)
            {
                result = func.Invoke(attribute, sourcePropertyInfo.GetValue(source));
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
