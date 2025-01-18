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
        Console.WriteLine(" .entrypoint");

        foreach(var s in args[0])
        {
            if(s == '+')
            {
                Console.WriteLine($"    add");
            }
            else if(s == '-')
            {
                Console.WriteLine($"    sub");
            }

        }


        /*
        ldc.i4 5
        ldc.i4 20
        add
        ldc.i4 4
        sub         * 
         */


        Console.WriteLine($"\tldc.i4 {args[0]}");
        Console.WriteLine("\tcall void [mscorlib]System.Console::WriteLine(int32)");
        Console.WriteLine("\tret");
        Console.WriteLine("}");
    }
}
