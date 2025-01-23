using System.Text;

namespace ChibiCSharpCompiler;

public class Compiler
{
    public static string Compile(string arg)
    {
        var result = new StringBuilder();
        result.AppendLine(".assembly AddExample { }");
        result.AppendLine(".method static void Main() cil managed {");
        result.AppendLine("    .entrypoint");
        for (int i = 0; i < arg.Length; i++)
        {
            char c = arg[i];
            if (c == '+')
            {
                var (str, idx) = GetNumber(arg, i + 1);
                if (str == "") return "+の後に数字がない";
                i = idx;
                result.AppendLine($"    ldc.i4 {str}");
                result.AppendLine($"    add");
            }
            else if (c == '-')
            {
                var (str, idx) = GetNumber(arg, i + 1);
                if (str == "") return "-の後に数字がない";
                i = idx;
                result.AppendLine($"    ldc.i4 {str}");
                result.AppendLine($"    sub");
            }
            else
            {
                var (str, idx) = GetNumber(arg, i);
                i = idx;
                result.AppendLine($"    ldc.i4 {str}");
            }
        }
        result.AppendLine("    call void [mscorlib]System.Console::WriteLine(int32)");
        result.AppendLine("    ret");
        result.AppendLine("}");
        return result.ToString();
    }

    private static (string, int) GetNumber(string arg, int idx)
    {
        if (idx >= arg.Length) return ("", idx);

        var s = arg[idx].ToString();
        while (true)
        {
            if (idx + 1 == arg.Length) break;
            var c = arg[idx + 1];
            if (!char.IsNumber(c)) break;
            s = s + c;
            idx++;
        }
        return (s, idx);
    }
}
