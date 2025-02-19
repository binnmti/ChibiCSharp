namespace ChibiCSharp;

public static class Compiler
{
    public static string Compile(string code)
    {
        try
        {
            var token = Tokenize.ToToken(code);
            var program = new Parse(token).ToProgram();
            int offset = 0;
            for (Parse.Variable var = program.Variable; var != null; var = var.Next!)
            {
                offset += 8;
                var.Offset = offset / 8 - 1;
            }
            program = program with { StackSize = offset };
            return new CodeGenerator().Generate(program);
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

}
