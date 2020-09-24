using System;

namespace ImmutaMap.Interfaces
{
    public interface ICustomActivator
    {
        object GetInstanceIgnoringCustomConstructors(Type resultType);
        T GetInstanceIgnoringCustomConstructors<T>();
    }
}
