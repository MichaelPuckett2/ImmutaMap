namespace ImmutaMap.Utilities;

/// <summary>
/// Can get an instance of T using the default empty constructor
/// </summary>
public class TypeFormatter : ITypeFormatter
{
    /// <inheritdoc />
    public T GetInstance<T>()
    {
        T result;
        var hasParameterlessConstructor = typeof(T).GetConstructor(Type.EmptyTypes) != null;
        try
        {
            if (hasParameterlessConstructor)
                result = Activator.CreateInstance<T>();
            else
                result = (T)Activator.CreateInstance(typeof(T), true)!;
        }
        catch
        {
            result = (T)FormatterServices.GetUninitializedObject(typeof(T));
        }
        return result;
    }

    /// <inheritdoc />
    public T GetInstance<T>(Func<object[]> getArgs)
    {
        T result;
        var hasParameterlessConstructor = typeof(T).GetConstructor(Type.EmptyTypes) != null;
        try
        {
            if (hasParameterlessConstructor)
                result = Activator.CreateInstance<T>();
            else
                result = (T)Activator.CreateInstance(typeof(T), true, getArgs.Invoke())!;
        }
        catch
        {
            result = (T)FormatterServices.GetUninitializedObject(typeof(T));
        }
        return result;
    }
}
