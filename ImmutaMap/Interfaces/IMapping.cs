namespace ImmutaMap.Interfaces;

/// <summary>
/// Used for mapping specific details about properties.
/// </summary>
public interface IMapping
{
    /// <summary>
    /// Tries to map properties with specific details during the mapping.
    /// </summary>
    /// <typeparam name="TSource">Source Type being mapped.</typeparam>
    /// <param name="source">Source implementation being mapped.</param>
    /// <param name="sourcePropertyInfo">Source PropertyInfo passed during mapping of the source.</param>
    /// <param name="targetPropertyInfo">Target PropertyInfo passed during mapping of the source.</param>
    /// <param name="result">Result value of the mapped property.</param>
    /// <returns>True is mapping matches and succeeds and false if it doesn't.</returns>
    bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object result);

    /// <summary>
    /// Tries to map properties with specific details during the mapping.
    /// </summary>
    /// <typeparam name="TSource">Source Type being mapped.</typeparam>
    /// <param name="source">Source implementation being mapped.</param>
    /// <param name="sourcePropertyInfo">Source PropertyInfo passed during mapping of the source.</param>
    /// <param name="targetPropertyInfo">Target PropertyInfo passed during mapping of the source.</param>
    /// <param name="previouslyMappedValue">The previously mapped value.</param>
    /// <param name="result">Result value of the mapped property.</param>
    /// <returns>True is mapping matches and succeeds and false if it doesn't.</returns>
    bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue, out object result);
}
