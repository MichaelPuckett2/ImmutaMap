namespace ImmutaMap.Transformers;

/// <inheritdoc />
public class SourceTypeTransformer<T> : ITransformer
{
    private readonly Func<T, object?> typeMapFunc;

    public SourceTypeTransformer(Func<T, object?> typeMapFunc)
    {
        this.typeMapFunc = typeMapFunc;
    }

    /// <inheritdoc />
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object result)
    {
        if (sourcePropertyInfo.PropertyType != typeof(T))
        {
            result = default!;
            return false;
        }
        result = typeMapFunc.Invoke((T)sourcePropertyInfo.GetValue(source)!)!;
        return true;
    }

    /// <inheritdoc />
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue, out object result)
    {
        if (sourcePropertyInfo.PropertyType != typeof(T))
        {
            result = default!;
            return false;
        }
        result = typeMapFunc.Invoke((T)previouslyMappedValue)!;
        return true;
    }
}
