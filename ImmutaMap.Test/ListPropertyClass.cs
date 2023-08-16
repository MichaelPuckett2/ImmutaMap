namespace ImmutaMap.Test;

public class ListItems
{
    public ListItems(List<string> items)
    {
        Items = items;
    }
    public List<string> Items { get; set; } = new List<string>();
}
