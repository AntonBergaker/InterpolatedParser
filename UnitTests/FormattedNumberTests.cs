using InterpolatedParsing;

namespace UnitTests;
public class FormattedNumberTests {
    [Test]
    public void FormattedInts() {
        {
            int x = 0x123;

            InterpolatedParser.Parse(
                $"{x:X}",
                 "123");

            Assert.AreEqual(0x123, x);
        }

        {
            int x = 0xABC;

            InterpolatedParser.Parse(
                $"{x:X}",
                 "ABC");

            Assert.AreEqual(0xABC, x);
        }
    }

    public void FormattedOtherTypes() {
        
        byte x = 0xCC;

        InterpolatedParser.Parse(
            $"{x:X}",
                "CC");

        Assert.AreEqual(0xCC, x);
        


    }

}
