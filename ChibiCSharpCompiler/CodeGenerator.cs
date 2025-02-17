using System.Text;

namespace ChibiCSharpCompiler;

internal static class CodeGenerator
{
    internal static string Generate(this Parse.Program program)
    {
        var sb = new StringBuilder();
        sb.AppendLine(".assembly AddExample { }");
        sb.AppendLine(".method static void Main() cil managed {");
        sb.AppendLine("    .entrypoint");
        try
        {
            for(var node = program.Node; node != null; node = node.Next)
            {
                if (node.Generator(sb))
                {
                    break;
                }
            }
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
        sb.AppendLine("}");
        return sb.ToString();

    }
    internal static bool Generator(this Parse.Node node, StringBuilder stringBuilder)
    {
        if (node == null) return false;

        switch (node.Kind)
        {
            case Parse.NodeKind.Num:
                stringBuilder.AppendLine($"    ldc.i4 {node.Value}");
                break;
            case Parse.NodeKind.Assign:
                if (node.Left.Kind == Parse.NodeKind.Variable)
                {
                    stringBuilder.AppendLine($"    .locals init (int32 {node.Left.Variable.Name})");
                    Generator(node.Right!, stringBuilder);
                    stringBuilder.AppendLine($"    stloc {node.Left.Variable.Offset}");
                }
                else
                {
                    Generator(node.Right!, stringBuilder);
                    stringBuilder.AppendLine($"    stloc {node.Left.Variable.Offset}");
                }
                return false;
            case Parse.NodeKind.Variable:
                stringBuilder.AppendLine($"    ldloc {node.Variable.Offset}");
                return false;
            case Parse.NodeKind.Return:
                Generator(node.Left!, stringBuilder);
                stringBuilder.AppendLine("    call void [mscorlib]System.Console::WriteLine(int32)");
                stringBuilder.AppendLine("    ret");
                return true;
            case Parse.NodeKind.ExpressionStatement:
                Generator(node.Left!, stringBuilder);
                return false;
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
        return false;
    }

}
