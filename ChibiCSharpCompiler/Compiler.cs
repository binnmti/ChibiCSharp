namespace ChibiCSharpCompiler;

public static class Compiler
{
    public static string Compile(string code)
    {
        try
        {
            var token = Tokenize.ToToken(code);
            var program = token.ToProgram();
            int offset = 0;
            for (Parse.Variable var = program.Variable; var != null; var = var.Next)
            {
                offset += 8;
                var.Offset = offset / 8 - 1;
            }
            program = program with { StackSize = offset };
            return program.Generate();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

}
