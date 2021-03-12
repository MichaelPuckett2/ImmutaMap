using System.Reflection;

namespace ImmutaMap.Interfaces
{
    public interface IMapping
    {
        bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object result);
    }
}
