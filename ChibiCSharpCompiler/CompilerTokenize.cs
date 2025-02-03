﻿namespace ChibiCSharpCompiler;
internal static class CompilerTokenize
{
    internal enum TokenKind
    {
        Reserved,   // 記号
        Number,     // 整数
        Error,      // エラー
        Eof,        // ファイル終端
    }

    internal class Token(TokenKind Kind, string Str, int Value)
    {
        public TokenKind Kind { get; } = Kind;
        public Token Next { get; set; } = null!;
        public string Str { get; } = Str;
        public int Value { get; } = Value;
    }

    internal static Token ToToken(string p)
    {
        Token head = new(TokenKind.Eof, "", 0);
        Token current = head;
        for (int i = 0; i < p.Length; i++)
        {
            var c = p[i];
            if (char.IsWhiteSpace(c)) continue;
            else if (IsEqualSign(i, p))
            {
                var str = string.Concat(p[i].ToString(), p[i + 1].ToString());
                current = current.AddToken(TokenKind.Reserved, str, 0);
                i++;
            }
            else if ("+-*/()<>".Contains(c))
            {
                current = current.AddToken(TokenKind.Reserved, c.ToString(), 0);
            }
            else if (char.IsDigit(c))
            {
                string num = c.ToString();
                while (i + 1 < p.Length)
                {
                    var next = p[i + 1];
                    if (!char.IsDigit(next)) break;

                    num = num + next.ToString();
                    i++;
                }

                current = current.AddToken(TokenKind.Number, num, int.Parse(num));
            }
            else
            {
                current = current.AddToken(TokenKind.Error, c.ToString(), 0);
            }
        }
        current.Next = new Token(TokenKind.Eof, "", 0);
        return head.Next;
    }

    private static Token AddToken(this Token Current, TokenKind Kind, string Str, int Value)
    {
        Current.Next = new Token(Kind, Str, Value);
        return Current.Next;
    }

    private static bool IsEqualSign(int i, string p)
        => i != p.Length - 1 && (
                 (p[i] == '=' && p[i + 1] == '=') ||
                 (p[i] == '!' && p[i + 1] == '=') ||
                 (p[i] == '<' && p[i + 1] == '=') ||
                 (p[i] == '>' && p[i + 1] == '='));
}
