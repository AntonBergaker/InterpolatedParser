
# InterpolatedParser

**DO NOT USE THIS LIBRARY IN PRODUCTION**  
The way InterpolatedParser works is by using unsafe code that is very volatile. Do not use this outside small short lived executions to test things or parse simple strings. It's meant more of a weird language showcase than an actual product.

InterpolatedParser is a [nuget](https://github.com/AntonBergaker/InterpolatedParser) library enabling string interpolation, but in reverse.

Example code:
```csharp
using InterpolatedParsing;

int x = 0;

string input = "x is 69";

InterpolatedParser.Parse($"x is {x}", input);

Console.WriteLine(x); // Prints 69.
```

## Usage

### Limitations
**Make sure any variable you parse into is a variable living on the stack. The reason why is explained below.** 

### Supported types
InterpolatedParser supports anything that implements IParseable<T>, which includes many common types in .NET. This also means you can use your own types by having them implement IParseable<T>.

### Collections
The parser supports Lists and Arrays. A separator is provided as a format string. Format strings don't allow trailing whitespace, so if you need that enclose the format string in single quotes.

```csharp
using InterpolatedParsing;

List<int> numbers = null!;

InterpolatedParser.Parse(
	$"Winning numbers are: {x:,}",
	"Winning numbers are: 5,10,15,25);

List<string> beans = null!;
InterpolatedParser.Parse(
	$"Bean list: {x:', '}", // Add single quotes to support whitespace
	"Bean list: "black", "coffee", "green");

```


## This is cursed, how does it do that?
C# 10 added support for writing [custom interpolated string handlers](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/interpolated-string-handler). At compile time they translate interpolated strings into a series of calls to magic methods: AppendLiteral for literal strings, and AppendFormatted to the parameters of the string.

```csharp
var str = $"Hello {123}!";

// Becomes this code on compile time:

var handler = new DefaultInterpolatedStringHandler(7, 4);
handler.AppendLiteral("Hello ");
handler.AppendFormatted(123);
handler.AppendLiteral("!");
var str = handler.ToStringAndClear();
```

That's all good and normally doesn't enable the shenanigans we need. However we can abuse the [in](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/method-parameters#in-parameter-modifier) parameter modifier. The in parameter can be implicit, so the generated calls to AppendFormatted allow it. This means we're now passing down a read only reference to the value when we call AppendFormatted. This is where things become really cursed, using unsafe code it's casted down to a void\* so we can keep a reference to it.

```csharp
    public readonly void AppendFormatted(in int value) {
        void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));
```

As you might imagine this is very unsafe and volatile. Because the reference is needed between method calls, the pointer can also not be pinned so it can be moved around by garbage collection. That's why it's very important to only use this for stack variables. 
