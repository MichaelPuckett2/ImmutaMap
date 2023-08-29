namespace ImmutaMap.Test;

public record PersonLowerPropsRecord(string firstName, string lastName, int age);
public record PersonLowerPropsLastNameDifferentRecord(string firstName, string last_Name, int age);
