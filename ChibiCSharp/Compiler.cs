namespace ChibiCSharp;

public static class Compiler
{
    public static string Compile(string code)
    {
        try
        {
            var token = Tokenize.ToToken(code);
            var function = new Parse(token).ToProgram();
            TypeResolver.AddType(function);
            int offset = 0;
            for (var fn = function; fn != null; fn = fn.Next)
            {
                for (ChibiCSharp.VariableList var = fn.VariableList; var != null; var = var.Next!)
                {
                    var.Variable.Offset = offset;
                    offset++;
                }
                // TODO:逆にするのにあまり良いやり方ではないし、IsArgumentのフラグも検討した方が良い
                for (ChibiCSharp.VariableList var = fn.VariableList; var != null; var = var.Next!)
                {
                    if (var.Variable.IsArgument)
                    {
                        offset--;
                        var.Variable.Offset = offset;
                    }
                }
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
