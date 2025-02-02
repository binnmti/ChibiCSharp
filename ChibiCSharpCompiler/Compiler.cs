using System.Text;

namespace ChibiCSharpCompiler;

public static class Compiler
{
    public static string Compile(string arg)
    {
        var sb = new StringBuilder();
        sb.AppendLine(".assembly AddExample { }");
        sb.AppendLine(".method static void Main() cil managed {");
        sb.AppendLine("    .entrypoint");

        try
        {
            var token = CompilerTokenize.ToToken(arg);
            var node = token.ToNode();
            node.Gen(sb);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }

        sb.AppendLine("    call void [mscorlib]System.Console::WriteLine(int32)");
        sb.AppendLine("    ret");
        sb.AppendLine("}");
        return sb.ToString();
    }

    // 再帰下降構文解析
    private static void Gen(this CompilerToNode.Node node, StringBuilder stringBuilder)
    {
        if (node.Kind == CompilerToNode.NodeKind.Num)
        {
            stringBuilder.AppendLine($"    ldc.i4 {node.Value}");
            return;
        }
        Gen(node.Left!, stringBuilder);
        Gen(node.Right!, stringBuilder);
        switch (node.Kind)
        {
            case CompilerToNode.NodeKind.Add:
                stringBuilder.AppendLine($"    add");
                break;
            case CompilerToNode.NodeKind.Sub:
                stringBuilder.AppendLine($"    sub");
                break;
            case CompilerToNode.NodeKind.Mul:
                stringBuilder.AppendLine($"    mul");
                break;
            case CompilerToNode.NodeKind.Div:
                stringBuilder.AppendLine($"    div");
                break;
        }
    }
}
