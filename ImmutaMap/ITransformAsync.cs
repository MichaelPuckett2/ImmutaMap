namespace ImmutaMap
{
    public interface ITransformAsync
    {
        ICollection<IAsyncTransformer> AsyncTransformers { get; }
    }
}