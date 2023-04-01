namespace ImmutaMap.Interfaces;

/// <summary>
/// Represents a type that can get the instance of a value T.
/// </summary>
public interface ITypeFormatter
{
    T GetInstance<T>();
    T GetInstance<T>(Func<object[]> args);
}
