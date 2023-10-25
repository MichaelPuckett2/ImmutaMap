namespace ImmutaMap.Transformers;

/// <inheritdoc />
public class AsyncSourceTypeTransformer<T> : IAsyncTransformer
{
    private readonly Func<T, Task<object?>> typeMapFunc;

    public AsyncSourceTypeTransformer(Func<T, Task<object?>> typeMapFunc)
    {
        this.typeMapFunc = typeMapFunc;
    }

    /// <inheritdoc />
    public async Task<Boolean<object>> GetValueAsync<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo)
    {
        if (sourcePropertyInfo.PropertyType != typeof(T))
        {
            return (default!, false);
        }
        var result = await typeMapFunc.Invoke((T)sourcePropertyInfo.GetValue(source)!)!;
        return (result!, true);
    }

    /// <inheritdoc />
    public async Task<Boolean<object>> GetValueAsync<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue)
    {
        if (sourcePropertyInfo.PropertyType != typeof(T))
        {
            return (default!, false);
        }
        var result = await typeMapFunc.Invoke((T)previouslyMappedValue)!;
        return (result!, true);
    }
}
