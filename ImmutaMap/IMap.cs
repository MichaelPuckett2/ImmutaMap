using ImmutaMap.Interfaces;
using System.Linq.Expressions;

namespace ImmutaMap
{
    public interface IMap<TSource, TTarget>
        where TSource : notnull
        where TTarget : notnull
    {
        bool IgnoreCase { get; set; }
        IList<Expression<Func<TSource, TTarget>>> Skips { get; }
        bool WillNotThrowExceptions { get; set; }

        Map<TSource, TTarget> MapCustom(IMapping mapping);
        Map<TSource, TTarget> MapProperty<TResult>(Expression<Func<TSource, TResult>> sourceExpression, Expression<Func<TTarget, TResult>> targetExpression);
        Map<TSource, TTarget> MapPropertyType<TSourcePropertyType, TTargetPropertyType>(Expression<Func<TSource, TSourcePropertyType>> sourceExpression, Func<TSourcePropertyType, TTargetPropertyType> propertyResultFunc)
            where TSourcePropertyType : notnull
            where TTargetPropertyType : notnull;
        Map<TSource, TTarget> MapSourceAttribute<TAttribute>(Func<TAttribute, object, object> func) where TAttribute : Attribute;
        Map<TSource, TTarget> MapTargetAttribute<TAttribute>(Func<TAttribute, object, object> func) where TAttribute : Attribute;
        Map<TSource, TTarget> MapType<TType>(Func<object, object> typeMapFunc);
    }
}