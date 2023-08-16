namespace ImmutaMap.Transformers;

/// <inheritdoc />
public class TargetAttributeTransformer<TAttribute> : ITransformer where TAttribute : Attribute
{
    private readonly Func<Attribute, object, object> func;

    public TargetAttributeTransformer(Func<Attribute, object, object> func)
    {
        this.func = func;
    }

    /// <inheritdoc />
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object result)
    {
        var attribute = targetPropertyInfo.GetCustomAttribute<TAttribute>();
        if (attribute != null)
        {
            result = func.Invoke(attribute, sourcePropertyInfo.GetValue(source)!);
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
        var attribute = targetPropertyInfo.GetCustomAttribute<TAttribute>();
        if (attribute != null)
        {
            result = func.Invoke(attribute, previouslyMappedValue);
            return true;
        }
        else
        {
            result = default!;
            return false;
        }
    }
}
