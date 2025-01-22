using ChibiCSharpCompiler;

namespace ChibiCSharp;

internal class Program
{
    static void Main(string[] args)
    {
        if (args.Length != 1)
        {
            Console.WriteLine("Hello, World!");
            return;
        }
        var result = Compiler.Compile(args[0]);
        Console.WriteLine(result);
    }
}
