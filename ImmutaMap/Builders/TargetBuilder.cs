namespace ImmutaMap.Builders;

public class TargetBuilder
{
    private const BindingFlags PropertyBindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private readonly ITypeFormatter typeFormatter;
    private readonly IDictionary<(Type, PropertyInfo), object> transformedValues = new Dictionary<(Type, PropertyInfo), object>();

    /// <summary>
    /// Initializes the Mapper with an ITypeFormatter.
    /// </summary>
    /// <param name="typeFormatter">The ITypeFormatter is used to instantiate all types during the Build method.</param>
    TargetBuilder(ITypeFormatter typeFormatter)
    {
        this.typeFormatter = typeFormatter;
    }

    /// <summary>
    /// A simpler instantiation that allows for quick fluent designing.
    /// </summary>
    /// <returns>A new Mapper used to map and instantiate the maps target.</returns>
    public static TargetBuilder GetNewInstance() => new(ITypeFormatter.Default);

    /// <summary>
    /// A simpler instantiation that allows for quick fluent designing.
    /// </summary>
    /// <param name="typeFormatter">The ITypeFormatter is used to instantiate all types during the Build method.</param>
    /// <returns>A new Mapper used to map and instantiate the maps target.</returns>
    public static TargetBuilder GetNewInstance(ITypeFormatter typeFormatter) => new(typeFormatter);

    /// <summary>
    /// Builds the target value from the source value using the default mappings and any custom mappings put in place.
    /// </summary>
    /// <typeparam name="TSource">The source type mapped from.</typeparam>
    /// <typeparam name="TTarget">The target type mapped to.</typeparam>
    /// <param name="mapper">The Map used to build.</param>
    /// <returns>An instance of the target type with values mapped from the source instance.</returns>
    public TTarget? Build<TSource, TTarget>(IConfiguration<TSource, TTarget> configuration, TSource source)
    {
        if (source == null)
        {
            return default;
        }
        var target = typeFormatter.GetInstance<TTarget>();
        Copy(configuration, source, target);
        return target;
    }

    /// <summary>
    /// Builds the target value from the source value using the default mappings and any custom mappings put in place.
    /// </summary>
    /// <typeparam name="TSource">The source type mapped from.</typeparam>
    /// <typeparam name="TTarget">The target type mapped to.</typeparam>
    /// <param name="configuration">The Map used to build.</param>
    /// <param name="source">The source used during the mapping.</param>
    /// <param name="args">Optional parameters that may be used to instantiate the target.</param>
    /// <returns>An instance of the target type with values mapped from the source instance.</returns>
    public TTarget Build<TSource, TTarget>(IConfiguration<TSource, TTarget> configuration, TSource source, Func<object[]> args)
    {
        var target = typeFormatter.GetInstance<TTarget>(args);
        Copy(configuration, source, target);
        return target;
    }

    private void Copy<TSource, TTarget>(IConfiguration<TSource, TTarget> configuration, TSource source, TTarget target)
    {
        var skipProperties = configuration.Skips.Select(x => x.GetMemberName()).ToHashSet();
        var sourcePropertyInfos = source == null
            ? typeof(TSource).GetProperties(PropertyBindingFlag).Where(x => !skipProperties.Contains(x.Name)).ToList()
            : source.GetType().GetProperties(PropertyBindingFlag).Where(x => !skipProperties.Contains(x.Name)).ToList();
        var targetPropertyInfos = typeof(TTarget).GetProperties(PropertyBindingFlag).Where(x => !skipProperties.Contains(x.Name)).ToList();
        var joinedPropertyInfos = GetSourceResultProperties(sourcePropertyInfos, targetPropertyInfos, configuration);
        AddPropertyNameMaps(configuration, sourcePropertyInfos, targetPropertyInfos, joinedPropertyInfos);

        foreach (var (sourcePropertyInfo, targetPropertyInfo) in joinedPropertyInfos)
        {
            var isTransformed = false;
            if (configuration is ITransform transform)
            {
                foreach (var transformer in transform.Transformers)
                {
                    var previouslyTransformedValue = transformedValues.ContainsKey((typeof(TSource), sourcePropertyInfo))
                        ? transformedValues[(typeof(TSource), sourcePropertyInfo)]
                        : default;

                    if (transformedValues.ContainsKey((typeof(TSource), sourcePropertyInfo)))
                    {
                        if (transformer.TryGetValue(source, sourcePropertyInfo, targetPropertyInfo, transformedValues[(typeof(TSource), sourcePropertyInfo)], out object transformedValue))
                        {
                            transformedValues[(typeof(TSource), sourcePropertyInfo)] = transformedValue;
                            SetTargetValue(target, targetPropertyInfo, transformedValue, configuration);
                            isTransformed = true;
                        }
                    }
                    else
                    {
                        if (transformer.TryGetValue(source, sourcePropertyInfo, targetPropertyInfo, out object transformedValue))
                        {
                            transformedValues[(typeof(TSource), sourcePropertyInfo)] = transformedValue;
                            SetTargetValue(target, targetPropertyInfo, transformedValue, configuration);
                            isTransformed = true;
                        }
                    }
                }
            }

            if (!isTransformed)
            {
                var previouslyTransformedValue = transformedValues.ContainsKey((typeof(TSource), sourcePropertyInfo))
                    ? transformedValues[(typeof(TSource), sourcePropertyInfo)]
                    : default;

                if (previouslyTransformedValue != default)
                {
                    SetTargetValue(target, targetPropertyInfo, previouslyTransformedValue, configuration);
                }
                else
                {
                    object? targetValue;
                    if (typeof(TSource) != typeof(TTarget)
                    && sourcePropertyInfo.PropertyType == typeof(TSource)
                    && targetPropertyInfo.PropertyType == typeof(TTarget))
                    {
                        targetValue = GetNewInstance().Build(configuration, source);
                    }
                    else
                    {
                        targetValue = sourcePropertyInfo.GetValue(source)!;
                    }
                    SetTargetValue(target, targetPropertyInfo, targetValue, configuration);
                }
            }
        }
    }

    private void SetTargetValue<TSource, TTarget>(TTarget target, PropertyInfo targetPropertyInfo, object? targetValue, IConfiguration<TSource, TTarget> configuration)
    {
        if (targetValue != null && !targetPropertyInfo.PropertyType.IsAssignableFrom(targetValue.GetType()))
        {
            if (configuration.WillNotThrowExceptions)
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

            backingField?.SetValue(target, targetValue);
        }

        transformedValues[(typeof(TTarget), targetPropertyInfo)] = targetValue!;
    }

    private static void AddPropertyNameMaps<TSource, TResult>(IConfiguration<TSource, TResult> configuration, List<PropertyInfo> sourceProperties, List<PropertyInfo> resultProperties, List<(PropertyInfo sourcePropertyInfo, PropertyInfo resultPropertyInfo)> joinedPropertyInfos)
    {
        foreach (var (sourcePropertyMapName, resultPropertyMapName) in configuration.PropertyNameMaps)
        {
            var sourcePropertyInfo = sourceProperties.FirstOrDefault(x => configuration.IgnoreCase ? x.Name.ToLowerInvariant() == sourcePropertyMapName.ToLowerInvariant() : x.Name == sourcePropertyMapName);
            if (sourcePropertyInfo == null) continue;
            var resultPropertyInfo = resultProperties.FirstOrDefault(x => configuration.IgnoreCase ? x.Name.ToLowerInvariant() == resultPropertyMapName.ToLowerInvariant() : x.Name == resultPropertyMapName);
            if (resultPropertyInfo == null) continue;
            if (joinedPropertyInfos.Any(x =>
                configuration.IgnoreCase
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

    private static List<(PropertyInfo sourceProperty, PropertyInfo resultProperty)>
        GetSourceResultProperties<TSource, TTarget>(List<PropertyInfo> sourceProperties,
                                                    List<PropertyInfo> targetProperties,
                                                    IConfiguration<TSource, TTarget> configuration)
    {
        return sourceProperties.Join(targetProperties,
            sourceProperty => configuration.IgnoreCase ? sourceProperty.Name.ToLowerInvariant() : sourceProperty.Name,
            resultProperty => configuration.IgnoreCase ? resultProperty.Name.ToLowerInvariant() : resultProperty.Name,
            (sourceProperty, resultProperty) => (sourceProperty, resultProperty))
            .ToList();
    }
}