namespace ImmutaMap.Interfaces;

/// <summary>
/// Represents a type that can get the instance of a value T.
/// </summary>
public interface ITypeFormatter
{
    /// <summary>
    /// Gets an instance to T
    /// </summary>
    /// <typeparam name="T">Type to initialize.</typeparam>
    /// <returns>Initialized T</returns>
    T GetInstance<T>();

    /// <summary>
    /// Gets an instance to T
    /// </summary>
    /// <typeparam name="T">Type to initialize.</typeparam>
    /// <param name="getArgs">Arguments used for initializing.</param>
    /// <returns>Initialized T</returns>
    T GetInstance<T>(Func<object[]> getArgs);

    static ITypeFormatter Default { get; } = new TypeFormatter();
}
