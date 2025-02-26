using System.Diagnostics;
using System.Text;

namespace ChibiCSharp;

internal class CodeGenerator
{
    private int LabelCount { get; set; }

    internal string Generate(ChibiCSharp.Function function)
    {
        var sb = new StringBuilder();
        sb.AppendLine(".assembly AddExample { }");
        sb.AppendLine(".method static int32 Main() cil managed {");
        sb.AppendLine("    .entrypoint");
        try
        {
            for (var node = function.Node; node != null; node = node.Next)
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

    private bool Generator(ChibiCSharp.Node node, StringBuilder stringBuilder)
    {
        if (node == null) return false;

        switch (node.Kind)
        {
            case ChibiCSharp.NodeKind.Num:
                stringBuilder.AppendLine($"    ldc.i4 {node.Value}");
                break;
            case ChibiCSharp.NodeKind.Assign:
                Debug.Assert(node.Left != null);

                if (node.Left.Kind == ChibiCSharp.NodeKind.Variable)
                {
                    Debug.Assert(node.Left.Variable != null);
                    Debug.Assert(node.Right != null);

                    stringBuilder.AppendLine($"    .locals init (int32 {node.Left.Variable.Name})");
                    Generator(node.Right, stringBuilder);
                    stringBuilder.AppendLine($"    stloc {node.Left.Variable.Offset}");
                }
                return false;

            case ChibiCSharp.NodeKind.Variable:
                Debug.Assert(node.Variable != null);

                stringBuilder.AppendLine($"    ldloc {node.Variable.Offset}");
                return false;

            case ChibiCSharp.NodeKind.Return:
                Debug.Assert(node.Left != null);

                Generator(node.Left, stringBuilder);
                stringBuilder.AppendLine("    ret");
                return true;

            case ChibiCSharp.NodeKind.If:
                GeneratorIf(node, stringBuilder);
                return false;

            case ChibiCSharp.NodeKind.While:
                GeneratorWhile(node, stringBuilder);
                return false;

            case ChibiCSharp.NodeKind.For:
                if (node.Init != null)
                {
                    Generator(node.Init, stringBuilder);
                }
                var labelBegin = LabelCount++;
                var labelEnd = LabelCount++;
                stringBuilder.AppendLine($"IL_{labelBegin:X4}:");
                if (node.Condition != null)
                {
                    Generator(node.Condition, stringBuilder);
                    stringBuilder.AppendLine($"    brfalse.s IL_{labelEnd:X4}");
                }
                Generator(node.Then!, stringBuilder);
                if (node.Inc != null)
                {
                    Generator(node.Inc, stringBuilder);
                }
                stringBuilder.AppendLine($"    br.s IL_{labelBegin:X4}");
                if (node.Condition != null)
                {
                    stringBuilder.AppendLine($"IL_{labelEnd:X4}:");
                }
                return false;

            case ChibiCSharp.NodeKind.ExpressionStatement:
                Debug.Assert(node.Left != null);

                Generator(node.Left, stringBuilder);
                // 代入を伴わない場合はpopで破棄
                if (node.Left.Kind != ChibiCSharp.NodeKind.Assign)
                {
                    stringBuilder.AppendLine("    pop");
                }
                return false;

            case ChibiCSharp.NodeKind.Block:
                for (var n = node.Body; n != null; n = n.Next)
                {
                    Generator(n, stringBuilder);
                }
                return false;

            case ChibiCSharp.NodeKind.FunctionCall:
                stringBuilder.AppendLine($"    call void {node.FunctionName}()");
                return false;
        }

        Generator(node.Left!, stringBuilder);
        Generator(node.Right!, stringBuilder);
        switch (node.Kind)
        {
            case ChibiCSharp.NodeKind.Add:
                stringBuilder.AppendLine($"    add");
                break;
            case ChibiCSharp.NodeKind.Sub:
                stringBuilder.AppendLine($"    sub");
                break;
            case ChibiCSharp.NodeKind.Mul:
                stringBuilder.AppendLine($"    mul");
                break;
            case ChibiCSharp.NodeKind.Div:
                stringBuilder.AppendLine($"    div");
                break;
            case ChibiCSharp.NodeKind.Eq:
                stringBuilder.AppendLine($"    ceq");
                break;
            case ChibiCSharp.NodeKind.Ne:
                stringBuilder.AppendLine($"    ceq");
                stringBuilder.AppendLine($"    ldc.i4 0");
                stringBuilder.AppendLine($"    ceq");
                break;
            case ChibiCSharp.NodeKind.Lt:
                stringBuilder.AppendLine($"    clt");
                break;
            case ChibiCSharp.NodeKind.Le:
                stringBuilder.AppendLine($"    cgt");
                stringBuilder.AppendLine($"    ldc.i4 0");
                stringBuilder.AppendLine($"    ceq");
                break;
        }
        return false;
    }

    private void GeneratorIf(ChibiCSharp.Node node, StringBuilder stringBuilder)
    {
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
    }

    private void GeneratorWhile(ChibiCSharp.Node node, StringBuilder stringBuilder)
    {
        Debug.Assert(node.Condition != null);
        Debug.Assert(node.Then != null);

        var labelBegin = LabelCount++;
        var labelWhite = LabelCount++;
        var labelWhileEnd = LabelCount++;
        stringBuilder.AppendLine($"IL_{labelBegin:X4}:");
        Generator(node.Condition, stringBuilder);
        stringBuilder.AppendLine($"    brfalse.s IL_{labelWhileEnd:X4}");
        stringBuilder.AppendLine($"IL_{labelWhite:X4}:");
        Generator(node.Then, stringBuilder);
        stringBuilder.AppendLine($"    br.s IL_{labelBegin:X4}");
        stringBuilder.AppendLine($"IL_{labelWhileEnd:X4}:");
    }
}
