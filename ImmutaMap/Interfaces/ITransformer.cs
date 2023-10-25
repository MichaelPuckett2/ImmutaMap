namespace ImmutaMap.Interfaces;

/// <summary>
/// Transforms the target value of properties or specific properties.
/// </summary>
public interface ITransformer
{
    /// <summary>
    /// Tries by performing checks and if qualified transforms specific property values.
    /// </summary>
    /// <typeparam name="TSource">Source Type being transformed.</typeparam>
    /// <param name="source">Source implementation being transformed.</param>
    /// <param name="sourcePropertyInfo">Source PropertyInfo passed during transforming of the source.</param>
    /// <param name="targetPropertyInfo">Target PropertyInfo passed during transforming of the source.</param>
    /// <param name="targetValue">Value used to set the target property.</param>
    /// <returns>True if transforming matches any conditions and the result is transformed and false if it does not.</returns>
    bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object targetValue);

    /// <summary>
    /// Tries to transform the target property value to be set.
    /// Transformers work in order and can be chained.
    /// This function is called by the TargetBuilder internally if the target value has already been transformed.
    /// The transformed result will be given in the previouslyTransformedValue parameter.
    /// </summary>
    /// <typeparam name="TSource">Source Type being transformed.</typeparam>
    /// <param name="source">Source implementation being transformed.</param>
    /// <param name="sourcePropertyInfo">Source PropertyInfo passed during transforming of the source.</param>
    /// <param name="targetPropertyInfo">Target PropertyInfo passed during transforming of the source.</param>
    /// <param name="previouslyTransformedValue">Transformers work in order and can be chained.  If the target property value has already been transformed this will be the previously transformed value.</param>
    /// <param name="targetValue">Value used to set the target property.</param>
    /// <returns>True if transforming matches any conditions and the result is transformed and false if it does not.</returns>
    bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyTransformedValue, out object targetValue);
}
