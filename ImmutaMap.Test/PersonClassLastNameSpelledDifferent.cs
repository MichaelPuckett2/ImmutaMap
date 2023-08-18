namespace ImmutaMap.Test
{
    public class PersonClassNamesSpelledDifferent
    {
        public PersonClassNamesSpelledDifferent(string firstName, string last_Name, int age)
        {
            First_Name = firstName;
            Last_Name = last_Name;
            Age = age;
        }
        public string First_Name { get; }
        public string Last_Name { get; }
        public int Age { get; }
        public static PersonClass Empty { get; } = new(string.Empty, string.Empty, int.MinValue);
    }
}