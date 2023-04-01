using ImmutaMap.Exceptions;
using ImmutaMap.Interfaces;
using ImmutaMap.Utilities;
using System.Reflection;

namespace ImmutaMap;

public class MapBuilder
{
    private readonly ITypeFormatter typeFormatter;
    private readonly IDictionary<(Type, PropertyInfo), object> mappedValues = new Dictionary<(Type, PropertyInfo), object>();
    private bool isIgnoringCase = false;
    private bool willNotThrowExceptions = true;

    /// <summary>
    /// Initializes the Mapper with an ITypeFormatter.
    /// </summary>
    /// <param name="typeFormatter">The ITypeFormatter is used to instantiate all types during the Build method.</param>
    public MapBuilder(ITypeFormatter typeFormatter)
    {
        this.typeFormatter = typeFormatter;
    }

    /// <summary>
    /// A simpler instantiation that allows for quick fluent designing.
    /// </summary>
    /// <returns>A new Mapper used to map and instantiate the maps target.</returns>
    public static MapBuilder GetNewInstance() => new(new TypeFormatter());

    /// <summary>
    /// A simpler instantiation that allows for quick fluent designing.
    /// </summary>
    /// <param name="typeFormatter">The ITypeFormatter is used to instantiate all types during the Build method.</param>
    /// <returns>A new Mapper used to map and instantiate the maps target.</returns>
    public static MapBuilder GetNewInstance(ITypeFormatter typeFormatter) => new(typeFormatter);

    /// <summary>
    /// Builds the target value from the source value using the default mappings and any custom mappings put in place.
    /// </summary>
    /// <typeparam name="TSource">The source type mapped from.</typeparam>
    /// <typeparam name="TTarget">The target type mapped to.</typeparam>
    /// <param name="map">The Map used to build.</param>
    /// <returns>An instance of the target type with values mapped from the source instance.</returns>
    public TTarget Build<TSource, TTarget>(Map<TSource, TTarget> map) where TSource : notnull where TTarget : notnull
        => Build(map, map.Source);

    /// <summary>
    /// Builds the target value from the source value using the default mappings and any custom mappings put in place.
    /// </summary>
    /// <typeparam name="TSource">The source type mapped from.</typeparam>
    /// <typeparam name="TTarget">The target type mapped to.</typeparam>
    /// <param name="map">The Map used to build.</param>
    /// <param name="args">Optional parameters that may be used to instantiate the target.</param>
    /// <returns>An instance of the target type with values mapped from the source instance.</returns>
    public TTarget Build<TSource, TTarget>(Map<TSource, TTarget> map, Func<object[]> args) where TSource: notnull where TTarget : notnull
        => Build(map, map.Source, args);

    /// <summary>
    /// Builds the target value from the source value using the default mappings and any custom mappings put in place.
    /// </summary>
    /// <typeparam name="TSource">The source type mapped from.</typeparam>
    /// <typeparam name="TTarget">The target type mapped to.</typeparam>
    /// <param name="map">The Map used to build.</param>
    /// <param name="source">The source used during the mapping.</param>
    /// <param name="args">Optional parameters that may be used to instantiate the target.</param>
    /// <returns>An instance of the target type with values mapped from the source instance.</returns>
    public TTarget Build<TSource, TTarget>(Map<TSource, TTarget> map, TSource source) where TSource : notnull where TTarget : notnull
    {
        willNotThrowExceptions = map.WillNotThrowExceptions;
        isIgnoringCase = map.IgnoringCase;
        var target = typeFormatter.GetInstance<TTarget>();
        Copy(map, source, target);
        return target;
    }

    /// <summary>
    /// Builds the target value from the source value using the default mappings and any custom mappings put in place.
    /// </summary>
    /// <typeparam name="TSource">The source type mapped from.</typeparam>
    /// <typeparam name="TTarget">The target type mapped to.</typeparam>
    /// <param name="map">The Map used to build.</param>
    /// <param name="source">The source used during the mapping.</param>
    /// <param name="args">Optional parameters that may be used to instantiate the target.</param>
    /// <returns>An instance of the target type with values mapped from the source instance.</returns>
    public TTarget Build<TSource, TTarget>(Map<TSource, TTarget> map, TSource source, Func<object[]> args) where TSource : notnull where TTarget : notnull
    {
        willNotThrowExceptions = map.WillNotThrowExceptions;
        isIgnoringCase = map.IgnoringCase;
        var target = typeFormatter.GetInstance<TTarget>(args);
        Copy(map, source, target);
        return target;
    }

    private void Copy<TSource, TTarget>(Map<TSource, TTarget> map, TSource source, TTarget target) where TSource : notnull where TTarget: notnull 
    {
        var skipProperties = map.MapConfiguration.Skips.Select(x => x.GetMemberName()).ToList();
        var sourcePropertyInfos = source.GetType().GetProperties().ToList();
        var targetPropertyInfos = typeof(TTarget).GetProperties(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(x => !skipProperties.Contains(x.Name)).ToList();
        var joinedPropertyInfos = GetSourceResultProperties(sourcePropertyInfos, targetPropertyInfos);
        AddPropertyNameMaps(map, sourcePropertyInfos, targetPropertyInfos, joinedPropertyInfos);

        foreach (var (sourcePropertyInfo, targetPropertyInfo) in joinedPropertyInfos)
        {
            var mappingFound = false;
            foreach (var mapping in map.Mappings)
            {
                var previouslyMappedValue = mappedValues.ContainsKey((typeof(TSource), sourcePropertyInfo))
                    ? mappedValues[(typeof(TSource), sourcePropertyInfo)]
                    : default;

                if (mappedValues.ContainsKey((typeof(TSource), sourcePropertyInfo)))
                {
                    if (mapping.TryGetValue(source, sourcePropertyInfo, targetPropertyInfo, mappedValues[(typeof(TSource), sourcePropertyInfo)], out object result))
                    {
                        mappedValues[(typeof(TSource), sourcePropertyInfo)] = result;
                        SetTargetValue(target, targetPropertyInfo, result);
                        mappingFound = true;
                    }
                }
                else
                {
                    if (mapping.TryGetValue(source, sourcePropertyInfo, targetPropertyInfo, out object result))
                    {
                        mappedValues[(typeof(TSource), sourcePropertyInfo)] = result;
                        SetTargetValue(target, targetPropertyInfo, result);
                        mappingFound = true;
                    }
                }
            }
            if (!mappingFound)
            {
                var previouslyMappedValue = mappedValues.ContainsKey((typeof(TSource), sourcePropertyInfo))
                    ? mappedValues[(typeof(TSource), sourcePropertyInfo)]
                    : default;

                if (previouslyMappedValue != default)
                {
                    SetTargetValue(target, targetPropertyInfo, previouslyMappedValue);
                }
                else
                {
                    object targetValue;
                    if (typeof(TSource) != typeof(TTarget)
                    && sourcePropertyInfo.PropertyType == typeof(TSource)
                    && targetPropertyInfo.PropertyType == typeof(TTarget))
                    {
                        targetValue = GetNewInstance().Build(map, source);
                    }
                    else
                    {
                        targetValue = sourcePropertyInfo.GetValue(source)!;
                    }
                    SetTargetValue(target, targetPropertyInfo, targetValue);
                }
            }
        }
    }

    private void SetTargetValue<TTarget>(TTarget target, PropertyInfo targetPropertyInfo, object targetValue)
    {
        if (targetValue != null && !targetPropertyInfo.PropertyType.IsAssignableFrom(targetValue.GetType()))
        {
            if (willNotThrowExceptions)
                return;
            else
                throw new BuildException(targetValue.GetType(), targetPropertyInfo);
        }

        if (targetPropertyInfo.CanWrite)
        {
            targetPropertyInfo.SetValue(target, targetValue);
        }
        else
        {
            var fields = typeof(TTarget).GetFields(BindingFlags.Public | BindingFlags.Instance | BindingFlags.NonPublic);
            var backingField = fields.FirstOrDefault(x => x.Name == $"<{targetPropertyInfo.Name}>k__BackingField");

            if (backingField != null)
            {
                backingField.SetValue(target, targetValue);
            }
        }

        mappedValues[(typeof(TTarget), targetPropertyInfo)] = targetValue!;
    }

    private void AddPropertyNameMaps<TSource, TResult>(Map<TSource, TResult> map, List<PropertyInfo> sourceProperties, List<PropertyInfo> resultProperties, List<(PropertyInfo sourcePropertyInfo, PropertyInfo resultPropertyInfo)> joinedPropertyInfos)
    {
        foreach (var (sourcePropertyMapName, resultPropertyMapName) in map.PropertyNameMaps)
        {
            var sourcePropertyInfo = sourceProperties.FirstOrDefault(x => isIgnoringCase ? x.Name.ToLowerInvariant() == sourcePropertyMapName.ToLowerInvariant() : x.Name == sourcePropertyMapName);
            if (sourcePropertyInfo == null) continue;
            var resultPropertyInfo = resultProperties.FirstOrDefault(x => isIgnoringCase ? x.Name.ToLowerInvariant() == resultPropertyMapName.ToLowerInvariant() : x.Name == resultPropertyMapName);
            if (resultPropertyInfo == null) continue;
            if (joinedPropertyInfos.Any(x =>
                isIgnoringCase
                ? x.sourcePropertyInfo.Name.ToLowerInvariant() == sourcePropertyMapName.ToLowerInvariant() && x.resultPropertyInfo.Name.ToLowerInvariant() == resultPropertyMapName.ToLowerInvariant()
                : x.sourcePropertyInfo.Name == sourcePropertyMapName && x.resultPropertyInfo.Name == resultPropertyMapName))

            {
                continue;
            }
            var existingJoinedPropertyInfo = joinedPropertyInfos
                .FirstOrDefault(x => x.sourcePropertyInfo.Name == sourcePropertyInfo.Name || x.resultPropertyInfo.Name == resultPropertyInfo.Name);
            if (existingJoinedPropertyInfo != default)
            {
                joinedPropertyInfos.Remove(existingJoinedPropertyInfo);
            }
            joinedPropertyInfos.Add((sourcePropertyInfo, resultPropertyInfo));
        }
    }

    private List<(PropertyInfo sourceProperty, PropertyInfo resultProperty)> GetSourceResultProperties(List<PropertyInfo> sourceProperties, List<PropertyInfo> targetProperties)
    {
        return sourceProperties.Join(targetProperties,
            sourceProperty => isIgnoringCase ? sourceProperty.Name.ToLowerInvariant() : sourceProperty.Name,
            resultProperty => isIgnoringCase ? resultProperty.Name.ToLowerInvariant() : resultProperty.Name,
            (sourceProperty, resultProperty) => (sourceProperty, resultProperty))
            .ToList();
    }
}