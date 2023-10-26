namespace ImmutaMap.Transformers;

public class AsyncPropertyTransformer<TSourcePropertyType> : IAsyncTransformer
{
    private readonly (string Name, Type type) key;
    private readonly Func<TSourcePropertyType, ValueTask<object>> func;

    public AsyncPropertyTransformer(string name, Func<TSourcePropertyType, ValueTask<object>> propertyResultFunc)
    {
        key = (name, typeof(TSourcePropertyType));
        func = new Func<TSourcePropertyType, ValueTask<object>>(propertyResultFunc.Invoke);
    }

    /// <inheritdoc />
    public async Task<Boolean<object>> GetValueAsync<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo)
    {
        var propertyMapFuncsKey = (sourcePropertyInfo.Name, sourcePropertyInfo.PropertyType);
        if (key == propertyMapFuncsKey)
        {
            var targetValue = (await func.Invoke((TSourcePropertyType)sourcePropertyInfo.GetValue(source)!)!)!;
            if (!targetPropertyInfo.PropertyType.IsAssignableFrom(targetValue.GetType()))
            {
                throw new BuildException(targetValue.GetType(), targetPropertyInfo);
            }
            return (targetValue, true);
        }
        return (default!, false);
    }

    /// <inheritdoc />
    public async Task<Boolean<object>> GetValueAsync<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyTransformedValue)
    {
        var propertyMapFuncsKey = (sourcePropertyInfo.Name, sourcePropertyInfo.PropertyType);
        if (key == propertyMapFuncsKey)
        {
            var targetValue = (await func.Invoke((TSourcePropertyType)previouslyTransformedValue)!)!;
            if (!targetPropertyInfo.PropertyType.IsAssignableFrom(targetValue.GetType()))
            {
                throw new BuildException(targetValue.GetType(), targetPropertyInfo);
            }
            return (targetValue, true);
        }
        return (default!, false);
    }
}
