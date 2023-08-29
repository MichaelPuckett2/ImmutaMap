namespace ImmutaMap.Transformers;

/// <inheritdoc />
public class PropertyTransformer<TProperty> : ITransformer
    where TProperty : notnull{
    private readonly (string Name, Type type) key;
    private readonly Func<TProperty, object> func;

    public PropertyTransformer(string name, Func<TProperty, object> func)
    {
        key = (name, typeof(TProperty));
        this.func = func;
    }

    /// <inheritdoc />
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object result)
    {
        var propertyMapFuncsKey = (sourcePropertyInfo.Name, sourcePropertyInfo.PropertyType);
        if (key == propertyMapFuncsKey)
        {
            var targetValue = func.Invoke((TProperty)sourcePropertyInfo.GetValue(source)!)!;
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
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue, out object result)
    {
        var propertyMapFuncsKey = (sourcePropertyInfo.Name, sourcePropertyInfo.PropertyType);
        if (key == propertyMapFuncsKey)
        {
            var targetValue = func.Invoke((TProperty)previouslyMappedValue)!;
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
