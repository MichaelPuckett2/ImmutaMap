namespace ImmutaMap.Interfaces;

public interface IAsyncTransformer
{
    /// <summary>
    /// Asynchronously checks and if qualified transforms specific property values.
    /// </summary>
    /// <typeparam name="TSource">Source Type being transformed.</typeparam>
    /// <param name="source">Source implementation being transformed.</param>
    /// <param name="sourcePropertyInfo">Source PropertyInfo passed during transforming of the source.</param>
    /// <param name="targetPropertyInfo">Target PropertyInfo passed during transforming of the source.</param>
    /// <returns>Returns a unique type Boolean`T, implicitely convertable to either a boolean BooleanValue or T Item.
    ///   Boolean`T will be false if the GetValueAsync fails. If it succeeds the BooleanValue of the Boolean`T will be true and the T Item will be the transormed result.
    /// </returns>
    Task<Boolean<object>> GetValueAsync<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo);

    /// <summary>
    /// Asynchronously checks and if qualified transforms specific property values.
    /// </summary>
    /// <typeparam name="TSource">Source Type being transformed.</typeparam>
    /// <param name="source">Source implementation being transformed.</param>
    /// <param name="sourcePropertyInfo">Source PropertyInfo passed during transforming of the source.</param>
    /// <param name="targetPropertyInfo">Target PropertyInfo passed during transforming of the source.</param>
    /// <param name="previouslyTransformedValue">If the AsyncTargetBuilder has already transformed this value then this method is called and the previouslyTransformedValue will be the last transformed value.</param>
    /// <returns>Returns a unique type Boolean`T, implicitely convertable to either a boolean BooleanValue or T Item.
    ///   Boolean`T will be false if the GetValueAsync fails. If it succeeds the BooleanValue of the Boolean`T will be true and the T Item will be the transormed result.
    /// </returns>
    Task<Boolean<object>> GetValueAsync<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyTransformedValue);
}
