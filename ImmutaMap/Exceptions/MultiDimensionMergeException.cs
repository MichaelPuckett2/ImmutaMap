using System;

namespace ImmutaMap.Exceptions
{
    public class MultiDimensionMergeException : Exception
    {
        public MultiDimensionMergeException() : base("Cannot merge multi dimension arrays at this time.") { }
    }
}
