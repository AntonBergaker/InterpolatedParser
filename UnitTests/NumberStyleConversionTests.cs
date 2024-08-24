using InterpolatedParsing;

namespace UnitTests;

public class NumberStyleConversionTests {

    private void AssertParsedNumberAreEqual(int number, string style) {
        string str = number.ToString(style);
        var numberStyles = InterpolatedParser.InterpolatedParseStringHandler.GetNumberStylesFromString(style);
        int parsedNumber = int.Parse(str, numberStyles);
        Assert.AreEqual(number, parsedNumber);
    }
    
    [Test]
    public void Hex() {

        AssertParsedNumberAreEqual(123, "X");

    }
}
