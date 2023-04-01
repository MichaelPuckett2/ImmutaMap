﻿using ImmutaMap.Interfaces;
using System.Reflection;

namespace ImmutaMap.Mappings;

public class TargetAttributeMapping<TAttribute> : IMapping where TAttribute : Attribute
{
    private readonly Func<Attribute, object, object> func;

    public TargetAttributeMapping(Func<Attribute, object, object> func)
    {
        this.func = func;
    }

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
