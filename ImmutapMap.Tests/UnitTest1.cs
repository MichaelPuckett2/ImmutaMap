using ImmutaMap;

namespace ImmutapMap.Tests;

[TestClass]
public class UnitTest1
{
    [TestMethod]
    public void TestMethod1()
    {
        var a = new
        {
            Id = 1,
            Addresses = new List<AddressA> { new ("55555"), new("99999") }
        };

        var personB = a.As<PersonB>();

        CollectionAssert.AreEquivalent(a.Addresses, personB.Addresses.ToList());
    }
}

public record PersonB(int Id, IEnumerable<AddressA> Addresses);

public record AddressA(string Zip);