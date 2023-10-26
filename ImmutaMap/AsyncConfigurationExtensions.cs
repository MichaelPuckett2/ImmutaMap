namespace ImmutaMap;

public static partial class ConfigurationExtensions
{
    public static ITransformAsync MapTypeAsync<TType>(
    this ITransformAsync configuration,
    Func<TType, Task<object?>> typeMapFunc)
    {
        var typeMapping = new AsyncSourceTypeTransformer<TType>(typeMapFunc);
        configuration.AsyncTransformers.Add(typeMapping);
        return configuration;
    }

    public static AsyncConfiguration<TSource, TTarget> MapPropertyTypeAsync<TSource, TTarget, TSourcePropertyType>(
        this AsyncConfiguration<TSource, TTarget> configuration,
        Expression<Func<TSource, TSourcePropertyType>> sourceExpression,
        Func<TSourcePropertyType, ValueTask<object>> propertyResultFunc)
    {
        if (sourceExpression.Body is MemberExpression sourceMemberExpression)
        {
            configuration.AsyncTransformers.Add(new AsyncPropertyTransformer<TSourcePropertyType>(sourceMemberExpression.Member.Name, propertyResultFunc));
        }
        return configuration;
    }
}
