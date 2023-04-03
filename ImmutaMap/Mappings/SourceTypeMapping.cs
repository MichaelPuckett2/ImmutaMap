﻿using ImmutaMap.Interfaces;
using System.Reflection;

namespace ImmutaMap.Mappings;

/// <inheritdoc />
public class SourceTypeMapping : IMapping
{
    private readonly Type type;
    private readonly Func<object, object> typeMapFunc;

    public SourceTypeMapping(Type type, Func<object, object> typeMapFunc)
    {
        this.type = type;
        this.typeMapFunc = typeMapFunc;
    }

    /// <inheritdoc />
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, out object result)
    {
        if (sourcePropertyInfo.PropertyType != type)
        {
            result = default!;
            return false;
        }

        result = typeMapFunc.Invoke(sourcePropertyInfo.GetValue(source)!);

        return true;
    }

    /// <inheritdoc />
    public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue, out object result)
    {
        if (sourcePropertyInfo.PropertyType != type)
        {
            result = default!;
            return false;
        }

        result = typeMapFunc.Invoke(previouslyMappedValue);

        return true;
    }
}
