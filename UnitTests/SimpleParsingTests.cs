using InterpolatedParsing;
using NUnit.Framework.Internal;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace UnitTests;

public class SimpleParsingTests {

    [Test]
    public void Single() {
        int x = 0;

        InterpolatedParser.Parse(
             "69",
            $"{x}"
        );

        Assert.AreEqual(69, x);
    }

    [Test]
    public void SimpleInts() {
        int x = 0;
        int y = 0;

        InterpolatedParser.Parse(
             "x=512, y=123",
            $"x={x}, y={y}"
        );

        Assert.AreEqual(512, x);
        Assert.AreEqual(123, y);
    }

    [Test]
    public void NoPrestring() {
        int x = 0;

        InterpolatedParser.Parse(
             "1337 is the ticket!",
            $"{x } is the ticket!"
        );

        Assert.AreEqual(1337, x);
    }

    [Test]
    public void Stacked() {
        int x = 0;

        InterpolatedParser.Parse("123 woop", $"{x} woop");
        InterpolatedParser.Parse("123 woop", $"{x} woop");
        InterpolatedParser.Parse("123 woop", $"{x} woop");

        Assert.AreEqual(123, x);
    }

    [Test]
    public void Strings() {
        {
            string flavor = "";

            InterpolatedParser.Parse(
                 "Favorite ice cream: vanilla",
                $"Favorite ice cream: {flavor}"
            );

            Assert.AreEqual("vanilla", flavor);
        }

        {
            string flavor0 = "";
            string flavor1 = "";

            InterpolatedParser.Parse(
                 "One cone with vanilla and chocolate please",
                $"One cone with {flavor0} and {flavor1} please"
            );

            Assert.AreEqual("vanilla", flavor0);
            Assert.AreEqual("chocolate", flavor1);
        }
    }
}