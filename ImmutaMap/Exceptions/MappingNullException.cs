using System;

namespace ImmutaMap.Exceptions
{
    public class MappingNullException : Exception
    {
        public MappingNullException() : base("Mapping cannot be null.") { }
    }
}
