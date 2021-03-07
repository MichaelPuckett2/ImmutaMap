using ImmutaMap.Interfaces;
using System;
using System.Runtime.Serialization;

namespace ImmutaMap.Utilities
{
    /// <summary>
    /// Can get an instance of T using the default empty constructor
    /// </summary>
    public class TypeFormatter : ITypeFormatter
    {
        public T GetInstance<T>(Func<object[]> args = null)
        {
            T result;
            var hasParameterlessConstructor = typeof(T).GetConstructor(Type.EmptyTypes) != null;
            try
            {
                if (hasParameterlessConstructor)
                    result = Activator.CreateInstance<T>();
                else if (args != null)
                    result = (T)Activator.CreateInstance(typeof(T), true);
                else
                    result = (T)Activator.CreateInstance(typeof(T), true, args);
            }
            catch
            {
                result = (T)FormatterServices.GetUninitializedObject(typeof(T));
            }
            return result;
        }
    }
}
