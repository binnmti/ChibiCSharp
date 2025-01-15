namespace ChibiCsharp;

internal class Program
{
    static void Main(string[] args)
    {
        if(args.Length != 1)
        {
            Console.WriteLine("Hello, World!");
            return;
        }
        Console.WriteLine(".assembly AddExample { }");
        Console.WriteLine(".method static void Main() cil managed {");
        Console.WriteLine("\t.entrypoint");
        Console.WriteLine($"\tldc.i4 {args[0]}");
        Console.WriteLine("\tcall void [mscorlib]System.Console::WriteLine(int32)");
        Console.WriteLine("\tret");
        Console.WriteLine("}");
    }
}
