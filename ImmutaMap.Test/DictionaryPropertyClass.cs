namespace ImmutaMap.Test;

public class DictionaryItems
{
    public DictionaryItems(Dictionary<int, string> items)
    {
        Items = items;
    }
    public Dictionary<int, string> Items { get; set; } = new Dictionary<int, string>();
}