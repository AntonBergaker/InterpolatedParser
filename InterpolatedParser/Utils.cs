using System.Globalization;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("UnitTests")]

namespace InterpolatedParsing;


internal static class Utils {

    public static NumberStyles GetNumberStylesFromString(string? style) {

        if (style == null) {
            return NumberStyles.Integer;
        }

        var styles = NumberStyles.Integer;

        if (style.StartsWith("x", StringComparison.CurrentCultureIgnoreCase)) {
            styles = NumberStyles.HexNumber;
        }

        return styles;
    }

}
