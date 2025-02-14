# InterpolatedParser

InterpolatedParser is a [nuget](https://www.nuget.org/packages/InterpolatedParser/) library enabling string interpolation, but in reverse.

Example code:
```csharp
using InterpolatedParsing;

string input = "x is 69";

int x = 0;
InterpolatedParser.Parse(input, $"x is {x}");

Console.WriteLine(x); // Prints 69.
```

## Usage

### Warning
This library abuses [Unsafe.AsRef](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.unsafe.asref) in a way that violates the runtimes expectations. It is not recommended to use this in production code. This library is intended as a fun little example of how C# features and implementation details can be used to do something unexpected and interesting. I am currently working on a much less cursed libary with a much more sane API.

### Supported types
InterpolatedParser supports anything that implements `IParseable<T>` and `ISpanParseable<T>`, which includes many common types in .NET. This also means you can use your own types by having them implement either of the two interfaces.

### Collections
The parser supports Lists and Arrays. A separator is provided as a format string. Format strings don't allow trailing whitespace, so if you need that enclose the format string in single quotes.

```csharp
using InterpolatedParsing;

List<int> numbers = null!;

InterpolatedParser.Parse(
	"Winning numbers are: 5,10,15,25",
	$"Winning numbers are: {numbers:,}");

List<string> beans = null!;
InterpolatedParser.Parse(
	"Bean list: black, coffee, green");
	$"Bean list: {beans:', '}"); // Add single quotes to support whitespace
```


## This is cursed, how does it do that?
C# 10 added support for writing [custom interpolated string handlers](https://learn.microsoft.com/en-us/dotnet/csharp/whats-new/tutorials/interpolated-string-handler). At compile time they translate interpolated strings into a series of calls to magic methods: `AppendLiteral` for literal strings, and `AppendFormatted` to the parameters of the string.

```csharp
var str = $"Hello {123}!";

// Becomes this code on compile time:

var handler = new DefaultInterpolatedStringHandler(7, 4);
handler.AppendLiteral("Hello ");
handler.AppendFormatted(123);
handler.AppendLiteral("!");
var str = handler.ToStringAndClear();
```

That's all good and normally doesn't enable the shenanigans we need. However we can abuse the [in](https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/keywords/method-parameters#in-parameter-modifier) parameter modifier. The in parameter can be implicit, so the generated calls to `AppendFormatted` allow it. This means we're now passing down a read only reference to the value when we call `AppendFormatted`. This is where things become really cursed, using `Unsafe.AsRef` it's possible to cast it into a ref parameter, allowing the parser to change the parameters value.

```csharp
    public readonly void AppendFormatted(in int value) {
        Unsafe.AsRef(in value) = 123;
```

This is the main hack that makes this work, but when `AppendFormatted` is called, we don't yet have the information to extract what part of the input string we should parse. (Previous versions of this parser stored the ref as a pointer which was giga unsafe as it could not be pinned.) The information we need for that is getting the next part of the string, which is added with an `AppendLiteral` after the `AppendFormatted`. To get the upcoming literal string this library uses a [C# Source Generator](https://learn.microsoft.com/en-us/dotnet/csharp/roslyn-sdk/source-generators-overview) to make a list of every single call to the `Parse` method and what string components each `Parse` call will have. Since generated code lives in the user project, to make it accessible to the Parser the entire Parser is also code generated. This has some other benefits, like allowing code generation to support both `ISpanParsable` and `IParsable` types.

Of course even with a list of all calls we still need a way to pick the right call out of this list. Somewhat surprisingly there's a set of slightly obscure attributes that will inject the [file path](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerfilepathattribute) and the [line number](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.callerlinenumberattribute) of the calling method at compile time.
Even more surprisingly these attributes still work on the auto generated constructor of the custom string interpolater. One side effect of this is that the accuracy is limited to line number, so placing two calls to `Parse` on the same line will break the parser.

The input string also gets passed to the interpolater's constructor using another [attribute](https://learn.microsoft.com/en-us/dotnet/api/system.runtime.compilerservices.interpolatedstringhandlerargumentattribute) and so we have all the information we need before any calls to `AppendFormatted`.
