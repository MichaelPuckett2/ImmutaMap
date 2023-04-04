namespace ImmutaMap.Test
{
    public class PersonClass
    {
        public PersonClass(string firstName, string lastName, int age)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }

        [FirstName(RealName = "MockFirstAttributeTarget")]
        [Trim]
        public string FirstName { get; }
        public string LastName { get; }
        public int Age { get; }
        public static PersonClass Empty { get; } = new(string.Empty, string.Empty, int.MinValue);
    }
}