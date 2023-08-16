namespace ImmutaMap.Transformers;

public class DynamicTransformer : ITransformer
{
    private readonly Type type;
    private readonly (string propertyName, Type type) key;
    private readonly Func<object, object> func;

    public DynamicTransformer(Type type, string propertyName, Func<object> propertyResultFunc)
    {
        key = (propertyName, type);
        func = new Func<object, object>(sourceValue => propertyResultFunc.Invoke());
        this.type = type;
    }

    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object result)
    {
        if (key == (sourcePropertyInfo.Name, type))
        {
            result = func.Invoke(sourcePropertyInfo.GetValue(source)!);
            return true;
        }
        else
        {
            result = default!;
            return false;
        }
    }

    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue, out object result)
    {
        if (key == (sourcePropertyInfo.Name, type))
        {
            result = func.Invoke(previouslyMappedValue);
            return true;
        }
        else
        {
            result = default!;
            return false;
        }
    }
}