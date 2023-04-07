namespace ImmutaMap.Test;

internal class Mappings
{
    public static Map<PersonRecord, PersonClassLastNameSpelledDifferent> GetPersonMap()
    {
        return new Map<PersonRecord, PersonClassLastNameSpelledDifferent>()
            .MapProperty(x => x.LastName, x => x.Last_Name);
    }
}
