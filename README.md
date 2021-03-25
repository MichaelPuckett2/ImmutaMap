# ImmutaMap
A type mapper with the ability to also map immutable types in a clean, readable, and maintainable pattern. Previous to records in .NET 5 (C# 9) to make a true immutable type meant it was very hard to copy or work with the values.
As a result developers and architects would often skip steps writing the proper converters or mappings because the work was extensive and instead rely on non immutable types.
Immutamap uses reflection to copy immutable types into new types with specific mappings.  The default mapping is a property name to property name mapping with the values as is.

## Quick mapping of same type
In some cases you might want to just update the one or more fields of an immutable type.
The result is a new immutable type of the same type with the selected fields only modified.

Example, suppose person has a FirstName, LastName and Age where we want to make a twin person with only a different first name.

    var twinBrother = person.MapSelf(x => x.FirstName, firstName => "John").Build();

You can also create an anoymous type that maps for you with the With extension.
However note that, although this pattern is cleaner and easier to read, FirstName is dynamic in the With statement and intellisense will not assist in finding the correct property name here.
This also follows a similar pattern used with records in .NET 5.0 / C# 9.0, which may make transitioning easier and or legacy immutable mappings look more modern.

    var twinBrother = person.With(new { FirstName = "John" }); 

## Mapping one type to another where property names are the same
Example:  ImmutableTypeA mapped to TypeB (even non immutable) then ImmutableTypeA.FirstName would automatically map to TypeB.FirstName.

You can make a mapping and build to produce the result without any custom mapping extensions to get a one to one mapping like so:

    var b = a.Map<ImmutableTypeA, TypeB>().Build();

Alternatively you can use the As<> extension that performs the same task but is simpler and easier to code and read.

    var b = a.As<B>();

You can even continue this mapping with a change to any specific fields on the out bound like so:

    var b = a.As<B>().With(x => x.FirstName, firstName = "John");

However; note that the above step is ok for smaller mappings but not as good as using the Map<TSource, TTarget>().Build() extensions.  When using the Map<TSource, TTarget> extension all mappings are stored and triggered once during the Build operation, producing one result.  The above example using the As<T> extension will actually produce the result and then remap that same result with the With extension.  This internally is 2 operations.  Point being, if you intend to have more than one operation it is recommended to start with the Map<TSource, TTarget> extension.

## Mapping types with variances in property names
If TypeB has a different spelling of FirstName, such as First_Name then you could also do this.

    var b = a.Map<ImmutableTypeA, TypeB>()
        .MapPropertyName(source => source.FirstName, target => target.First_Name)
        .Build();

You can apply as many maps as needed before calling build.

## Defining logic for specific property mappings

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

## Storing maps for later
A mapping can also be stored for usage later and used like so.

    var bMapping = a.Map<ImmutableTypeA, TypeB>()
        .MapPropertyName(source => source.FirstName, target => target.First_Name)
        .MapProperty(source => source.Members, members => members.Tolist()); //We just left out the Build statement.
        
    //later we can then build our map at anytime
    var b = bMapping.Build();

## Defining logic for properties with specific attributes
If you want to map against a specific attribute you can do so by adding the mapping for one. In this case we have a TrimAttribute on some of our source properties and during the build process we trim the strings that have that attribute applied to the source property. There is also a MapTargetAttribute extension.

    person = person.Map<PersonDto, Person>
        .MapSourceAttribute<PersonDto, Person, TrimAttribute>((attribute, sourceValue) => sourceValue is string str ? str.Trim() : sourceValue)
        .Build();

## Ignoring property name case and exceptions
By default ImmutaMap uses case sensitive property mapping. If you want to ignore the property casing between types you can set the default parameter when the mapping begins.
Likewise, ImmutaMap will throw exceptions for types it can't propertly map by default.  You can also ignore these exceptions forcing ImmutaMap to skip these failed properties if you desire.
Examples:

    var b = a.Map<A, B>(ignoreCase: true, throwExceptions: false).Build();

    var b = a.As<B>(ignoreCase: true, throwExceptions: false);

## Make a custom mapping
You can use the MapCustom extension method to add your own custom mapping logic.
The interface is IMapping and has a single method 

    bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue, out object result)

The method is called for every property during the build process and is chained inline with any other mappings.
Here's a UpperCase() custom mapping type and how it looks.

    public class UpperCaseMap : ImmutaMap.Interfaces.IMapping
    {
        public bool TryGetValue<TSource>(TSource source, PropertyInfo sourcePropertyInfo, PropertyInfo targetPropertyInfo, object previouslyMappedValue, out object result)
        {
            if (sourcePropertyInfo.PropertyType == typeof(string))
            {
                if (previouslyMappedValue is string str)
                {
                    result = str.ToUpper();
                }
                else
                {
                    result = ((string)sourcePropertyInfo.GetValue(source)).ToUpper();
                }
                return true;
            }
            else
            {
                result = default;
                return false;
            }
        }
    }

Now we can automatically uppercase all strings during the mapping by applying this custom UpperCaseMap() to our mapping chain.

    var typeB = typeA
        .MapCustom(new UpperCaseMap())
        .Build();
   
# Known Issues:
At this time mapping is not recursive.  If you have a nested type and the mapping of that type is required, then the mapping will be manually written down the chain or conquered prior to the mapping logic in then placed inline.

Example:

    public class PersonA
    {
        //firstname, lastname etc...
        
        //In this case, since FamilyMembers is enumeration of Person and each family member can also have FamilyMembers.
        //Recusively mapping the members is not yet supported if you are mapping from PersonA to PersonB where one is IEnumerable and the other is ICollection.
        public IEnumerable<PersonA> FamilyMembers { get; }
    }
    
    
    public class PersonB
    {
        //firstname, lastname etc...
        
        public ICollection<PersonB> FamilyMembers { get; }
    }
    
    //......
    
    //Note that, as of now, the MapProperty only goes one deep.  If each family member, has has family members of it's own, then it requires distinct mapping down the pipe.
    var personB = personA
        .Map<PersonA, PersonB>()
        .MapProperty(source => source.FamilyMembers, familyMembers => familyMembers.ToList())
        .Build();

# Reference

    With<T>, With<TSource, TTarget>, With<T, TSourcePropertyType> and the As<T> extensions produce a result immediately and, although they can be chained, each use of With<...> or As<T> will return an instantiated type before moving to the next event.

    Map<TSource, TTarget> and all extensions beginning with the word Map are lazy.  They will only produce one single result but must be closed calling the Build extension to do so.
