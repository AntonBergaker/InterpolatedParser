using InterpolatedParsing;

namespace UnitTests;
internal class CollectionTests {
    [Test]
    public void IntArray() {
        int[] array = default!;

        InterpolatedParser.Parser.Parse(
             "1,2,4,8,16",
            $"{array:,}"
        );

        Assert.AreEqual(new int[] { 1, 2, 4, 8, 16 }, array);
    }

    [Test]
    public void StringArray() {
        string[] array = default!;

        InterpolatedParser.Parser.Parse(
             "i am cool",
            $"{array:' '}"
        );

        Assert.AreEqual(new string[] {"i", "am", "cool" }, array);
    }
}
