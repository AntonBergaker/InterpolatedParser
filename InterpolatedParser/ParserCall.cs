using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace InterpolatedParser;
internal class ParserCall(TypeData[] types, string fileLocation, int lineStart, int lineEnd, string[] components) : IEquatable<ParserCall?> {
    public TypeData[] Types { get; } = types;
    public string[] Components { get; } = components;
    public string FileLocation { get; } = fileLocation;
    public int LineStart { get; } = lineStart;
    public int LineEnd { get; } = lineEnd;

    public override bool Equals(object? obj) {
        return Equals(obj as ParserCall);
    }

    public bool Equals(ParserCall? other) {
        return other is not null &&
            FileLocation == other.FileLocation &&
            LineStart == other.LineStart &&
            LineEnd == other.LineEnd &&
            Enumerable.SequenceEqual(Types, other.Types) &&
            Enumerable.SequenceEqual(Components, other.Components);
    }

    public override int GetHashCode() {
        int hashCode = 1230013848;

        foreach (var t in Types) {
            hashCode = hashCode * -1521134295 + t.GetHashCode();
        }
        foreach (var c in Components) {
            hashCode = hashCode * -1521134295 + c.GetHashCode();
        }

        hashCode = hashCode * -1521134295 + FileLocation.GetHashCode();
        hashCode = hashCode * -1521134295 + LineStart.GetHashCode();
        hashCode = hashCode * -1521134295 + LineEnd.GetHashCode();
        return hashCode;
    }
}
