using System.Globalization;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace InterpolatedParsing;

[InterpolatedStringHandler]
unsafe public partial struct InterpolatedParseStringHandler {

    private delegate void ParseAction(ReadOnlySpan<char> input, int alignment, string? format, void* pointer);

    private readonly struct Entry {
        public readonly void* Pointer;
        public readonly int Alignment;
        public readonly string? Format;
        public readonly ParseAction Action;

        public Entry(void* pointer, int alignment, string? format, ParseAction action) {
            Pointer = pointer;
            Alignment = alignment;
            Format = format;
            Action = action;
        }
    }

    private readonly List<Entry> _entries;
    private readonly List<string> _breaks;

    private string? _preString;


    public InterpolatedParseStringHandler(int _, int formattedCount) {
        _entries = new(formattedCount);
        _breaks = new(formattedCount);
    }

    public void AppendLiteral(string literal) {
        if (_entries.Count == 0) {
            _preString = literal;
            return;
        }

        if (_entries.Count - _breaks.Count != 1) {
            throw new Exception("Every formatted variable needs at least one character of seperator between them.");
        }
        _breaks.Add(literal);
    }


    public readonly void AppendFormatted<T>(in T value) where T: IParsable<T> {
        void* p = Unsafe.AsPointer(ref Unsafe.AsRef(in value));
        AppendFormatted<T>(p, in value, 0, null);
    }


    private readonly void AppendFormatted<T>(void* pointer, in T value, int alignment, string? format) where T : IParsable<T> {
        _entries.Add(new(pointer, alignment, format, static (chars, alignment, format, p) => {
            var typedPointer = CastPointer<T>(p);

            var culture = CultureInfo.InvariantCulture;

            *typedPointer = T.Parse(new string(chars), CultureInfo.InvariantCulture);
        }));
    }

#pragma warning disable CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    private static T* CastPointer<T>(void* pointer) {
        return (T*)pointer;
#pragma warning restore CS8500 // This takes the address of, gets the size of, or declares a pointer to a managed type
    }

    internal readonly void Execute(string input) {
        int readIndex = 0;
        if (_preString != null) {
            if (input.StartsWith(_preString) == false) {
                throw new Exception($"Expected input to start with {_preString}");
            }
            readIndex = _preString.Length;
        }

        var inputSpan = input.AsSpan();

        for (int i=0; i< _entries.Count; i++) { 
            var entry = _entries[i];
            // If last entry and no post, send remaining string
            if (i == _entries.Count - 1 && _breaks.Count < _entries.Count) {
                entry.Action(inputSpan[readIndex..], entry.Alignment, entry.Format, entry.Pointer);
                continue;
            }

            var @break = _breaks[i];
            var indexOfNext = input.IndexOf(@break, readIndex);

            if (indexOfNext == -1) {
                throw new Exception($"The seperator \"{@break}\" was not found inside the remainder of the expression: {input[..readIndex]}");
            }

            entry.Action(inputSpan[readIndex ..indexOfNext], entry.Alignment, entry.Format, entry.Pointer);
            readIndex = indexOfNext + @break.Length;
        }
    }
}
