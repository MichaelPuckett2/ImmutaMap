using System;
using System.Linq.Expressions;

namespace ImmutaMap
{
    public static class Extensions
    {
        public static T With<T, TSourcePropertyType>(this T t, Expression<Func<T, TSourcePropertyType>> sourceExpression, Func<TSourcePropertyType, TSourcePropertyType> valueFunc)
        {
            var mapper = Mapper.GetNewInstance();
            var map = mapper.Map<T, T>(t).MapProperty(sourceExpression, (value) => valueFunc.Invoke(sourceExpression.Compile().Invoke(t)));
            return mapper.Build(map);
        }
    }
}
