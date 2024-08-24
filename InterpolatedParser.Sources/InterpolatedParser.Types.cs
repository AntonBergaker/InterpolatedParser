namespace InterpolatedParsing {
    partial class InterpolatedParser {
        partial struct InterpolatedParseStringHandler {
            public void AppendFormatted(in System.Guid value) {
                var nextPart = GetNextPart();
                System.Runtime.CompilerServices.Unsafe.AsRef(in value) = System.Guid.Parse(nextPart);
            }


        }
    }
}
