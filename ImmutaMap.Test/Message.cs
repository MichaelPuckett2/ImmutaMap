namespace ImmutaMap.Test;

public record Message(string Msg, DateTime TimeStamp);
public class MessageDto
{
    public string Msg { get; set; } = string.Empty;
    public DateTime TimeStamp { get; set; }
}