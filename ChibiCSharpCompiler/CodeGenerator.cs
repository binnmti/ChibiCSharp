using System.Text;

namespace ChibiCSharpCompiler;

internal static class CodeGenerator
{
    internal static string Generate(this Parse.Node node)
    {
        var sb = new StringBuilder();
        sb.AppendLine(".assembly AddExample { }");
        sb.AppendLine(".method static void Main() cil managed {");
        sb.AppendLine("    .entrypoint");
        try
        {
            node.Generator(sb);
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
    internal static void Generator(this Parse.Node node, StringBuilder stringBuilder)
    {
        if (node.Kind == Parse.NodeKind.Num)
        {
            stringBuilder.AppendLine($"    ldc.i4 {node.Value}");
            return;
        }
        Generator(node.Left!, stringBuilder);
        Generator(node.Right!, stringBuilder);
        switch (node.Kind)
        {
            case Parse.NodeKind.Add:
                stringBuilder.AppendLine($"    add");
                break;
            case Parse.NodeKind.Sub:
                stringBuilder.AppendLine($"    sub");
                break;
            case Parse.NodeKind.Mul:
                stringBuilder.AppendLine($"    mul");
                break;
            case Parse.NodeKind.Div:
                stringBuilder.AppendLine($"    div");
                break;
            case Parse.NodeKind.Eq:
                stringBuilder.AppendLine($"    ceq");
                break;
            case Parse.NodeKind.Ne:
                stringBuilder.AppendLine($"    ceq");
                stringBuilder.AppendLine($"    ldc.i4 0");
                stringBuilder.AppendLine($"    ceq");
                break;
            case Parse.NodeKind.Lt:
                stringBuilder.AppendLine($"    clt");
                break;
            case Parse.NodeKind.Le:
                stringBuilder.AppendLine($"    cgt");
                stringBuilder.AppendLine($"    ldc.i4 0");
                stringBuilder.AppendLine($"    ceq");
                break;
        }
    }

}
