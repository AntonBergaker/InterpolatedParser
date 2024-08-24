using InterpolatedParsing;

namespace UnitTests;
internal class InvalidInputsTests {

    [Test]
    public void InvalidStart() {
        Assert.Catch(() => {
            int x = 0;
            InterpolatedParser.Parser.Parse(
                "too different 12",
                $"something {x}"
            );
        });

        Assert.Catch(() => {
            int x = 0;
            InterpolatedParser.Parser.Parse(
                "12",
                $"too much {x}"
                );
        });

        Assert.Catch(() => {
            int x = 0;
            InterpolatedParser.Parser.Parse(
                "missing 12",
                $"{x}"
                );
        });
    }
}
