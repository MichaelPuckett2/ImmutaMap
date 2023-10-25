namespace ImmutaMap;

public static partial class ConfigurationExtensions
{
    public static IAsyncConfiguration<TSource, TTarget> MapTypeAsync<TSource, TTarget, TType>(
    this IAsyncConfiguration<TSource, TTarget> configuration,
    Func<TType, Task<object?>> typeMapFunc)
    {
        var typeMapping = new AsyncSourceTypeTransformer<TType>(typeMapFunc);
        configuration.AsyncTransformers.Add(typeMapping);
        return configuration;
    }

    public static IAsyncConfiguration<TSource, TTarget> MapPropertyTypeAsync<TSource, TTarget, TSourcePropertyType, TTargetPropertyType>(
        this IAsyncConfiguration<TSource, TTarget> configuration,
        Expression<Func<TSource, TSourcePropertyType>> sourceExpression,
        Func<TSourcePropertyType, ValueTask<TTargetPropertyType>> propertyResultFunc)
    {
        if (sourceExpression.Body is MemberExpression sourceMemberExpression)
        {
            configuration.AsyncTransformers.Add(new AsyncPropertyTransformer<TSourcePropertyType, TTargetPropertyType>(sourceMemberExpression.Member.Name, propertyResultFunc));
        }
        return configuration;
    }
}
