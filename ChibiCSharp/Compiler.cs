namespace ChibiCSharp;

public static class Compiler
{
    public static string Compile(string code)
    {
        try
        {
            var token = Tokenize.ToToken(code);
            var function = new Parse(token).ToProgram();
            int offset = 0;
            for (ChibiCSharp.Variable var = function.Variable; var != null; var = var.Next!)
            {
                offset += 8;
                var.Offset = offset / 8 - 1;
            }
            function.StackSize = offset;
            return new CodeGenerator().Generate(function);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

}
