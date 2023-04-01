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
        public void TestPropertyNameAsMapping()
        {
            //Arrange
            var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);

            //Act
            var personClass = personRecord
                .As<PersonRecord, PersonClassLastNameSpelledDifferent>(config =>
                config.AddPropertyNameMap(x => x.LastName, x => x.Last_Name));

            //Assert
            Assert.AreEqual(personRecord.FirstName, personClass.FirstName);
            Assert.AreEqual(personRecord.LastName, personClass.Last_Name);
            Assert.AreEqual(personRecord.Age, personClass.Age);
        }

        [TestMethod]
        public void TestPropertyNameMapBuilderMapping()
        {
            //Arrange
            var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);

            //Act
            var personClass = personRecord
                .Map<PersonRecord, PersonClassLastNameSpelledDifferent>(config =>
                {
                    config
                    .AddPropertyNameMap(x => x.LastName, x => x.Last_Name);
                    //.AddPropertyNameMap<PersonRecord, PersonClassLastNameSpelledDifferent>(x => x.LastName, x => x.Last_Name);
                })
                .Build();

            //Assert
            Assert.AreEqual(personRecord.FirstName, personClass.FirstName);
            Assert.AreEqual(personRecord.LastName, personClass.Last_Name);
            Assert.AreEqual(personRecord.Age, personClass.Age);
        }
    }
}