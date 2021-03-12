using System;
using System.Linq;

namespace ImmutaMap.Exceptions
{
    public class BuildException : Exception
    {
        public BuildException(Type targetType, Type producedType)
        {
            if (targetType.IsGenericType)
            {
                var genericTargetParameters = string.Join(", ", targetType.GetGenericArguments().Select(x => x.Name));
                var genericProducedParameters = string.Join(", ", producedType.GetGenericArguments().Select(x => x.Name));
                Message = $"Build failed: The property mapping cannot convert {producedType.Name}<{genericProducedParameters}> to {targetType.Name}<{genericTargetParameters}>. Make sure the mapping produces a result that can be used by the target property. {producedType.Name}<{genericProducedParameters}> cannot become {targetType.Name}<{genericTargetParameters}>. If you haven't already mapped this type you will need to use MapProperty to correct this; if you have then the logic in MapProperty isn't converted as expected.";
            }
            else
            {
                Message = $"Build failed: The property mapping cannot convert {producedType.Name} to {targetType.Name}. Make sure the mapping produces a result that can be used by the target property. {producedType.Name} cannot become {targetType.Name}. If you haven't already mapped this type you will need to use MapProperty to correct this; if you have then the logic in MapProperty isn't converted as expected.";
            }
        }

        public override string Message { get; }
    }
}
