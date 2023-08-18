namespace ImmutaMap.Interfaces;

public interface IStringTransformer
{
    string OriginalSourceName { get; }
    string TransformedSourceName { get; }
    string OriginaTargetName { get; }
    string TransformedTargetName { get; }
    void TransformSource(string originalValue, ref string transFormedValue);
    void TransformTarget(string originalValue, ref string transFormedValue);
}