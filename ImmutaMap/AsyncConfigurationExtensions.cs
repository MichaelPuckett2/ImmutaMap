namespace ImmutaMap;

public static partial class ConfigurationExtensions
{
    public static ITransformAsync MapTypeAsync<TSource, TTarget, TType>(
    this ITransformAsync configuration,
    Func<TType, Task<object?>> typeMapFunc)
    {
        var typeMapping = new AsyncSourceTypeTransformer<TType>(typeMapFunc);
        configuration.AsyncTransformers.Add(typeMapping);
        return configuration;
    }

    public static ITransformAsync MapPropertyTypeAsync<TSource, TTarget, TSourcePropertyType, TTargetPropertyType>(
        this ITransformAsync configuration,
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
