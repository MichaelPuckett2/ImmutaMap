using ImmutaMap.Transformers;

namespace ImmutaMap.Test;

[TestClass]
public class TargetBuilderTests
{
    [TestMethod]
    public void TestWithExtension()
    {
        //Arrange
        var actor = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedFirstName = "FirstMock2";

        //Act
        var actual = actor.With(x => x.FirstName, ExpectedFirstName);

        //Assert
        Assert.AreEqual(ExpectedFirstName, actual?.FirstName);
        Assert.AreEqual(actor.LastName, actual?.LastName);
        Assert.AreEqual(actor.Age, actual?.Age);
    }

    [TestMethod]
    public void TestWithPropertyValueExtension()
    {
        //Arrange
        var personClass = new PersonClass("FirstMock", "LastMock1", 50);
        const string ExpectedFirstName = "FirstMock1";

        //Act
        var actor = personClass.With(x => x.FirstName, firstName => $"{firstName}1");

        //Assert
        Assert.AreEqual(ExpectedFirstName, actor?.FirstName);
        Assert.AreEqual(personClass.LastName, actor?.LastName);
        Assert.AreEqual(personClass.Age, actor?.Age);
    }

    [TestMethod]
    public void TestWithDynamicExtension()
    {
        //Arrange
        var personClass = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedFirstName = "FirstMock2";

        //Act
        var actor = personClass.With(new { FirstName = ExpectedFirstName });

        //Assert
        Assert.AreEqual(ExpectedFirstName, actor?.FirstName);
        Assert.AreEqual(personClass.LastName, actor?.LastName);
        Assert.AreEqual(personClass.Age, actor?.Age);
    }

    [TestMethod]
    public void TestToExtension()
    {
        //Arrange
        var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);

        //Act
        var actor = personRecord.To<PersonClass>();

        //Assert
        Assert.AreEqual(personRecord.FirstName, actor?.FirstName);
        Assert.AreEqual(personRecord.LastName, actor?.LastName);
        Assert.AreEqual(personRecord.Age, actor?.Age);
    }

    [TestMethod]
    public void TestPropertyNameAsExtension()
    {
        //Arrange
        var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);

        //Act
        var actor = personRecord
            .To<PersonRecord, PersonClassLastNameSpelledDifferent>(map =>
            {
                map.MapName(x => x.LastName, x => x.Last_Name);
            });

        //Assert
        Assert.AreEqual(personRecord.FirstName, actor?.FirstName);
        Assert.AreEqual(personRecord.LastName, actor?.Last_Name);
        Assert.AreEqual(personRecord.Age, actor?.Age);
    }

    [TestMethod]
    public void TestSourceAttibuteWithPropertyExtension()
    {
        //Arrange
        var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);
        const string expectedValue = "MockFirstAttributeSource";

        //Act
        var actor = personRecord
            .To<PersonRecord, PersonClass>(config =>
            {
                config.MapSourceAttribute<PersonRecord, PersonClass, FirstNameAttribute>((attribute, value) => attribute.RealName);
            });

        //Assert
        Assert.AreEqual(expectedValue, actor?.FirstName);
        Assert.AreEqual(personRecord.LastName, actor?.LastName);
        Assert.AreEqual(personRecord.Age, actor?.Age);
    }

    [TestMethod]
    public void TestTargetAttibuteWithPropertyExtension()
    {
        //Arrange
        var personRecord = new PersonRecord("FirstMock1", "LastMock1", 50);
        const string expectedValue = "MockFirstAttributeTarget";

        //Act
        var actor = personRecord
            .To<PersonRecord, PersonClass>(config =>
            {
                config.MapTargetAttribute<PersonRecord, PersonClass, FirstNameAttribute>((attribute, value) => attribute.RealName);
            });

        //Assert
        Assert.AreEqual(expectedValue, actor?.FirstName);
        Assert.AreEqual(personRecord.LastName, actor?.LastName);
        Assert.AreEqual(personRecord.Age, actor?.Age);
    }

    [TestMethod]
    public void TestTargetTrimAttibuteExtension()
    {
        //Arrange
        var personRecord = new PersonRecord(" FirstMock1   ", "LastMock1", 50);
        const string expectedValue = "FirstMock1";

        //Act
        var actor = personRecord.To<PersonRecord, PersonClass>(map =>
        {
            map.MapTargetAttribute<PersonRecord, PersonClass, TrimAttribute>((attribute, value) => value is string str ? str.Trim() : value);
        });

        //Assert
        Assert.AreEqual(expectedValue, actor?.FirstName);
        Assert.AreEqual(personRecord.LastName, actor?.LastName);
        Assert.AreEqual(personRecord.Age, actor?.Age);
    }

    [TestMethod]
    public void TestPropertyTypeExtension()
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
            .To<ListItems, DictionaryItems>(config =>
            {
                config.MapPropertyType(x => x.Items, items =>
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
        CollectionAssert.AreEqual(expectedValue, actor?.Items);
    }

    [TestMethod]
    public void TestToAndWithExtension()
    {
        //Arrange
        var person = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedValue = "FirstMock1 LastMock1";

        //Act
        var actor = person
            .To<EmployeeRecord>()
            .With(x => x.FullName, fullName => $"{person.FirstName} {person.LastName}");

        //Assert
        Assert.AreEqual(ExpectedValue, actor?.FullName);
    }

    [TestMethod]
    public void TestToAndWithDynamicExtension()
    {
        //Arrange
        var person = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedValue = "FirstMock1 LastMock1";

        //Act
        var actor = person
            .To<EmployeeRecord>()
            .With(new { FullName = $"{person.FirstName} {person.LastName}" });

        //Assert
        Assert.AreEqual(ExpectedValue, actor?.FullName);
    }

    [TestMethod]
    public void TestCustomExtension()
    {
        //Arrange
        var personClass = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedFirstName = "FIRSTMOCK1";
        const string ExpectedLastName = "LASTMOCK1";

        //Act
        var actor = personClass.To<PersonClass, PersonRecord>(config =>
        {
            config.Transformers.Add(new UpperCaseTransformer());
        });

        //Assert
        Assert.AreEqual(ExpectedFirstName, actor?.FirstName);
        Assert.AreEqual(ExpectedLastName, actor?.LastName);
        Assert.AreEqual(personClass.Age, actor?.Age);
    }

    [TestMethod]
    public void TestMapTypeExtensions()
    {
        //Arrange
        var actor = new List<MessageDto>
        {
            new MessageDto{ Msg = "Mock1", TimeStamp = DateTime.UtcNow.Subtract(TimeSpan.FromHours(4)), Modified = DateTime.UtcNow },
            new MessageDto{ Msg = "Mock2", TimeStamp = DateTime.UtcNow.Subtract(TimeSpan.FromHours(4)), Modified = DateTime.UtcNow },
            new MessageDto{ Msg = "Mock3", TimeStamp = DateTime.UtcNow.Subtract(TimeSpan.FromHours(4)), Modified = DateTime.UtcNow },
        };

        var expected = new List<Message>
        {
            new Message(actor[0].Msg, actor[0].TimeStamp.ToLocalTime(), actor[0].Modified.ToLocalTime()),
            new Message(actor[1].Msg, actor[1].TimeStamp.ToLocalTime(), actor[1].Modified.ToLocalTime()),
            new Message(actor[2].Msg, actor[2].TimeStamp.ToLocalTime(), actor[2].Modified.ToLocalTime())
        };

        //Act
        var actual = actor.Select(x => x.To<MessageDto, Message>(map =>
        {
            map.MapType<MessageDto, Message, DateTime>(datetime => datetime.ToLocalTime());
        })).ToList();

        //Assert
        CollectionAssert.AreEqual(expected, actual);
    }

    [TestMethod]
    public void TestToDynamicExtension()
    {
        //Arrange
        var personClass = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedFirstName = "FirstMock1";

        //Act
        var actor = personClass.ToDynamic();

        //Assert
        Assert.AreEqual(ExpectedFirstName, actor?.FirstName);
        Assert.AreEqual(personClass.LastName, actor?.LastName);
        Assert.AreEqual(personClass.Age, actor?.Age);
    }

    [TestMethod]
    public void TestToDynamicConfiguredExtension()
    {
        //Arrange
        var personClass = new PersonClass("FirstMock1", "LastMock1", 50);
        const string ExpectedFirstName = "FirstMock1";

        //Act
        var actor = personClass.ToDynamic(map =>
        {
            map.Skips.Add((x) => x.FirstName);
        });

        //Assert
        Assert.AreEqual(ExpectedFirstName, actor?.FirstName);
        Assert.AreEqual(personClass.LastName, actor?.LastName);
        Assert.AreEqual(personClass.Age, actor?.Age);
    }

    [TestMethod]
    public async Task TestToAsyncExtension()
    {
        //Arrange
        var count = 0;
        async Task<int> GetCountAsync()
        {
            await Task.Delay(1);
            count++;
            await Task.Delay(1);
            return count;
        }

        var actors = new List<CounterClass>
        {
            new CounterClass{ Count = count++ },
            new CounterClass{ Count = count++ },
            new CounterClass{ Count = count++ }
        };

        var actuals = new List<Counter>();

        //Act
        foreach (var actor in actors)
        {
            var actual = await actor.ToAsync<CounterClass, Counter>(config =>
            {
                config.MapTypeAsync<CounterClass, Counter, int>(async x => await GetCountAsync());
            });
            actuals.Add(actual!);
        }

        //Assert
        Assert.AreEqual(4, actuals[0].Count);
        Assert.AreEqual(5, actuals[1].Count);
        Assert.AreEqual(6, actuals[2].Count);
    }

    [TestMethod]
    public async Task TestAsyncPropertyTypeExtension()
    {
        //Arrange
        var listItems = new ListItems(new() { "Mock1", "Mock2" });
        var expectedValue = new Dictionary<int, string>
        {
            [1] = "Mock1",
            [2] = "Mock2"
        };

        async ValueTask<object> GetItemsAsync(List<string> items)
        {
            await Task.Delay(1);
            var dictionary = new Dictionary<int, string>();
            var counter = 0;
            foreach (var item in items)
            {
                dictionary.Add(++counter, item);
            }
            await Task.Delay(1);
            return dictionary;
        }

        //Act
        var actor = await listItems
            .ToAsync<ListItems, DictionaryItems>(config =>
            {
                config.MapPropertyTypeAsync(x => x.Items, GetItemsAsync);
            });

        //Assert
        CollectionAssert.AreEqual(expectedValue, actor?.Items);
    }
}