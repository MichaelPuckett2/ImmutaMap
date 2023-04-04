namespace ImmutaMap.Test;

public class ListPropertyClass
{
    public ListPropertyClass(List<string> items)
    {
        Items = items;
    }
    public List<string> Items { get; set; } = new List<string>();
}
