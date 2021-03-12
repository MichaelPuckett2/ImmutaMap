using System;

namespace ImmutaMap.Exceptions
{
    public class EnumerableTypeMismatchException : Exception
    {
        public EnumerableTypeMismatchException(Type type1, Type type2)
        : base($"{type1.Name} does not match the type {type2.Name}. ImmutaMap has no way to handle mapping these types at this time.  Please provide a specific property mapping for these types.")
        { }
    }
}
