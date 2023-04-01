using System.Reflection;

namespace ImmutaMap.Exceptions;

public class BuildException : Exception
{
    public BuildException(Type producedType, PropertyInfo propertyInfo)
    {
        if (propertyInfo.PropertyType.IsGenericType)
        {
            var genericTargetParameters = string.Join(", ", propertyInfo.PropertyType.GetGenericArguments().Select(x => x.Name));
            var genericProducedParameters = string.Join(", ", producedType.GetGenericArguments().Select(x => x.Name));
            Message = $"Build failed: The property mapping cannot convert {propertyInfo.Name} of {producedType.Name}<{genericProducedParameters}> to {propertyInfo.PropertyType.Name}<{genericTargetParameters}>. Make sure the mapping produces a result that can be used by the target property. {producedType.Name}<{genericProducedParameters}> cannot become {propertyInfo.PropertyType.Name}<{genericTargetParameters}>. If you haven't already mapped this type you will need to use MapProperty to correct this; if you have then the logic in MapProperty isn't converted as expected.";
        }
        else
        {
            Message = $"Build failed: The property mapping cannot convert {propertyInfo.Name} of {producedType.Name} to {propertyInfo.PropertyType.Name}. Make sure the mapping produces a result that can be used by the target property. {producedType.Name} cannot become {propertyInfo.PropertyType.Name}. If you haven't already mapped this type you will need to use MapProperty to correct this; if you have then the logic in MapProperty isn't converted as expected.";
        }
    }

    public override string Message { get; }
}
