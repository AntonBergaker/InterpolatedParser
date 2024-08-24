namespace InterpolatedParsing {
	partial class InterpolatedParser {
		partial struct InterpolatedParseStringHandler {
			private static System.Collections.Generic.Dictionary<(string File, System.Range Line), InterpolatedStringSource> GetInterpolatedStringSources() {
				return new() {
					{("REDACTED FOR GIT", 7..10), new(new [] {"! You are ", " years old" }) },
                };
			}

		}
	}
}
