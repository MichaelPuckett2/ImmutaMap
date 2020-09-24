using ImmutaMap.Interfaces;
using System;
using System.Linq;
using System.Runtime.Serialization;

namespace ImmutaMap.ReflectionTools
{
    public class CustomActivator : ICustomActivator
    {
        public T GetInstanceIgnoringCustomConstructors<T>() => (T)GetInstanceIgnoringCustomConstructors(typeof(T));
        public object GetInstanceIgnoringCustomConstructors(Type resultType)
        {
            object result;
            if (!resultType.GetConstructors().Where(x => !x.GetParameters().Any()).Any())
            {
                result = FormatterServices.GetUninitializedObject(resultType);
            }
            else
            {
                result = Activator.CreateInstance(resultType);
            }
            return result;
        }
    }
}
