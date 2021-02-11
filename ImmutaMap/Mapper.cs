using System;

namespace ImmutaMap
{
    public class Mapper
    {
        public Map<TSource, TResult> Map<TSource, TResult>(TSource source) => new Map<TSource, TResult>(source);
        public static TResult Build<TSource, TResult>(Map<TSource, TResult> map)
        {
            TResult result;

            try
            {
                result = Activator.CreateInstance<TResult>();
            }
            catch
            {
                result = Create
            }
    }

    public class Map<TSource, TResult>
    {
        public Map(TSource source)
        {
            Source = source;
        }

        public TSource Source { get; }
    }
}
