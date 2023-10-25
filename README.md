# ImmutaMap
Making the mapping of immutable types easier.  Maps between records and classes.

Pull down source to review Tests and get an example of what can be done.

Map various property names.
Map various property types.
Map attributes on properties.
Make your own custom mappings and add them.

[See Test Examples:](https://github.com/MichaelPuckett2/ImmutaMap/blob/master/ImmutaMap.Test/MapTests.cs)

Examples:
```csharp
//With Extension
var firstBorn = new Person("Mike", "Doe", 42);
var twin = firstBorn.With(x => x.FirstName, "John");
//or
var twin = firstBorn.With(new {FirstName = "John" });

var item = new Item(Number = 1);
var nextItem = item.With(x => x.Number, number => number++);

//Immutable or not mapped to different immutable or not type
var person = new Person("Mike", "Doe", 42);
var employee = person.To<Employee>();

//Map same properties, different names
var person = new Person("Mike", "Doe", 42);
var employee = person.To<Employee>(config => config.MapName(x => x.FirstName, x => x.Name));



```