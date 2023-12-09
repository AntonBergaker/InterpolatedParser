using System.Globalization;
using System.Runtime.CompilerServices;

namespace InterpolatedParsing;
unsafe partial struct InterpolatedParseStringHandler {

	// string
	public readonly void AppendFormatted(in string value) {
		void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, 0, null, static (chars, alignment, format, p) => {
            var typedPointer = CastPointer<string>(p);
            *typedPointer = new string(chars);
        }));
    }

    // char
    public readonly void AppendFormatted(in char value) {
        void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, 0, null, static (chars, alignment, format, p) => {
            var typedPointer = CastPointer<char>(p);
            *typedPointer = chars[0];
        }));
    }


    private readonly void AppendFormattedSpan<T>(void* pointer, int alignment = 0, string? format = default) where T: ISpanParsable<T> {
		_entries.Add(new(pointer, alignment, format, static (chars, alignment, style, p) => {
            var typedPointer = CastPointer<T>(p);
            *typedPointer = T.Parse(chars, CultureInfo.InvariantCulture);
		}));
	}

	// bool
	public readonly void AppendFormatted(in bool value) {
		void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));
		AppendFormattedSpan<bool>(pointer);
	}


    // byte
    public readonly void AppendFormatted(in byte value) {
        AppendFormatted(in value, 0, default);
    }
    public readonly void AppendFormatted(in byte value, int alignment = 0, string? format = default) {
		void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, alignment, format, static (chars, alignment, style, p) => {
            var typedPointer = CastPointer<byte>(p);
            *typedPointer = byte.Parse(chars, Utils.GetNumberStylesFromString(style));
        }));
	}

    // sbyte
    public readonly void AppendFormatted(in sbyte value) {
        AppendFormatted(in value, 0, default);
    }
    public readonly void AppendFormatted(in sbyte value, int alignment = 0, string? format = default) {
        void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, alignment, format, static (chars, alignment, style, p) => {
            var typedPointer = CastPointer<sbyte>(p);
            *typedPointer = sbyte.Parse(chars, Utils.GetNumberStylesFromString(style));
        }));
    }

    // short
    public readonly void AppendFormatted(in short value) {
        AppendFormatted(in value, 0, default);
    }
    public readonly void AppendFormatted(in short value, int alignment = 0, string? format = default) {
		void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, alignment, format, static (chars, alignment, style, p) => {
            var typedPointer = CastPointer<short>(p);
            *typedPointer = short.Parse(chars, Utils.GetNumberStylesFromString(style));
        }));
    }

    // ushort
    public readonly void AppendFormatted(in ushort value) {
        AppendFormatted(in value, 0, default);
    }
    public readonly void AppendFormatted(in ushort value, int alignment = 0, string? format = default) {
		void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, alignment, format, static (chars, alignment, style, p) => {
            var typedPointer = CastPointer<ushort>(p);
            *typedPointer = ushort.Parse(chars, Utils.GetNumberStylesFromString(style));
        }));
    }

    // int
    public readonly void AppendFormatted(in int value) {
        AppendFormatted(value, 0, default);
    }

    public readonly void AppendFormatted(in int value, int alignment = 0, string? format = default) {
		void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, alignment, format, static (chars, alignment, style, p) => {
            var typedPointer = CastPointer<int>(p);
            *typedPointer = int.Parse(chars, Utils.GetNumberStylesFromString(style));
        }));
    }

    // uint
    public readonly void AppendFormatted(in uint value) {
        AppendFormatted(in value, 0, default);
    }
    public readonly void AppendFormatted(in uint value, int alignment = 0, string? format = default) {
		void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, alignment, format, static (chars, alignment, style, p) => {
            var typedPointer = CastPointer<uint>(p);
            *typedPointer = uint.Parse(chars, Utils.GetNumberStylesFromString(style));
        }));
    }

    // long
    public readonly void AppendFormatted(in long value) {
        AppendFormatted(in value, 0, default);
    }
    public readonly void AppendFormatted(in long value, int alignment = 0, string? format = default) {
		void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, alignment, format, static (chars, alignment, style, p) => {
            var typedPointer = CastPointer<long>(p);
            *typedPointer = long.Parse(chars, Utils.GetNumberStylesFromString(style));
        }));
    }

    // ulong
    public readonly void AppendFormatted(in ulong value) {
        AppendFormatted(in value, 0, default);
    }
    public readonly void AppendFormatted(in ulong value, int alignment = 0, string? format = default) {
		void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, alignment, format, static (chars, alignment, style, p) => {
            var typedPointer = CastPointer<ulong>(p);
            *typedPointer = ulong.Parse(chars, Utils.GetNumberStylesFromString(style));
        }));
    }

    // float
    public readonly void AppendFormatted(in float value) {
        AppendFormatted(in value, 0, default);
    }
    public readonly void AppendFormatted(in float value, int alignment = 0, string? format = default) {
		void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, alignment, format, static (chars, alignment, style, p) => {
            var typedPointer = CastPointer<float>(p);
            *typedPointer = float.Parse(chars, Utils.GetNumberStylesFromString(style));
        }));
    }

    // double
    public readonly void AppendFormatted(in double value) {
        AppendFormatted(in value, 0, default);
    }
    public readonly void AppendFormatted(in double value, int alignment = 0, string? format = default) {
		void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, alignment, format, static (chars, alignment, style, p) => {
            var typedPointer = CastPointer<double>(p);
            *typedPointer = double.Parse(chars, Utils.GetNumberStylesFromString(style));
        }));
    }

    // decimal
    public readonly void AppendFormatted(in decimal value) {
        AppendFormatted(in value, 0, default);
    }
    public readonly void AppendFormatted(in decimal value, int alignment = 0, string? format = default) {
		void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, alignment, format, static (chars, alignment, style, p) => {
            var typedPointer = CastPointer<decimal>(p);
            *typedPointer = decimal.Parse(chars, Utils.GetNumberStylesFromString(style));
        }));
    }
}

