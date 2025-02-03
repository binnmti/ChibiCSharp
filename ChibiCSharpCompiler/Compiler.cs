using System.Text;

namespace ChibiCSharpCompiler;

public static class Compiler
{
    public static string Compile(string code)
    {
        var sb = new StringBuilder();
        sb.AppendLine(".assembly AddExample { }");
        sb.AppendLine(".method static void Main() cil managed {");
        sb.AppendLine("    .entrypoint");

        try
        {
            var token = CompilerTokenize.ToToken(code);
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
            case CompilerToNode.NodeKind.Eq:
                stringBuilder.AppendLine($"    ceq");
                break;
            case CompilerToNode.NodeKind.Ne:
                stringBuilder.AppendLine($"    ceq");
                stringBuilder.AppendLine($"    ldc.i4 0");
                stringBuilder.AppendLine($"    ceq");
                break;
            case CompilerToNode.NodeKind.Lt:
                stringBuilder.AppendLine($"    clt");
                break;
            case CompilerToNode.NodeKind.Le:
                stringBuilder.AppendLine($"    cgt");
                stringBuilder.AppendLine($"    ldc.i4 0");
                stringBuilder.AppendLine($"    ceq");
                break;
        }
    }
}
