using InterpolatedParsing;

namespace UnitTests;
internal class InvalidInputsTests {

    [Test]
    public void InvalidStart() {
        Assert.Catch(() => {
            int x = 0;
            InterpolatedParser.Parse(
                "too different 12",
                $"something {x}"
            );
        });

        Assert.Catch(() => {
            int x = 0;
            InterpolatedParser.Parse(
                "12",
                $"too much {x}"
                );
        });

        Assert.Catch(() => {
            int x = 0;
            InterpolatedParser.Parse(
                "missing 12",
                $"{x}"
                );
        });
    }
}
