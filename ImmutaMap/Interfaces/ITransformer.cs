namespace ImmutaMap.Interfaces;

/// <summary>
/// Transforms the target value of properties or specific properties.
/// </summary>
public interface ITransformer
{
    /// <summary>
    /// Tries to transform properties with specific details during the mapping.
    /// </summary>
    /// <typeparam name="TSource">Source Type being mapped.</typeparam>
    /// <param name="source">Source implementation being mapped.</param>
    /// <param name="sourcePropertyInfo">Source PropertyInfo passed during mapping of the source.</param>
    /// <param name="targetPropertyInfo">Target PropertyInfo passed during mapping of the source.</param>
    /// <param name="targetValue">Value used to set the target property.</param>
    /// <returns>True if transforming matches any conditions and the result is transformed and false if it does not.</returns>
    bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object targetValue);

    /// <summary>
    /// Tries to transform the target property value to be set.
    /// Transformers work in order and can be chained.
    /// This function is called by the MapBuilder internally if the target value has already been transformed.
    /// The transformed result will be given in the previouslyMappedValue parameter.
    /// </summary>
    /// <typeparam name="TSource">Source Type being mapped.</typeparam>
    /// <param name="source">Source implementation being mapped.</param>
    /// <param name="sourcePropertyInfo">Source PropertyInfo passed during mapping of the source.</param>
    /// <param name="targetPropertyInfo">Target PropertyInfo passed during mapping of the source.</param>
    /// <param name="previouslyMappedValue">Transformers work in order and can be chained.  If the target property value has already been transformed this will be the previously transformed value.</param>
    /// <param name="targetValue">Value used to set the target property.</param>
    /// <returns>True if transforming matches any conditions and the result is transformed and false if it does not.</returns>
    bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue, out object targetValue);
}

public interface ITransform<TSourceValue, TTargetValue>
{
    Func<PropertyInfo, TSourceValue, bool> If { get; }
    Func<PropertyInfo, TSourceValue, TTargetValue> Then { get; }
}

public class Transform<TSourceValue, TTargetValue> : ITransform<TSourceValue, TTargetValue>
{
    public Transform(Func<PropertyInfo, TSourceValue, bool> @if, Func<PropertyInfo, TSourceValue, TTargetValue> then)
    {
        If = @if;
        Then = then;
    }
    public Func<PropertyInfo, TSourceValue, bool> If { get; }
    public Func<PropertyInfo, TSourceValue, TTargetValue> Then { get; }
}
