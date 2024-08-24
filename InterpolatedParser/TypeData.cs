namespace InterpolatedParser;

public enum TypeDataKind {
    Parsable,
    SpanParsable,
    Array,
    List,
}

public record class TypeData(string FullName, TypeDataKind Kind, TypeData? InnerType);

