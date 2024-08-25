using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Operations;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Threading;


namespace InterpolatedParser;

[Generator]
public class InterpolatedParser : IIncrementalGenerator {
    private const string ParseMethodName = "InterpolatedParsing.InterpolatedParser.Parse(string, InterpolatedParsing.InterpolatedParser.InterpolatedParseStringHandler)";

    public void Initialize(IncrementalGeneratorInitializationContext context) {
        //Debugger.Launch();
        context.RegisterPostInitializationOutput(InitializationOutput);

        var interpolationCalls = context.SyntaxProvider.CreateSyntaxProvider(FindParseCalls, FindParseCallsTransform)!.Where<ParserCall>(x => x is not null).Collect();

        context.RegisterSourceOutput(interpolationCalls, EmitParseCallsCode);

        var types = interpolationCalls.Select(FindTypes);
        context.RegisterSourceOutput(types, EmitTypesCode);
    }

    private void InitializationOutput(IncrementalGeneratorPostInitializationContext context) {
        context.AddSource("InterpolatedParser.Core.g.cs", InitializationContent.GetContent());
    }

    private bool FindParseCalls(SyntaxNode node, CancellationToken token) {
        if (node is not InvocationExpressionSyntax invocation) {
            return false;
        }
        return invocation.ArgumentList.Arguments.Count > 0;
    }

    private static HashSet<string> _builtInTypes = [
        "string",
        "char",
        "bool",
        "byte",
        "sbyte",
        "ushort",
        "short",
        "int",
        "uint",
        "long",
        "ulong",
        "float",
        "double",
        "decimal"
    ];

    private ParserCall? FindParseCallsTransform(GeneratorSyntaxContext context, CancellationToken token) {
        if (context.SemanticModel.GetOperation(context.Node, token) is not IInvocationOperation operation) {
            return null;
        }

        var symbolInfo = context.SemanticModel.GetSymbolInfo(context.Node, token);

        if (symbolInfo.Symbol?.ToString() != ParseMethodName) {
            return null;
        }

        if (operation.Arguments[1].Value is not IInterpolatedStringHandlerCreationOperation interpolatedString) {
            return null;
        }

        if (interpolatedString.ChildOperations.Last() is not IInterpolatedStringOperation stringOperation) {
            return null;
        }

        List<TypeData> types = new();
        List<string> components = new();
        bool isFirst = true;

        foreach (var addOperations in stringOperation.ChildOperations) {
            if (addOperations.Syntax is InterpolatedStringTextSyntax interpolatedStringText) {
                if (isFirst == false) {
                    components.Add(interpolatedStringText.TextToken.Text);
                }
                continue;
            }

            isFirst = false;

            if (addOperations.Syntax is not InterpolationSyntax interpolationSyntax) {
                continue;
            }
            if (interpolationSyntax.Expression is not IdentifierNameSyntax idNameSyntax) {
                continue;
            }
            
            var type = GetTypeData(context.SemanticModel.GetTypeInfo(idNameSyntax).Type, false);
            if (type != null) {
                types.Add(type);
            }
        }

        var lineSpan = context.Node.GetLocation().GetLineSpan();

        return new(types.ToArray(), context.Node.SyntaxTree.FilePath, lineSpan.StartLinePosition.Line + 1, lineSpan.EndLinePosition.Line + 1, components.ToArray());
    }
    
    /// <summary>
    /// Returns TypeData if it makes sense to generate it
    /// </summary>
    /// <param name="type"></param>
    /// <returns></returns>
    private TypeData? GetTypeData(ITypeSymbol? type, bool ignoreBuiltinCheck) {
        if (type == null) {
            return null;
        }
        var typeName = type.ToString();

        if (ignoreBuiltinCheck == false && _builtInTypes.Contains(typeName)) {
            return null;
        }

        TypeData? innerType = null;

        TypeDataKind kind = TypeDataKind.Parsable;
        if (type is IArrayTypeSymbol array) {
            kind = TypeDataKind.Array;

            innerType = GetTypeData(array.ElementType, true);
        } else if (type.OriginalDefinition?.ToString() == "System.Collections.Generic.List<T>") {
            if (type is not INamedTypeSymbol namedType) {
                return null;
            }
            kind = TypeDataKind.List;
            innerType = GetTypeData(namedType.TypeArguments[0], true);
        } else {
            var spanParsableString = $"System.ISpanParsable<{typeName}>";
            bool isSpanParsable = type.AllInterfaces.Any(i => i.ToString() == spanParsableString);

            if (isSpanParsable == false) { // span parsable has priority, so only check for normal parsable as fallback
                var parsableString = $"System.IParsable<{typeName}>";
                if (type.AllInterfaces.Any(i => i.ToString() == parsableString) == false) {
                    // If neither type of parsable, skip the type
                    return null;
                }
            }
            kind = isSpanParsable ? TypeDataKind.SpanParsable : TypeDataKind.Parsable;
        }

        return new(typeName, kind, innerType);
    }

    private void EmitParseCallsCode(SourceProductionContext context, ImmutableArray<ParserCall> parserCalls) {
        var code = new CodeBuilder();
        code.StartBlock("namespace InterpolatedParsing");
        code.StartBlock("partial class InterpolatedParser");
        code.StartBlock("partial struct InterpolatedParseStringHandler");
        code.StartBlock("private static System.Collections.Generic.Dictionary<(string File, System.Range Line), InterpolatedStringSource> GetInterpolatedStringSources()");

        code.AddLine("return new() {");
        code.Indent();

        foreach (var call in parserCalls) {
            code.AddLine($"{{({EscapeString(call.FileLocation)}, {call.LineStart} .. {call.LineEnd}), new(new string[] {{ {string.Join(", ", call.Components.Select(EscapeString))} }} )}},");
        }

        code.Unindent();
        code.AddLine("};");

        code.EndBlock();
        code.EndBlock();
        code.EndBlock();
        code.EndBlock();

        context.AddSource("InterpolatedParser.Sources.g.cs", code.ToString());
    }


    private ImmutableArray<TypeData> FindTypes(ImmutableArray<ParserCall> calls, CancellationToken token) {
        var dict = new Dictionary<string, TypeData>();
        

        foreach (var type in calls.SelectMany(x => x.Types)) {
            if (dict.ContainsKey(type.FullName)) {
                continue;
            }
            dict.Add(type.FullName, type);
        }

        return dict.Values.ToImmutableArray();
    }

    private void EmitTypesCode(SourceProductionContext context, ImmutableArray<TypeData> types) {
        var code = new CodeBuilder();
        code.AddLine("#nullable enable");
        code.StartBlock("namespace InterpolatedParsing");
        code.StartBlock("partial class InterpolatedParser");
        code.StartBlock("partial struct InterpolatedParseStringHandler");

        foreach (var type in types) {
            if (type.Kind is TypeDataKind.Parsable or TypeDataKind.SpanParsable) {
                code.StartBlock($"public void AppendFormatted(in {type.FullName} value)");
                code.AddLine("var nextPart = GetNextPart();");
                code.AddLine($"System.Runtime.CompilerServices.Unsafe.AsRef(in value) = ");
                var variableCall = type.Kind == TypeDataKind.SpanParsable ? "nextPart" : "new string(nextPart)";
                code.AddLine($"\t{type.FullName}.Parse({variableCall}, System.Globalization.CultureInfo.InvariantCulture);");
                code.EndBlock();
            } else if (type.Kind is TypeDataKind.Array or TypeDataKind.List) {
                code.StartBlock($"public void AppendFormatted(in {type.FullName} value, int alignment = 0, string? format = null)");
                
                var returnedValue = type.Kind == TypeDataKind.List ? "list" : "list.ToArray()";
                var function = type.InnerType!.Kind == TypeDataKind.Parsable ? "GetListFromCharsParsable" : "GetListFromCharsSpanParsable";

                code.AddLine($$"""
                    if (format == null || format.Length == 0) {
                    {{'\t'}}throw new System.ArgumentException("An array needs a separator provided in the format string provided.", nameof(format));
                    }

                    var nextPart = GetNextPart();

                    var list = {{function}}<{{type.InnerType.FullName}}>(nextPart, format);
                    System.Runtime.CompilerServices.Unsafe.AsRef(in value) = {{returnedValue}};
                    """);

                code.EndBlock();
            }
        }

        code.EndBlock();
        code.EndBlock();
        code.EndBlock();

        context.AddSource("InterpolatedParser.Types.g.cs", code.ToString());
    }

    private string EscapeString(string str) {
        return $"\"{str.Replace("\\", "\\\\")}\"";
    }

}
