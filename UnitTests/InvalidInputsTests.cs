using InterpolatedParsing;

namespace UnitTests;
internal class InvalidInputsTests {

    [Test]
    public void InvalidStart() {
        Assert.Catch(() => {
            int x = 0;
            InterpolatedParser.Parse(
                $"something {x}",
                 "too different 12");
        });

        Assert.Catch(() => {
            int x = 0;
            InterpolatedParser.Parse(
                $"too much {x}",
                 "12");
        });

        Assert.Catch(() => {
            int x = 0;
            InterpolatedParser.Parse(
                $"{x}",
                 "missing 12");
        });
    }
}
