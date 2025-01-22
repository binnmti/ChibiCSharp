namespace ChibiCsharp;

internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Hello, World!");
            return;
        }
        Console.WriteLine(".assembly AddExample { }");
        Console.WriteLine(".method static void Main() cil managed {");
        Console.WriteLine(" .entrypoint");
        for (int i = 0; i < args[0].Length; i++)
        {
            char c = args[0][i];
            if (c == '+')
            {
                var (str, idx) = GetNumber(args[0], i + 1);
                i = idx;
                Console.WriteLine($"    ldc.i4 {str}");
                Console.WriteLine($"    add");
            }
            else if (c == '-')
            {
                var (str, idx) = GetNumber(args[0], i + 1);
                i = idx;
                Console.WriteLine($"    ldc.i4 {str}");
                Console.WriteLine($"    sub");
            }
            else
            {
                var (str, idx) = GetNumber(args[0], i);
                i = idx;
                Console.WriteLine($"    ldc.i4 {str}");
            }
        }
        Console.WriteLine("    call void [mscorlib]System.Console::WriteLine(int32)");
        Console.WriteLine("    ret");
        Console.WriteLine("}");
    }

    private static (string, int) GetNumber(string arg, int idx)
    {
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
