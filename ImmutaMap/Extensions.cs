﻿using ImmutaMap.Exceptions;
using ImmutaMap.Interfaces;
using ImmutaMap.Mappings;
using ImmutaMap.Utilities;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace ImmutaMap
{
    public static class Extensions
    {
        /// <summary>
        /// Starts the mapping between types. Once mapping is complete call build to get the result or start the Map for another time.
        /// If storing the Map you can call Mapper.GetNewInstance().Build(map) to get the result.
        /// </summary>
        /// <typeparam name="TSource">Type being mapped from.</typeparam>
        /// <typeparam name="TTarget">Type being mapped to.</typeparam>
        /// <param name="source">The source values used during the build process after mapping.</param>
        /// <param name="ignoreCase">If true the mapping will ignore case sensitivity. Defaults to false.</param>
        /// <param name="throwExceptions">If false the mapping will skip mappings that would throw exceptions.  Defaults to true.</param>
        /// <returns>returns a Map that can be modified, stored, or built for a result.</returns>
        public static Map<TSource, TTarget> Map<TSource, TTarget>(this TSource source, bool ignoreCase = false, bool throwExceptions = true)
        {
            return new Map<TSource, TTarget>(source, ignoreCase, throwExceptions);
        }

        /// <summary>
        /// Maps a type to itself; allows passing of anonymous types with values that get translated to to existing types values.
        /// </summary>
        /// <typeparam name="T">The source type being mapped.</typeparam>
        /// <typeparam name="TSourceProperty">The property type being mapped.</typeparam>
        /// <param name="map">The Map this method is applied to.</param>
        /// <param name="a">The anonymous object.</param>
        /// <returns>The Map this method is applied to.</returns>
        public static Map<T, TSourceProperty> MapSelf<T, TSourceProperty>(
            this Map<T, TSourceProperty> map,
            dynamic a)
        {
            var properties = new List<(string Name, object Value)>();
            foreach (var prop in a.GetType().GetProperties())
            {
                var foundProp = typeof(T).GetProperty(prop.Name);
                if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
            }

            foreach (var (Name, Value) in properties) map.AddMapping(new DynamicMapping(Value.GetType(), Name, () => Value));
            return map;
        }      

        /// <summary>
        /// Maps the name of the source property to the name of the target property.  Example: TypeA.FirstName can map to TypeB.First_Name
        /// </summary>
        /// <typeparam name="TSource">The source type mapped from.</typeparam>
        /// <typeparam name="TResult">The target type mapped to.</typeparam>
        /// <param name="map">The map this method works against.</param>
        /// <param name="sourceExpression">The source expression to obtain the source property name to map.</param>
        /// <param name="targetExpression">The result expression to obtain the target property name to map.</param>
        /// <returns>The Map used for the method.</returns>
        public static Map<TSource, TResult> MapPropertyName<TSource, TResult>(
            this Map<TSource, TResult> map,
            Expression<Func<TSource, object>> sourceExpression,
            Expression<Func<TResult, object>> targetExpression)
        {
            if (sourceExpression.Body is MemberExpression sourceMemberExpression
            && targetExpression.Body is MemberExpression resultMemberExpression)
            {
                map.AddPropertyNameMapping(sourceMemberExpression.Member.Name, resultMemberExpression.Member.Name);
            }
            return map;
        }

        /// <summary>
        /// Maps the value or property in source to the written value to the target.
        /// </summary>
        /// <typeparam name="TSource">The source type being mapped.</typeparam>
        /// <typeparam name="TTarget">The target type being mapped.</typeparam>
        /// <typeparam name="TSourcePropertyType">The source property type being mapped.</typeparam>
        /// <param name="map">The Map this method works against.</param>
        /// <param name="sourceExpression">The expression used to get the source property name and value. Invoked on Build()</param>
        /// <param name="propertyResultFunc">The function used to get the target property value. Invoked on Build()</param>
        /// <returns>The Map this method workds against.</returns>
        public static Map<TSource, TTarget> MapProperty<TSource, TTarget, TSourcePropertyType>(
            this Map<TSource, TTarget> map,
            Expression<Func<TSource, TSourcePropertyType>> sourceExpression,
            Func<TSourcePropertyType, object> propertyResultFunc)
        {
            if (sourceExpression.Body is MemberExpression sourceMemberExpression)
            {
                map.AddMapping(new PropertyMapping<TSourcePropertyType>(sourceMemberExpression.Member.Name, propertyResultFunc));
            }
            return map;
        }

        /// <summary>
        /// Maps the source properties, containing the given attribute, in a defined way.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <typeparam name="TAttribute">The attribute type.</typeparam>
        /// <param name="map">The map used in this method.</param>
        /// <param name="func">The function defined to work on the attribute mapping. Passes the attribute found, the source value, and expects the target value in return.</param>
        /// <returns>The map thsi method works against.</returns>
        public static Map<TSource, TTarget> MapSourceAttribute<TSource, TTarget, TAttribute>(this Map<TSource, TTarget> map, Func<TAttribute, object, object> func) where TAttribute : Attribute
        {
            var att = new SourceAttributeMapping<TAttribute>(new Func<Attribute, object, object>((attribute, target) => func.Invoke((TAttribute)attribute, target)));
            map.AddMapping(att);
            return map;
        }

        /// <summary>
        /// Maps the target properties, containing the given attribute, in a defined way.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <typeparam name="TAttribute">The attribute type.</typeparam>
        /// <param name="map">The map used in this method.</param>
        /// <param name="func">The function defined to work on the attribute mapping. Passes the attribute found, the source value, and expects the target value in return.</param>
        /// <returns>The map thsi method works against.</returns>
        public static Map<TSource, TTarget> MapTargetAttribute<TSource, TTarget, TAttribute>(this Map<TSource, TTarget> map, Func<TAttribute, object, object> func) where TAttribute : Attribute
        {
            var att = new TargetAttributeMapping<TAttribute>(new Func<Attribute, object, object>((attribute, target) => func.Invoke((TAttribute)attribute, target)));
            map.AddMapping(att);
            return map;
        }

        /// <summary>
        /// Adds a custom map.
        /// </summary>
        /// <typeparam name="TSource">The source type.</typeparam>
        /// <typeparam name="TTarget">The target type.</typeparam>
        /// <param name="map">The map this method works against.</param>
        /// <param name="mapping">The custom mapping added.</param>
        /// <returns>The map this method works against.</returns>
        public static Map<TSource, TTarget> MapCustom<TSource, TTarget>(this Map<TSource, TTarget> map, IMapping mapping)
        {
            if (mapping == null) throw new MappingNullException();
            map.AddMapping(mapping);
            return map;
        }

        /// <summary>
        /// Maps a type from source property.
        /// </summary>
        /// <typeparam name="TSource">The source type being mapped.</typeparam>
        /// <typeparam name="TTarget">The target type being mapped.</typeparam>
        /// <typeparam name="TType">The source property type being mapped.</typeparam>
        /// <param name="map">The map this method works against.</param>
        /// <param name="typeMapFunc">The function used to get the result value.</param>
        /// <returns>The map this method works against.</returns>
        public static Map<TSource, TTarget> MapType<TSource, TTarget, TType>(this Map<TSource, TTarget> map, Func<object, object> typeMapFunc)
        {
            var typeMapping = new SourceTypeMapping(typeof(TType), typeMapFunc);
            map.AddMapping(typeMapping);
            return map;
        }

        /// <summary>
        /// Builds the Map, invoking all mappings added.  Takes the source values and places them in the targets values.
        /// </summary>
        /// <typeparam name="TSource">The source type built from.</typeparam>
        /// <typeparam name="TTarget">The target type being produced / instantiated.</typeparam>
        /// <param name="map">The Map this method works against.</param>
        /// <returns>The result of the mapping as an instantiated TTarget Type.</returns>
        public static TTarget Build<TSource, TTarget>(this Map<TSource, TTarget> map)
        {
            return MapBuilder.GetNewInstance().Build(map, map.Source);
        }

        /// <summary>
        /// A quick update of a Type using an anonymous type to get the values to use.
        /// </summary>
        /// <typeparam name="T">The type this method works against.</typeparam>
        /// <param name="t">The instantiation of the type being mapped from.</param>
        /// <param name="a">The anonymous type used to make the mapping work.</param>
        /// <returns>Instantiated T target value.</returns>
        public static T With<T>(this T t, dynamic a, bool throwExceptions = true)
        {
            var properties = new List<(string Name, object Value)>();
            foreach (var prop in a.GetType().GetProperties())
            {
                var foundProp = typeof(T).GetProperty(prop.Name);
                if (foundProp != null) properties.Add((prop.Name, prop.GetValue(a, null)));
            }
            var map = new Map<T, T>(throwExceptions: throwExceptions);

            foreach (var (Name, Value) in properties) map.AddMapping(new DynamicMapping(Value.GetType(), Name, () => Value));
            return MapBuilder.GetNewInstance().Build(map, t);
        }

        public static TTarget With<TSource, TTarget>(this TSource source, Map<TSource, TTarget> map)
        {
            return MapBuilder.GetNewInstance().Build(map, source);
        }

        /// <summary>
        /// Maps a type to itself where an expression binding the property to a map and another function is used to perform the mapping logic.
        /// </summary>
        /// <typeparam name="TSource">The source type being mapped.</typeparam>
        /// <typeparam name="TSourcePropertyType">The source property type being mapped.</typeparam>
        /// <param name="t">The source object this method sets the mapping against.</param>
        /// <param name="sourceExpression">The expression used to get teh source property.</param>
        /// <param name="valueFunc">The function used to get the target value from the source property.</param>
        /// <returns></returns>
        public static TSource With<TSource, TSourcePropertyType>(this TSource t, Expression<Func<TSource, TSourcePropertyType>> sourceExpression, Func<TSourcePropertyType, TSourcePropertyType> valueFunc, bool throwExceptions = true)
        {
            return t.Map<TSource, TSource>(throwExceptions: throwExceptions).MapProperty(sourceExpression, (value) => valueFunc.Invoke(sourceExpression.Compile().Invoke(t))).Build();
        }

        /// <summary>
        /// For simple one to one mappings from type to type.
        /// </summary>
        /// <typeparam name="T">The type to map to.</typeparam>
        /// <param name="obj">The obejct this method works against.</param>
        /// <returns>Returns an instantiated T with the values from the object used as reference.</returns>
        public static T As<T>(this object obj, bool ignoreCase = false, bool throwExceptions = true)
        {
            return MapBuilder.GetNewInstance().Build(new Map<object, T>(ignoreCase, throwExceptions), obj);
        }
    }
}
