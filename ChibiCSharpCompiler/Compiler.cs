using System.Text;

namespace ChibiCSharpCompiler;

public static class Compiler
{
    public static string Compile(string code)
    {
        try
        {
            var token = Tokenize.ToToken(code);
            var node = token.ToNode();
            return node.Generate();
        }
        catch (Exception ex)
        {
            return ex.Message;
        }
    }

}
