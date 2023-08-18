namespace ImmutaMap.Transformers;

/// <inheritdoc />
public class SourceTypeTransformer : ITransformer
{
    private readonly Type type;
    private readonly Func<object, object> targetValueFunc;

    public SourceTypeTransformer(Type type, Func<object, object> targetValueFunc)
    {
        this.type = type;
        this.targetValueFunc = targetValueFunc;
    }

    /// <inheritdoc />
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object result)
    {
        if (sourcePropertyInfo.PropertyType != type)
        {
            result = default!;
            return false;
        }

        result = targetValueFunc.Invoke(sourcePropertyInfo.GetValue(source)!);

        return true;
    }

    /// <inheritdoc />
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue, out object result)
    {
        if (sourcePropertyInfo.PropertyType != type)
        {
            result = default!;
            return false;
        }

        result = targetValueFunc.Invoke(previouslyMappedValue);

        return true;
    }
}
