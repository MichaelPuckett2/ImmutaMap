using System.Collections.Generic;
using System.Reflection;

namespace ImmutaMap
{
    internal class SourceResultProperty
    {
        public PropertyInfo SourceProperty { get; }
        public PropertyInfo ResultProperty { get; }

        public SourceResultProperty(PropertyInfo sourceProperty, PropertyInfo resultProperty)
        {
            SourceProperty = sourceProperty;
            ResultProperty = resultProperty;
        }

        public override bool Equals(object obj)
        {
            return obj is SourceResultProperty other &&
                   EqualityComparer<PropertyInfo>.Default.Equals(SourceProperty, other.SourceProperty) &&
                   EqualityComparer<PropertyInfo>.Default.Equals(ResultProperty, other.ResultProperty);
        }

        public override int GetHashCode()
        {
            int hashCode = 1227928062;
            hashCode = hashCode * -1521134295 + EqualityComparer<PropertyInfo>.Default.GetHashCode(SourceProperty);
            hashCode = hashCode * -1521134295 + EqualityComparer<PropertyInfo>.Default.GetHashCode(ResultProperty);
            return hashCode;
        }
    }
}
