namespace ImmutaMap.Test;

public class DictionaryPropertyClass
{
    public DictionaryPropertyClass(Dictionary<int, string> items)
    {
        Items = items;
    }
    public Dictionary<int, string> Items { get; set; } = new Dictionary<int, string>();
}