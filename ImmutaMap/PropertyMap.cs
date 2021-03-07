using System;
using System.Reflection;

namespace ImmutaMap
{
    public class PropertyMap
    {
        public PropertyInfo SourcePropertyInfo { get; }
        public PropertyInfo ResultPropertyInfo { get; }
        public Func<PropertyInfo, PropertyInfo, object> Map { get; }
    }
}
