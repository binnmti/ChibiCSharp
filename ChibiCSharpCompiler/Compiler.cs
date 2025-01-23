﻿using System.Text;

namespace ChibiCSharpCompiler;

public class Compiler
{
    private enum TokenKind
    {
        Reserved,   // 記号
        Number,     // 整数
        Error,      // エラー
        Eof,        // ファイル終端
    }

    private record Token(TokenKind Kind, string Str, int Value);
    private static List<Token> TokenList { get; set; } = [];

    private static void Tokenize(string p)
    {
        TokenList.Clear();
        for (int i = 0; i < p.Length; i++)
        {
            var c = p[i];
            if (char.IsWhiteSpace(c)) continue;
            else if (c == '+' || c == '-')
            {
                TokenList.Add(new Token(TokenKind.Reserved, c.ToString(), 0));
                continue;
            }
            else if(char.IsDigit(c))
            {
                string num = c.ToString();
                while(true)
                {
                    if (i + 1 == p.Length) break;

                    var next = p[i + 1];
                    if (!char.IsDigit(next)) break;
                    num = num + next.ToString();
                    i++;
                }
                TokenList.Add(new Token(TokenKind.Number, c.ToString(), int.Parse(num)));
            }
            else
            {
                TokenList.Add(new Token(TokenKind.Error, c.ToString(), 0));
            }
        }
    }

    public static string Compile(string arg)
    {
        Tokenize(arg);

        var result = new StringBuilder();
        result.AppendLine(".assembly AddExample { }");
        result.AppendLine(".method static void Main() cil managed {");
        result.AppendLine("    .entrypoint");
        for (int i = 0; i < TokenList.Count; i++)
        {
            var token = TokenList[i];
            if (token.Kind.Equals(TokenKind.Number))
            {
                result.AppendLine($"    ldc.i4 {token.Value}");
            }
            else if (token.Kind.Equals(TokenKind.Reserved))
            {
                if (token.Str == "+")
                {
                    if(i + 1 >= TokenList.Count) return "+の後に数字がない";
                    i++;
                    if (TokenList[i].Kind.Equals(TokenKind.Number))
                    {
                        result.AppendLine($"    ldc.i4 {TokenList[i].Value}");
                    }
                    else
                    {
                        return "+の後に数字がない";
                    }
                    result.AppendLine("    add");
                }
                else if (token.Str == "-")
                {
                    if (i + 1 >= TokenList.Count) return "-の後に数字がない";
                    i++;
                    if (TokenList[i].Kind.Equals(TokenKind.Number))
                    {
                        result.AppendLine($"    ldc.i4 {TokenList[i].Value}");
                    }
                    else
                    {
                        return "-の後に数字がない";
                    }
                    result.AppendLine("    sub");
                }
            }
        }
        result.AppendLine("    call void [mscorlib]System.Console::WriteLine(int32)");
        result.AppendLine("    ret");
        result.AppendLine("}");
        return result.ToString();
    }
}
