﻿using ImmutaMap.Interfaces;
using System;
using System.Reflection;

namespace ImmutaMap.Mappings
{
    public class DynamicMapping : IMapping
    {
        private readonly Type type;
        private readonly (string propertyName, Type type) key;
        private readonly Func<object, object> func;

        public DynamicMapping(Type type, string propertyName, Func<object> propertyResultFunc)
        {
            key = (propertyName, type);
            func = new Func<object, object>(sourceValue => propertyResultFunc.Invoke());
            this.type = type;
        }

        public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue, out object result)
        {
            if (key == (sourcePropertyInfo.Name, type))
            {
                result = func.Invoke(previouslyMappedValue ?? sourcePropertyInfo.GetValue(source));
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
    }
}
