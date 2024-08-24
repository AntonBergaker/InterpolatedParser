using System.Text;

namespace InterpolatedParser;

public class CodeBuilder {
    private readonly StringBuilder _builder;
    private int _indent;

    public CodeBuilder() {
        _builder = new StringBuilder();
        _indent = 0;
    }

    public void AddLine(string line) {
        if (line.Contains("\n")) {
            foreach (string str in line.Split('\n')) {
                this.AddLine(str);
            }
            return;
        }
        _builder.AppendLine(new string('\t', _indent) + line);
    }

    public void AddLines(params string[] lines) {
        foreach (string line in lines) {
            _builder.AppendLine(new string('\t', _indent) + line);
        }
    }

    public void StartBlock() {
        AddLine("{");
        Indent();
    }

    public void StartBlock(string blockStart) {
        AddLine(blockStart + " {");
        Indent();
    }

    public void EndBlock() {
        Unindent();
        AddLine("}");
    }

    public void Indent() {
        _indent++;
    }

    public void Unindent() {
        _indent--;
    }

    public override string ToString() {
        return _builder.ToString();
    }
}