using InterpolatedParsing;

namespace UnitTests;
public class FormattedNumberTests {
    [Test]
    public void FormattedInts() {
        {
            int x = 0x123;

            InterpolatedParser.Parser.Parse(
                 "123",
                $"{x:X}"
            );

            Assert.AreEqual(0x123, x);
        }

        {
            int x = 0xABC;

            InterpolatedParser.Parser.Parse(
                 "ABC",
                $"{x:X}"
                );

            Assert.AreEqual(0xABC, x);
        }
    }

    public void FormattedOtherTypes() {
        byte x = 0xCC;

        InterpolatedParser.Parser.Parse(
             "CC",
            $"{x:X}"
        );

        Assert.AreEqual(0xCC, x);
    }

}
