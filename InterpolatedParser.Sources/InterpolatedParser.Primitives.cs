namespace InterpolatedParsing {
	partial class InterpolatedParser {
		partial struct InterpolatedParseStringHandler {
			public void AppendLiteral(string literal) {
				_currentStringPosition += literal.Length;
			}

			// string
			public void AppendFormatted(in string value) {
				var nextPart = GetNextPart();
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