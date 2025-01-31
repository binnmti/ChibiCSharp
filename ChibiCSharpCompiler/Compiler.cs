using System.Text;

namespace ChibiCSharpCompiler;

public class Compiler
{
    static void Gen(CompilerToNode.Node node, StringBuilder stringBuilder)
    {
        if(node.Kind == CompilerToNode.NodeKind.Num)
        {
            stringBuilder.AppendLine($"    ldc.i4 {node.Value}");
            return;
        }
        Gen(node.Left!, stringBuilder);
        Gen(node.Right!, stringBuilder);
        switch(node.Kind)
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

    public static string Compile(string arg)
    {
        CompilerToNode.Node node;
        try
        {
            node = CompilerTokenize.ToToken(arg).ToNode();
        }
        catch(Exception ex)
        {
            return ex.Message;
        }

        var result = new StringBuilder();
        result.AppendLine(".assembly AddExample { }");
        result.AppendLine(".method static void Main() cil managed {");
        result.AppendLine("    .entrypoint");
        Gen(node, result);
        result.AppendLine("    call void [mscorlib]System.Console::WriteLine(int32)");
        result.AppendLine("    ret");
        result.AppendLine("}");
        return result.ToString();
    }
}
