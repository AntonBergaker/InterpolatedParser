using InterpolatedParsing;
using NUnit.Framework.Internal;
using System.Numerics;

namespace UnitTests;

public class SimpleParsingTests {

    [Test]
    public void Single() {
        int x = 0;

        InterpolatedParser.Parse(
            $"{x}",
             "69");

        Assert.AreEqual(69, x);
    }

    [Test]
    public void SimpleInts() {
        int x = 0;
        int y = 0;

        InterpolatedParser.Parse(
            $"x={x}, y={y}",
             "x=512, y=123");

        Assert.AreEqual(512, x);
        Assert.AreEqual(123, y);
    }

    [Test]
    public void NoPrestring() {
        int x = 0;

        InterpolatedParser.Parse(
            $"{x} is the ticket!",
             "1337 is the ticket!");

        Assert.AreEqual(1337, x);
    }

    [Test]
    public void Strings() {
        {
            string flavor = "";

            InterpolatedParser.Parse(
                $"Favorite ice cream: {flavor}",
                "Favorite ice cream: vanilla");

            Assert.AreEqual("vanilla", flavor);
        }

        {
            string flavor0 = "";
            string flavor1 = "";

            InterpolatedParser.Parse(
                $"One cone with {flavor0} and {flavor1} please",
                "One cone with vanilla and chocolate please");

            Assert.AreEqual("vanilla", flavor0);
            Assert.AreEqual("chocolate", flavor1);
        }
    }
}