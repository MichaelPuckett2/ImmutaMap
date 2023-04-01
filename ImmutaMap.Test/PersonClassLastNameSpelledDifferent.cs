namespace ImmutaMap.Test
{
    public class PersonClassLastNameSpelledDifferent
    {
        public PersonClassLastNameSpelledDifferent(string firstName, string last_Name, int age)
        {
            FirstName = firstName;
            Last_Name = last_Name;
            Age = age;
        }
        public string FirstName { get; }
        public string Last_Name { get; }
        public int Age { get; }
        public static PersonClass Empty { get; } = new(string.Empty, string.Empty, int.MinValue);
    }
}