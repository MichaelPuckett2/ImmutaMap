using ImmutaMap.Transformers;

namespace ImmutaMap.Test;

[TestClass]
public class MapTests
{
    [TestMethod]
    public void TestWithMapping()
    {
        //Arrange
        var personClass = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedFirstName = "FirstMock2";

        //Act
        var actor = personClass.With(x => x.FirstName, ExpectedFirstName);

        //Assert
        Assert.AreEqual(ExpectedFirstName, actor.FirstName);
        Assert.AreEqual(personClass.LastName, actor.LastName);
        Assert.AreEqual(personClass.Age, actor.Age);
    }

    [TestMethod]
    public void TestWithPropertyValueMapping()
    {
        //Arrange
        var personClass = new PersonClass("FirstMock", "LastMock1", 50);
        const string ExpectedFirstName = "FirstMock1";

        //Act
        var actor = personClass.With(x => x.FirstName, firstName => $"{firstName}1");

        //Assert
        Assert.AreEqual(ExpectedFirstName, actor.FirstName);
        Assert.AreEqual(personClass.LastName, actor.LastName);
        Assert.AreEqual(personClass.Age, actor.Age);
    }

    [TestMethod]
    public void TestWithDynamicMapping()
    {
        //Arrange
        var personClass = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedFirstName = "FirstMock2";

        //Act
        var actor = personClass.With(new { FirstName = ExpectedFirstName });

        //Assert
        Assert.AreEqual(ExpectedFirstName, actor.FirstName);
        Assert.AreEqual(personClass.LastName, actor.LastName);
        Assert.AreEqual(personClass.Age, actor.Age);
    }

    [TestMethod]
    public void TestAsMapping()
    {
        //Arrange
        var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);

        //Act
        var actor = personRecord.As<PersonClass>();

        //Assert
        Assert.AreEqual(personRecord.FirstName, actor.FirstName);
        Assert.AreEqual(personRecord.LastName, actor.LastName);
        Assert.AreEqual(personRecord.Age, actor.Age);
    }

    [TestMethod]
    public void TestPropertyNameAsMapping()
    {
        //Arrange
        var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);

        //Act
        var actor = personRecord
            .As<PersonRecord, PersonClassLastNameSpelledDifferent>(map =>
            {
                map.MapProperty(x => x.LastName, x => x.Last_Name);
            });

        //Assert
        Assert.AreEqual(personRecord.FirstName, actor.FirstName);
        Assert.AreEqual(personRecord.LastName, actor.Last_Name);
        Assert.AreEqual(personRecord.Age, actor.Age);
    }

    [TestMethod]
    public void TestSourceAttibuteWithPropertyMapping()
    {
        //Arrange
        var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);
        const string expectedValue = "MockFirstAttributeSource";

        //Act
        var actor = personRecord
            .As<PersonRecord, PersonClass>(map =>
            {
                map.MapSourceAttribute<PersonRecord, PersonClass, FirstNameAttribute>((attribute, value) => attribute.RealName);
            });

        //Assert
        Assert.AreEqual(expectedValue, actor.FirstName);
        Assert.AreEqual(personRecord.LastName, actor.LastName);
        Assert.AreEqual(personRecord.Age, actor.Age);
    }

    [TestMethod]
    public void TestTargetAttibuteWithPropertyMapping()
    {
        //Arrange
        var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);
        const string expectedValue = "MockFirstAttributeTarget";

        //Act
        var actor = personRecord
            .As<PersonRecord, PersonClass>(config =>
            {
                config.MapTargetAttribute<PersonRecord, PersonClass, FirstNameAttribute>((attribute, value) => attribute.RealName);
            });

        //Assert
        Assert.AreEqual(expectedValue, actor.FirstName);
        Assert.AreEqual(personRecord.LastName, actor.LastName);
        Assert.AreEqual(personRecord.Age, actor.Age);
    }

    [TestMethod]
    public void TestTargetTrimAttibuteMapping()
    {
        //Arrange
        var personRecord = new PersonRecord(" FirstMock1   ", "LastMock1", 50);
        const string expectedValue = "FirstMock1";

        //Act
        var actor = personRecord.As<PersonRecord, PersonClass>(map =>
        {
            map.MapTargetAttribute<PersonRecord, PersonClass, TrimAttribute>((attribute, value) => value is string str ? str.Trim() : value);
        });

        //Assert
        Assert.AreEqual(expectedValue, actor.FirstName);
        Assert.AreEqual(personRecord.LastName, actor.LastName);
        Assert.AreEqual(personRecord.Age, actor.Age);
    }

    [TestMethod]
    public void TestPropertyTypeMapping()
    {
        //Arrange
        var listItems = new ListItems(new() { "Mock1", "Mock2" });
        var expectedValue = new Dictionary<int, string>
        {
            [1] = "Mock1",
            [2] = "Mock2"
        };

        //Act
        var actor = listItems
            .As<ListItems, DictionaryItems>(map =>
            {
                map.MapPropertyType(x => x.Items, items =>
                {
                    var dictionary = new Dictionary<int, string>();
                    var counter = 0;
                    foreach (var item in items)
                    {
                        dictionary.Add(++counter, item);
                    }
                    return dictionary;
                });
            });

        //Assert
        CollectionAssert.AreEqual(expectedValue, actor.Items);
    }

    [TestMethod]
    public void TestAsAndWithMapping()
    {
        //Arrange
        var person = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedValue = "FirstMock1 LastMock1";

        //Act
        var actor = person
            .As<EmployeeRecord>()
            .With(x => x.FullName, fullName => $"{person.FirstName} {person.LastName}");

        //Assert
        Assert.AreEqual(ExpectedValue, actor.FullName);
    }

    [TestMethod]
    public void TestAsAndWithDynamicMapping()
    {
        //Arrange
        var person = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedValue = "FirstMock1 LastMock1";

        //Act
        var actor = person
            .As<EmployeeRecord>()
            .With(new { FullName = $"{person.FirstName} {person.LastName}" });

        //Assert
        Assert.AreEqual(ExpectedValue, actor.FullName);
    }

    [TestMethod]
    public void TestCustomMapping()
    {
        //Arrange
        var personClass = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedFirstName = "FIRSTMOCK1";
        const string ExpectedLastName = "LASTMOCK1";

        //Act
        var actor = personClass.As<PersonClass, PersonRecord>(config =>
        {
            config.Transformers.Add(new UpperCaseTransformer());
        });

        //Assert
        Assert.AreEqual(ExpectedFirstName, actor.FirstName);
        Assert.AreEqual(ExpectedLastName, actor.LastName);
        Assert.AreEqual(personClass.Age, actor.Age);
    }

    [TestMethod]
    public void TestAsAnonymous()
    {
        //Arrange
        var personClass = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedFirstName = "FirstMock1";

        //Act
        var actor = personClass.AsDynamic();

        //Assert
        Assert.AreEqual(ExpectedFirstName, actor.FirstName);
        Assert.AreEqual(personClass.LastName, actor.LastName);
        Assert.AreEqual(personClass.Age, actor.Age);
    }

    [TestMethod]
    public void TestAsAnonymousMap()
    {
        //Arrange
        var personClass = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedFirstName = "FirstMock1";

        //Act
        var actor = personClass.AsDynamic(map =>
        {
            map.Skips.Add((x) => x.FirstName);
        });

        //Assert
        Assert.AreEqual(ExpectedFirstName, actor.FirstName);
        Assert.AreEqual(personClass.LastName, actor.LastName);
        Assert.AreEqual(personClass.Age, actor.Age);
    }
}