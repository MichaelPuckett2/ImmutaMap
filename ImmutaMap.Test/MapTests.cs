namespace ImmutaMap.Test
{
    [TestClass]
    public class MapTests
    {
        [TestMethod]
        public void TestWithMapping()
        {
            //Arrange
            var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);
            const string ExpectedFirstName = "John";

            //Act
            var twinBrother = personRecord.With(x => x.FirstName, firstName => ExpectedFirstName);

            //Assert
            Assert.IsTrue(twinBrother.FirstName == ExpectedFirstName);
            Assert.AreEqual(personRecord.LastName, twinBrother.LastName);
            Assert.AreEqual(personRecord.Age, twinBrother.Age);
        }

        [TestMethod]
        public void TestAsMapping()
        {
            //Arrange
            var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);

            //Act
            var personClass = personRecord.As<PersonClass>();

            //Assert
            Assert.AreEqual(personRecord.FirstName, personClass.FirstName);
            Assert.AreEqual(personRecord.LastName, personClass.LastName);
            Assert.AreEqual(personRecord.Age, personClass.Age);
        }

        [TestMethod]
        public void TestMapBuilderMapping()
        {
            //Arrange
            var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);

            //Act
            var personClass = personRecord.Map<PersonRecord, PersonClass>().Build();

            //Assert
            Assert.AreEqual(personRecord.FirstName, personClass.FirstName);
            Assert.AreEqual(personRecord.LastName, personClass.LastName);
            Assert.AreEqual(personRecord.Age, personClass.Age);
        }

        [TestMethod]
        public void TestPropertyNameMapping()
        {
            //Arrange
            var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);

            //Act
            var personClass = personRecord
                .Map<PersonRecord, PersonClassLastNameSpelledDifferent>(config =>
                {
                    config
                    .AddPropertyNameMap<PersonRecord, PersonClassLastNameSpelledDifferent>(x => x.LastName, x => x.Last_Name);
                })
                .Build();

            //Assert
            Assert.AreEqual(personRecord.FirstName, personClass.FirstName);
            Assert.AreEqual(personRecord.LastName, personClass.Last_Name);
            Assert.AreEqual(personRecord.Age, personClass.Age);
        }
    }

    public record PersonRecord(string FirstName, string LastName, int Age)
    {
        public static PersonRecord Empty { get; } = new(string.Empty, string.Empty, int.MinValue);
    }

    public class PersonClass
    {
        public PersonClass(string firstName, string lastName, int age)
        {
            FirstName = firstName;
            LastName = lastName;
            Age = age;
        }
        public string FirstName { get; }
        public string LastName { get; }
        public int Age { get; }
        public static PersonClass Empty { get; } = new(string.Empty, string.Empty, int.MinValue);
    }

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