namespace ImmutaMap.Transformers;

/// <inheritdoc />
public class PropertyTransformer<TSourcePropertyType> : ITransformer
{
    private readonly (string Name, Type type) key;
    private readonly Func<TSourcePropertyType, object> func;

    public PropertyTransformer(string name, Func<TSourcePropertyType, object> propertyResultFunc)
    {
        key = (name, typeof(TSourcePropertyType));
        func = new Func<TSourcePropertyType, object>(propertyResultFunc.Invoke);
    }

    /// <inheritdoc />
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object result)
    {
        var propertyMapFuncsKey = (sourcePropertyInfo.Name, sourcePropertyInfo.PropertyType);
        if (key == propertyMapFuncsKey)
        {
            var targetValue = func.Invoke((TSourcePropertyType)sourcePropertyInfo.GetValue(source)!)!;
            if (!targetPropertyInfo.PropertyType.IsAssignableFrom(targetValue.GetType()))
            {
                throw new BuildException(targetValue.GetType(), targetPropertyInfo);
            }
            result = targetValue;
            return true;
        }
        result = default!;
        return false;
    }

    /// <inheritdoc />
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue, out object result)
    {
        var propertyMapFuncsKey = (sourcePropertyInfo.Name, sourcePropertyInfo.PropertyType);
        if (key == propertyMapFuncsKey)
        {
            var targetValue = func.Invoke((TSourcePropertyType)previouslyMappedValue)!;
            if (!targetPropertyInfo.PropertyType.IsAssignableFrom(targetValue.GetType()))
            {
                throw new BuildException(targetValue.GetType(), targetPropertyInfo);
            }
            result = targetValue;
            return true;
        }
        result = default!;
        return false;
    }
}
