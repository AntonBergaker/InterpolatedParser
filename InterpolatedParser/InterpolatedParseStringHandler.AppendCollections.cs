using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace InterpolatedParsing;

unsafe partial struct InterpolatedParseStringHandler {

    private static List<T> GetListFromChars<T>(ReadOnlySpan<char> chars, ReadOnlySpan<char> seperator) where T: IParsable<T> {
        if (seperator[0] == '\'' && seperator[^1] == '\'') {
            seperator = seperator[1..^1];
        }

        var list = new List<T>();

        while (true) {
            int index = chars.IndexOf(seperator);
            if (index == -1) {
                list.Add(T.Parse(new string(chars), CultureInfo.InvariantCulture));
                break;
            }

            list.Add(T.Parse(new string(chars[..index]), CultureInfo.InvariantCulture));
            chars = chars[(index + seperator.Length)..];
           
        }

        return list;
    }

    // array
    public readonly void AppendFormatted<T>(in T[] value, int alignment = 0, string? format = null) where T: IParsable<T> {
        if (format == null || format.Length == 0) {
            throw new ArgumentException("An array needs a seperator provided in the format string provided.", nameof(format));
        }

        void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, alignment, format, static (chars, alignment, format, p) => {
            var list = GetListFromChars<T>(chars, format!);

            var typedPointer = CastPointer<T[]>(p);

            *typedPointer = list.ToArray();
        }));
    }

    // list
    public readonly void AppendFormatted<T>(in List<T> value, int alignment = 0, string? format = null) where T : IParsable<T> {
        if (format == null || format.Length == 0) {
            throw new ArgumentException("A list needs a seperator provided in the format string provided.", nameof(format));
        }

        void* pointer = Unsafe.AsPointer(ref Unsafe.AsRef(in value));

        _entries.Add(new(pointer, alignment, format, static (chars, alignment, format, p) => {
            var list = GetListFromChars<T>(chars, format!);

            var typedPointer = CastPointer<List<T>>(p);

            *typedPointer = list;
        }));
    }
}
