namespace ImmutaMap.Mappings;

/// <inheritdoc />
public class PropertyMapping<TSourcePropertyType, TTargetPropertyType> : IMapping
    where TSourcePropertyType : notnull where TTargetPropertyType : notnull
{
    private readonly (string Name, Type type) key;
    private readonly Func<TSourcePropertyType, TTargetPropertyType> func;

    public PropertyMapping(string name, Func<TSourcePropertyType, TTargetPropertyType> propertyResultFunc)
    {
        key = (name, typeof(TSourcePropertyType));
        func = new Func<TSourcePropertyType, TTargetPropertyType>(propertyResultFunc.Invoke);
    }

    /// <inheritdoc />
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object result)
    {
        var propertyMapFuncsKey = (sourcePropertyInfo.Name, sourcePropertyInfo.PropertyType);
        if (key == propertyMapFuncsKey)
        {
            TTargetPropertyType targetValue = func.Invoke((TSourcePropertyType)sourcePropertyInfo.GetValue(source)!)!;
            if (!targetPropertyInfo.PropertyType.IsAssignableFrom(targetValue.GetType()))
            {
                throw new BuildException(targetValue.GetType(), targetPropertyInfo);
            }
            result = targetValue;
            return true;
        }
        else
        {
            result = default!;
            return false;
        }
    }

    /// <inheritdoc />
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue,  out object result)
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
        else
        {
            result = default!;
            return false;
        }
    }
}
