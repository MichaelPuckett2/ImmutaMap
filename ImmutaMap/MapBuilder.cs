namespace ImmutaMap;

public class MapBuilder
{
    private const BindingFlags PropertyBindingFlag = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic;
    private readonly ITypeFormatter typeFormatter;
    private readonly IDictionary<(Type, PropertyInfo), object> mappedValues = new Dictionary<(Type, PropertyInfo), object>();

    /// <summary>
    /// Initializes the Mapper with an ITypeFormatter.
    /// </summary>
    /// <param name="typeFormatter">The ITypeFormatter is used to instantiate all types during the Build method.</param>
    MapBuilder(ITypeFormatter typeFormatter)
    {
        this.typeFormatter = typeFormatter;
    }

    /// <summary>
    /// A simpler instantiation that allows for quick fluent designing.
    /// </summary>
    /// <returns>A new Mapper used to map and instantiate the maps target.</returns>
    public static MapBuilder GetNewInstance() => new(ITypeFormatter.Default);

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
    /// <param name="mapper">The Map used to build.</param>
    /// <returns>An instance of the target type with values mapped from the source instance.</returns>
    public TTarget Build<TSource, TTarget>(IConfiguration<TSource, TTarget> configuration, TSource source)
        where TSource : notnull where TTarget : notnull
    {
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
    public TTarget Build<TSource, TTarget>(IConfiguration<TSource, TTarget> configuration, TSource source, Func<object[]> args) where TSource : notnull where TTarget : notnull
    {
        var target = typeFormatter.GetInstance<TTarget>(args);
        Copy(configuration, source, target);
        return target;
    }

    private void Copy<TSource, TTarget>(IConfiguration<TSource, TTarget> configuration, TSource source, TTarget target) where TSource : notnull where TTarget : notnull
    {
        var sourceProps = source.GetType().GetProperties(PropertyBindingFlag).AsEnumerable();
        var targetProps = target.GetType().GetProperties(PropertyBindingFlag).AsEnumerable();
        //var (SourcePropertyInfos, TargetPropertyInfos) = IFilter<TSource, TTarget>.GetDefaultRule(source, target);

        var joined = sourceProps.Join(targetProps,
                                      sourceProp => (IgnoreHash<PropertyInfo>)sourceProp,
                                      targetProp => (IgnoreHash<PropertyInfo>)targetProp,
                                      (SourcePropertyInfo, TargetPropertyInfo) => (SourcePropertyInfo, TargetPropertyInfo),
                                      (a, b) => configuration.Comparers.Any(x => x.Equals(b, a)))
                                      .ToList();

        foreach (var (sourcePropertyInfo, targetPropertyInfo) in joined)
        {
            var mappingFound = false;
            foreach (var transformer in configuration.Transformers)
            {
                if (mappedValues.ContainsKey((typeof(TSource), sourcePropertyInfo)))
                {
                    if (transformer.TryGetValue(source, sourcePropertyInfo, targetPropertyInfo, mappedValues[(typeof(TSource), sourcePropertyInfo)], out object result))
                    {
                        mappedValues[(typeof(TSource), sourcePropertyInfo)] = result;
                        SetTargetValue(target, targetPropertyInfo, result, configuration);
                        mappingFound = true;
                    }
                }
                else
                {
                    if (transformer.TryGetValue(source, sourcePropertyInfo, targetPropertyInfo, out object result))
                    {
                        mappedValues[(typeof(TSource), sourcePropertyInfo)] = result;
                        SetTargetValue(target, targetPropertyInfo, result, configuration);
                        mappingFound = true;
                    }
                }
            }
            if (!mappingFound)
            {
                if (mappedValues.ContainsKey((typeof(TSource), sourcePropertyInfo)))
                {
                    SetTargetValue(target, targetPropertyInfo, mappedValues[(typeof(TSource), sourcePropertyInfo)], configuration);
                }
                else
                {
                    object targetValue;
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

    private void SetTargetValue<TSource, TTarget>(TTarget target, PropertyInfo targetPropertyInfo, object targetValue, IConfiguration<TSource, TTarget> configuration)
        where TSource : notnull where TTarget : notnull
    {
        if (targetValue != null && !targetPropertyInfo.PropertyType.IsAssignableFrom(targetValue.GetType()))
        {
            //if (configuration.WillNotThrowExceptions)
            //    return;
            //else
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
}
