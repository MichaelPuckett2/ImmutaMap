using System;
using System.Collections.Generic;

namespace ImmutaMap.Interfaces
{
    public interface IMapper
    {
        IEnumerable<PropertyMap> Maps { get; }
        IDictionary<Type, Func<Attribute, object, object>> AttributeFunctions { get; }

        /// <summary>
        /// Maps the source property name to the property on the result being instantiated.
        /// </summary>
        /// <param name="sourcePropertyName">The source property name.</param>
        /// <param name="resultPropertyName">The Result property name.</param>
        /// <returns></returns>
        Mapper MapProperty(string sourcePropertyName, string resultPropertyName);

        /// <summary>
        /// Invoked when the result property contains a specific attribute.
        /// </summary>
        /// <typeparam name="T">The Attribute looking for.</typeparam>
        /// <param name="func">A function invoked with parameters of the Attribute and the source property value.</param>
        /// <returns></returns>
        Mapper WithAttribute<T>(Func<T, object, object> func) where T : Attribute;
    }
}
