namespace ImmutaMap
{
    public interface ITransform
    {
        ICollection<ITransformer> Transformers { get; }
    }
}