//* <- Add comment here to switch between string and readable
namespace InterpolatedParser;

public static class InitializationContent {
    public static string GetContent() =>
"""
/**/
#nullable enable


namespace InterpolatedParsing {
    internal static partial class InterpolatedParser {
        public static void Parse(string input,
            [System.Runtime.CompilerServices.InterpolatedStringHandlerArgument("input")] InterpolatedParseStringHandler stringHandler) {
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
                    throw new System.Exception($"Unable to find source string for {sourceFilePath}:{sourceLineNumber}.");
                }
            }

            private System.ReadOnlySpan<char> GetNextPart() {
                // Final bit
                if (_currentIndex >= _source.Components.Length) {
                    return System.MemoryExtensions.AsSpan(_inputString, _currentStringPosition, _inputString.Length - _currentStringPosition);
                }

                string nextPart = _source.Components[_currentIndex++];
                int index = _inputString.IndexOf(nextPart, _currentStringPosition);

                if (index <= -1) {
                    throw new System.Exception("Failed to find the substring of the next part. This shouldn't be possible.");
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



namespace InterpolatedParsing {
	partial class InterpolatedParser {
		partial struct InterpolatedParseStringHandler {
			public void AppendLiteral(string literal) {
				_currentStringPosition += literal.Length;
			}

			// string
			public void AppendFormatted(in string value) {
				var nextPart = GetNextPart();
				// Errors here, but is implemented everywhere interpolated string handling is.
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = new string(nextPart);
			}

			// char
			public void AppendFormatted(in char value) {
				var nextPart = GetNextPart();
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = nextPart[0];
			}

			// bool
			public void AppendFormatted(in bool value) {
				var nextPart = GetNextPart();
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = bool.Parse(nextPart);
			}


			// byte
			public void AppendFormatted(in byte value) {
				AppendFormatted(in value, 0, default);
			}
			public void AppendFormatted(in byte value, int alignment = 0, string? format = default) {
				var nextPart = GetNextPart();
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = byte.Parse(nextPart, GetNumberStylesFromString(format));
			}

			// sbyte
			public void AppendFormatted(in sbyte value) {
				AppendFormatted(in value, 0, default);
			}
			public void AppendFormatted(in sbyte value, int alignment = 0, string? format = default) {
				var nextPart = GetNextPart();
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = sbyte.Parse(nextPart, GetNumberStylesFromString(format));
			}

			// short
			public void AppendFormatted(in short value) {
				AppendFormatted(in value, 0, default);
			}
			public void AppendFormatted(in short value, int alignment = 0, string? format = default) {
				var nextPart = GetNextPart();
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = short.Parse(nextPart, GetNumberStylesFromString(format));
			}

			// ushort
			public void AppendFormatted(in ushort value) {
				AppendFormatted(in value, 0, default);
			}
			public void AppendFormatted(in ushort value, int alignment = 0, string? format = default) {
				var nextPart = GetNextPart();
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = ushort.Parse(nextPart, GetNumberStylesFromString(format));
			}

			// int
			public void AppendFormatted(in int value) {
				AppendFormatted(value, 0, default);
			}

			public void AppendFormatted(in int value, int alignment = 0, string? format = default) {
				var nextPart = GetNextPart();
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = int.Parse(nextPart, GetNumberStylesFromString(format));
			}

			// uint
			public void AppendFormatted(in uint value) {
				AppendFormatted(in value, 0, default);
			}
			public void AppendFormatted(in uint value, int alignment = 0, string? format = default) {
				var nextPart = GetNextPart();
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = uint.Parse(nextPart, GetNumberStylesFromString(format));
			}

			// long
			public void AppendFormatted(in long value) {
				AppendFormatted(in value, 0, default);
			}
			public void AppendFormatted(in long value, int alignment = 0, string? format = default) {
				var nextPart = GetNextPart();
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = long.Parse(nextPart, GetNumberStylesFromString(format));
			}

			// ulong
			public void AppendFormatted(in ulong value) {
				AppendFormatted(in value, 0, default);
			}
			public void AppendFormatted(in ulong value, int alignment = 0, string? format = default) {
				var nextPart = GetNextPart();
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = ulong.Parse(nextPart, GetNumberStylesFromString(format));
			}

			// float
			public void AppendFormatted(in float value) {
				AppendFormatted(in value, 0, default);
			}
			public void AppendFormatted(in float value, int alignment = 0, string? format = default) {
				var nextPart = GetNextPart();
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = float.Parse(nextPart, GetNumberStylesFromString(format));
			}

			// double
			public void AppendFormatted(in double value) {
				AppendFormatted(in value, 0, default);
			}
			public void AppendFormatted(in double value, int alignment = 0, string? format = default) {
				var nextPart = GetNextPart();
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = double.Parse(nextPart, GetNumberStylesFromString(format));
			}

			// decimal
			public void AppendFormatted(in decimal value) {
				AppendFormatted(in value, 0, default);
			}
			public void AppendFormatted(in decimal value, int alignment = 0, string? format = default) {
				var nextPart = GetNextPart();
				System.Runtime.CompilerServices.Unsafe.AsRef(in value) = decimal.Parse(nextPart, GetNumberStylesFromString(format));
			}

			public static System.Globalization.NumberStyles GetNumberStylesFromString(string? style) {
				if (style == null) {
					return System.Globalization.NumberStyles.Integer;
				}

				var styles = System.Globalization.NumberStyles.Integer;

				if (style.StartsWith("x", System.StringComparison.CurrentCultureIgnoreCase)) {
					styles = System.Globalization.NumberStyles.HexNumber;
				}

				return styles;
			}
		}
	}
}

//*
""";
}
/**/