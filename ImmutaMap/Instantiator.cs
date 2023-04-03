using ImmutaMap.Mappings;
using ImmutaMap.Utilities;
using System.Reflection;

namespace ImmutaMap;

public static class Instantiator
{
    public static T New<T>(dynamic a, Action<Map<T, T>> mapAction) where T : class
    {
        var target = new TypeFormatter().GetInstance<T>();
        var properties = new List<(string Name, object Value)>();
        foreach (var prop in a.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic))
        {
            var foundProp = typeof(T).GetProperty(prop.Name);
            if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
        }
        var map = new Map<T, T>();
        mapAction.Invoke(map);

        foreach (var (Name, Value) in properties) map.AddMapping(new DynamicMapping(Value.GetType(), Name, () => Value));
        return MapBuilder.GetNewInstance().Build(map, target);
    }

    public static T New<T>() where T : class
    {
        var target = new TypeFormatter().GetInstance<T>();
        var properties = new List<(string Name, object Value)>();
        var map = new Map<T, T>();
        foreach (var (Name, Value) in properties) map.AddMapping(new DynamicMapping(Value.GetType(), Name, () => Value));
        return MapBuilder.GetNewInstance().Build(map, target);
    }
}
