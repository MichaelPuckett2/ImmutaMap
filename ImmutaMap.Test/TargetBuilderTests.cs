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
            map.MapType<DateTime>(datetime => datetime.ToLocalTime());
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
                config.MapTypeAsync<int>(async x => await GetCountAsync());
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

    [TestMethod]
    public void TestCopyExtension()
    {
        //Arrange
        var master = new Master
        {
            Count = 50,
            Item1 = "Mock1",
            Item2 = "Mock2",
            Item3 = "Mock3"
        };

        var slave = new Slave
        {
            Count = 100,
            Item2 = "Slave2"
        };

        //Act
        master.Copy(slave);

        //Assert
        Assert.AreEqual(slave.Count, master.Count);
        Assert.AreEqual("Mock1", master.Item1);
        Assert.AreEqual(slave.Item2, master.Item2);
        Assert.AreEqual("Mock3", master.Item3);
    }

    [TestMethod]
    public void TestCopyConfiguredExtension()
    {
        //Arrange
        var master = new Master
        {
            Count = 50,
            Item1 = "Mock1",
            Item2 = "Mock2",
            Item3 = "Mock3"
        };

        var slave = new SlaveOdd
        {
            Counter = 100,
            Item_2 = "Slave2"
        };

        //Act
        master.Copy(slave, config =>
        {
            config.MapName(x => x.Counter, x => x.Count)
                  .MapName(x => x.Item_2, x => x.Item2);
        });

        //Assert
        Assert.AreEqual(slave.Counter, master.Count);
        Assert.AreEqual("Mock1", master.Item1);
        Assert.AreEqual(slave.Item_2, master.Item2);
        Assert.AreEqual("Mock3", master.Item3);
    }

    [TestMethod]
    public async Task TestCopyAsyncExtension()
    {
        //Arrange
        var master = new Master
        {
            Count = 50,
            Item1 = "Mock1",
            Item2 = "Mock2",
            Item3 = "Mock3"
        };

        var slave = new Slave
        {
            Count = 100,
            Item2 = "Slave2"
        };

        //Act
        await master.CopyAsync(slave);

        //Assert
        Assert.AreEqual(slave.Count, master.Count);
        Assert.AreEqual("Mock1", master.Item1);
        Assert.AreEqual(slave.Item2, master.Item2);
        Assert.AreEqual("Mock3", master.Item3);
    }

    [TestMethod]
    public async Task TestCopyAsyncConfiguredExtension()
    {
        //Arrange
        var master = new Master
        {
            Count = 50,
            Item1 = "Mock1",
            Item2 = "Mock2",
            Item3 = "Mock3"
        };

        var slave = new SlaveOdd
        {
            Counter = 100,
            Item_2 = "Slave2"
        };

        //Act
        await master.CopyAsync(slave, config =>
        {
            config.MapName(x => x.Counter, x => x.Count)
                  .MapName(x => x.Item_2, x => x.Item2);
            config.MapTypeAsync<string>(async str =>
            {
                await Task.Delay(1);
                str = str.ToUpper();
                await Task.Delay(1);
                return str;
            });
        });

        //Assert
        Assert.AreEqual(slave.Counter, master.Count);
        Assert.AreEqual("Mock1", master.Item1);
        Assert.AreEqual(slave.Item_2.ToUpper(), master.Item2);
        Assert.AreEqual("Mock3", master.Item3);
    }
}

public class Master
{
    public int Count { get; set; }
    public string Item1 { get; set; } = string.Empty;
    public string Item2 { get; set; } = string.Empty;
    public string Item3 { get; set; } = string.Empty;
}

public class Slave
{
    public int Count { get; set; }
    public string Item2 { get; set; } = string.Empty;
}

public class SlaveOdd
{
    public int Counter { get; set; }
    public string Item_2 { get; set; } = string.Empty;
}