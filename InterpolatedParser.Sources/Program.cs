using System;
using InterpolatedParsing;

string name = null!;
int age = 0;

InterpolatedParser.Parser.Parse(
    "Hi Anton! You are 25 years old",
    $"Hi {name}! You are {age} years old"
);

Console.WriteLine(name);
Console.WriteLine(age);