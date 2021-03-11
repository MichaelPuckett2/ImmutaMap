# ImmutaMap
A type mapper with the ability to also map immutable types in a clean, readable, and maintainable pattern.

    //Employee is immutable in this case
    var newEmployee = existingEmployee.With(new { Department = "Stocking", YearsInService = 0 });

Previous to records in .NET 5 (C# 9) to make a true immutable type meant it was very hard to copy or work with the values.
As a result developers and architects would often skip steps writing the proper converters or mappings because the work was extensive and instead rely on non immutable types.
Immutamap uses reflection to copy immutable types into new types with specific mappings.  The default mapping is a property name to property name mapping with the values as is.

Example:  ImmutableTypeA mapped to TypeB (even non immutable) then ImmutableTypeA.FirstName would automatically map to TypeB.FirstName.

To do so you would write code like this:

    var b = a.Map<ImmutableTypeA, TypeB>().Build();

If TypeB has a different spelling of FirstName, such as First_Name then you could also do this.

    var b = a.Map<ImmutableTypeA, TypeB>()
        .MapPropertyName(source => source.FirstName, target => target.First_Name)
        .Build();

You can apply as many maps as needed before calling build.

If you need special logic for the mapping, such as type conversion, you can also map the property with distinct logic.

Example:  ImmutableTypeA.Members is type of IEnumerable'string however TypeB.Members is ICollection'string

    var b = a.Map<ImmutableTypeA, TypeB>()
        .MapProperty(source => source.Members, members => members.Tolist()) //where the second parameter is invoked to produce the target property result.
        .Build();

Once again, all mappings can be chained.

    var b = a.Map<ImmutableTypeA, TypeB>()
        .MapPropertyName(source => source.FirstName, target => target.First_Name)
        .MapProperty(source => source.Members, members => members.Tolist())
        .Build();
        
A mapping can also be stored for usage later and used like so.

    var bMapping = a.Map<ImmutableTypeA, TypeB>()
        .MapPropertyName(source => source.FirstName, target => target.First_Name)
        .MapProperty(source => source.Members, members => members.Tolist()); //We just left out the Build statement.
        
    //later we can then build our map at anytime
    var b = bMapping.Build();

There are times when you want to make a map of a source type to a target type and then apply that same map to all available types.  
At this time that code is not in place but it's coming.

In some cases you might want to just update the one or more fields of an immutable type.
The result is a new immutable type of the same type with the selected fields only modified.

Example, suppose person has a FirstName, LastName and Age where we want to make a twin person with only a different first name.

    var twinBrother = person.MapSelf(x => x.FirstName, firstName => "John").Build();

You can also create an anoymous type that maps for you with the With extension.
However note that, although this pattern is cleaner and easier to read, FirstName is dynamic in the With statement and intellisense will not assist in finding the correct property name here.
This also follows a similar pattern used with records in .NET 5.0 / C# 9.0, which may make transitioning easier and or legacy immutable mappings look more modern.

    var twinBrother = person.With(new { FirstName = "John" }); 
   
