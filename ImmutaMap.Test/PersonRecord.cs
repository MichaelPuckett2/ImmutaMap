namespace ImmutaMap.Test
{
    public record PersonRecord(string FirstName, string LastName, int Age)
    {
        public static PersonRecord Empty { get; } = new(string.Empty, string.Empty, int.MinValue);
    }
}