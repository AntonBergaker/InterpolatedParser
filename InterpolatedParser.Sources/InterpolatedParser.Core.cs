
namespace InterpolatedParsing {
    internal static partial class InterpolatedParser {
        public static void Parse(string input,
            [System.Runtime.CompilerServices.InterpolatedStringHandlerArgument("input")] InterpolatedParseStringHandler template) {
            // Do nothing. Building the string already did all the fun stuff.
        }


        [System.Runtime.CompilerServices.InterpolatedStringHandler]
        public partial struct InterpolatedParseStringHandler {
            private class InterpolatedStringSource {
                public readonly string[] Components;

                public InterpolatedStringSource(string[] components) {
                    Components = components;
                }
            }

            private static readonly System.Collections.Generic.Dictionary<(string File, int Line), InterpolatedStringSource> _sources = GetSourcesLineExpanded();

            private static System.Collections.Generic.Dictionary<(string File, int Line), InterpolatedStringSource> GetSourcesLineExpanded() {
                var dict = new System.Collections.Generic.Dictionary<(string File, int Line), InterpolatedStringSource>();
                var sources = GetInterpolatedStringSources();
                foreach (var source in sources) {
                    var range = source.Key.Line;
                    for (int i = range.Start.Value; i <= range.End.Value; i++) {
                        dict.Add((source.Key.File, i), source.Value);
                    }
                }

                return dict;
            }

            private int _currentIndex;
            private int _currentStringPosition;
            private readonly string _inputString;
            private readonly InterpolatedStringSource _source;

            public InterpolatedParseStringHandler(
                // All these optional inputs are required so we don't accidentally fill the source file path and source line number.
                int _literalLength,
                int _formattedCount,
                string input,
                out bool shouldFormat,
                [System.Runtime.CompilerServices.CallerFilePath] string sourceFilePath = "",
                [System.Runtime.CompilerServices.CallerLineNumber] int sourceLineNumber = 0
            ) {

                shouldFormat = true;

                _currentIndex = 0;
                _currentStringPosition = 0;
                _inputString = input;
                if (_sources.TryGetValue((sourceFilePath, sourceLineNumber), out _source!) == false) {
                    throw new System.Exception(
                        $"Unable to find source string for {sourceFilePath}:{sourceLineNumber}. " +
                        "This can occur if the source generator resolves the line that calls Parse incorrectly. " +
                        "Make sure there's only one InterpolatedParser.Parse call on its line."
                    );
                }
            }

            private System.ReadOnlySpan<char> GetNextPart() {
                // Final bit
                if (_currentIndex >= _source.Components.Length) {
                    if (_currentStringPosition > _inputString.Length) {
                        throw new System.Exception("Tried to read beyond the size of the input string. This usually means the provided string is missing parts of the template.");
                    }
                    return System.MemoryExtensions.AsSpan(_inputString, _currentStringPosition, _inputString.Length - _currentStringPosition);
                }

                string nextPart = _source.Components[_currentIndex++];
                int index = _inputString.IndexOf(nextPart, _currentStringPosition);

                if (index <= -1) {
                    throw new System.Exception(
                        $"Failed to find the next part of the template: \"{nextPart}\" in the remainder of the input string. " +
                        $"Make sure the template matches the input string, and that any previous parsed part does not contain the substring: \"{nextPart}\""
                    );
                }

                var startPos = _currentStringPosition;
                _currentStringPosition = index;
                return System.MemoryExtensions.AsSpan(_inputString, startPos, index - startPos);

            }

#if NET7_0_OR_GREATER
            private static System.Collections.Generic.List<T> GetListFromCharsParsable<T>(System.ReadOnlySpan<char> chars, System.ReadOnlySpan<char> separator) where T : System.IParsable<T> {
                if (separator[0] == '\'' && separator[^1] == '\'') {
                    separator = separator[1..^1];
                }

                var list = new System.Collections.Generic.List<T>();

                while (true) {
                    int index = System.MemoryExtensions.IndexOf(chars, separator);
                    if (index == -1) {
                        list.Add(T.Parse(new string(chars), System.Globalization.CultureInfo.InvariantCulture));
                        break;
                    }

                    list.Add(T.Parse(new string(chars[..index]), System.Globalization.CultureInfo.InvariantCulture));
                    chars = chars[(index + separator.Length)..];

                }

                return list;
            }

            private static System.Collections.Generic.List<T> GetListFromCharsSpanParsable<T>(System.ReadOnlySpan<char> chars, System.ReadOnlySpan<char> separator) where T : System.ISpanParsable<T> {
                if (separator[0] == '\'' && separator[^1] == '\'') {
                    separator = separator[1..^1];
                }

                var list = new System.Collections.Generic.List<T>();

                while (true) {
                    int index = System.MemoryExtensions.IndexOf(chars, separator);
                    if (index == -1) {
                        list.Add(T.Parse(chars, System.Globalization.CultureInfo.InvariantCulture));
                        break;
                    }

                    list.Add(T.Parse(chars[..index], System.Globalization.CultureInfo.InvariantCulture));
                    chars = chars[(index + separator.Length)..];

                }

                return list;
            }
#endif
        }
    }
}