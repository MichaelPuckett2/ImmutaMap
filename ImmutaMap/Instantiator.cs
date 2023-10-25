namespace ImmutaMap;

public static class Instantiator
{
    public static T? New<T>(dynamic a, Action<Configuration<T, T>> mapAction) where T : class
    {
        var target = new TypeFormatter().GetInstance<T>();
        var properties = new List<(string Name, object Value)>();
        foreach (var prop in a.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var foundProp = typeof(T).GetProperty(prop.Name);
            if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
        }
        var configuration = new Configuration<T, T>();
        mapAction.Invoke(configuration);
        foreach (var (Name, Value) in properties) configuration.Transformers.Add(new DynamicTransformer(Value.GetType(), Name, () => Value));
        return TargetBuilder.GetNewInstance().Build(configuration, target);
    }

    public static T? New<T>() where T : class
    {
        var target = new TypeFormatter().GetInstance<T>();
        var properties = new List<(string Name, object Value)>();
        var configuration = new Configuration<T, T>();
        foreach (var (Name, Value) in properties) configuration.Transformers.Add(new DynamicTransformer(Value.GetType(), Name, () => Value));
        return TargetBuilder.GetNewInstance().Build(configuration, target);
    }
}
