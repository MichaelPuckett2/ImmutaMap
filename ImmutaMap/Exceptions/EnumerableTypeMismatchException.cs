using System;

namespace ImmutaMap.Exceptions
{
    public class EnumerableTypeMismatchException : Exception
    {
        public EnumerableTypeMismatchException(Type type1, Type type2)
        : base($"{type1.Name} does not match the type {type2.Name}. ImmutaMap has no way to handle mapping these types at this time.  Please provide a specific property mapping for these types.")
        { }
    }

    public class MappedPropertyException : Exception
    {
        public MappedPropertyException(Type sourceType, Type targetType, Type producedType) 
            : base($"{sourceType.Name}.MapProperty failed: The property mapping cannot convert {producedType.Name} to {targetType.Name} . Make sure the mapping produces a result that can be used by the target property. {producedType.Name} cannot become {targetType.Name}.") { }
    }

}
