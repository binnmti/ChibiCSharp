using System.Diagnostics;
using System.Text;

namespace ChibiCSharp;

internal class CodeGenerator
{
    private int LabelCount { get; set; }

    internal string Generate(Parse.Program program)
    {
        var sb = new StringBuilder();
        sb.AppendLine(".assembly AddExample { }");
        sb.AppendLine(".method static int32 Main() cil managed {");
        sb.AppendLine("    .entrypoint");
        try
        {
            for (var node = program.Node; node != null; node = node.Next)
            {
                if (Generator(node, sb))
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

    private bool Generator(Parse.Node node, StringBuilder stringBuilder)
    {
        if (node == null) return false;

        switch (node.Kind)
        {
            case Parse.NodeKind.Num:
                stringBuilder.AppendLine($"    ldc.i4 {node.Value}");
                break;
            case Parse.NodeKind.Assign:
                Debug.Assert(node.Left != null);

                if (node.Left.Kind == Parse.NodeKind.Variable)
                {
                    Debug.Assert(node.Left.Variable != null);
                    Debug.Assert(node.Right != null);

                    stringBuilder.AppendLine($"    .locals init (int32 {node.Left.Variable.Name})");
                    Generator(node.Right, stringBuilder);
                    stringBuilder.AppendLine($"    stloc {node.Left.Variable.Offset}");
                }
                return false;

            case Parse.NodeKind.Variable:
                Debug.Assert(node.Variable != null);

                stringBuilder.AppendLine($"    ldloc {node.Variable.Offset}");
                return false;

            case Parse.NodeKind.Return:
                Debug.Assert(node.Left != null);

                Generator(node.Left, stringBuilder);
                stringBuilder.AppendLine("    ret");
                return true;

            case Parse.NodeKind.If:
                Debug.Assert(node.Condition != null);
                Debug.Assert(node.Then != null);

                int labelElse = LabelCount++;
                int labelEnd = LabelCount++;
                Generator(node.Condition, stringBuilder);
                stringBuilder.AppendLine($"    brfalse.s IL_{labelElse:X4}");
                Generator(node.Then, stringBuilder);
                stringBuilder.AppendLine($"    br.s IL_{labelEnd:X4}");
                stringBuilder.AppendLine($"IL_{labelElse:X4}:");
                if (node.Else != null)
                {
                    Generator(node.Else, stringBuilder);
                }
                stringBuilder.AppendLine($"IL_{labelEnd:X4}:");
                return false;

            case Parse.NodeKind.ExpressionStatement:
                Debug.Assert(node.Left != null);

                Generator(node.Left, stringBuilder);
                // 代入を伴わない場合はpopで破棄
                if (node.Left.Kind != Parse.NodeKind.Assign)
                {
                    stringBuilder.AppendLine("    pop");
                }
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
