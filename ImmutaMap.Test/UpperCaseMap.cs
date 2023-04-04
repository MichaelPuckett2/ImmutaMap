using System.Reflection;

namespace ImmutaMap.Test;

public class UpperCaseMap : Interfaces.IMapping
{
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue, out object result)
    {
        if (sourcePropertyInfo.PropertyType == typeof(string))
        {
            if (previouslyMappedValue is string str)
            {
                result = str.ToUpper();
            }
            else
            {
                result = ((string)sourcePropertyInfo.GetValue(source)!).ToUpper();
            }
            return true;
        }
        else
        {
            result = default!;
            return false;
        }
    }

    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object result)
    {
        if (sourcePropertyInfo.PropertyType == typeof(string))
        {
            result = ((string)sourcePropertyInfo.GetValue(source)!).ToUpper();
            return true;
        }
        else
        {
            result = default!;
            return false;
        }
    }
}