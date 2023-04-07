namespace ImmutaMap.Exceptions;

public class ImmutaMapAttributeUnknownException : Exception
{
    public ImmutaMapAttributeUnknownException(Type type) : base($"{type.Name} is an unknown ImmutaMap Attribute.") { }
}