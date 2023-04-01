namespace ImmutaMap.Exceptions;

public class SourceNullException : Exception
{
    public SourceNullException(Type type)
    : base($"Source for {type.Name} was null and cannot be built.") { }
}
