using System.Diagnostics;

namespace ChibiCSharpCompiler;
internal static class Tokenize
{
    internal enum TokenKind
    {
        Reserved,   // 記号
        Identifier, // 識別子
        Number,     // 整数
        Error,      // エラー
        Eof,        // ファイル終端
    }

    private static readonly string[] ReservedWords = { "return", "if", "else" };

    internal class Token(TokenKind Kind, string Str, int Value)
    {
        public TokenKind Kind { get; } = Kind;
        public Token? Next { get; set; }
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

            bool isReserved = false;
            foreach (var word in ReservedWords)
            {
                if (p[i..].StartsWith(word))
                {
                    current = current.AddToken(TokenKind.Reserved, word, 0);
                    i += word.Length - 1;
                    isReserved = true;
                    break;
                }
            }
            if (isReserved)
            {
                continue;
            }
            else if (IsEqualSign(i, p))
            {
                var str = string.Concat(p[i].ToString(), p[i + 1].ToString());
                current = current.AddToken(TokenKind.Reserved, str, 0);
                i++;
            }
            else if (char.IsLetter(c) || c == '_')
            {
                string variable = c.ToString();
                while (i + 1 < p.Length)
                {
                    var next = p[i + 1];
                    if (char.IsDigit(next) || char.IsLetter(next) || c == '_')
                    {
                        variable = variable + next.ToString();
                        i++;
                    }
                    else
                    {
                        break;
                    }
                }
                current = current.AddToken(TokenKind.Identifier, variable, 0);
            }
            else if ("+-*/()<>;=".Contains(c))
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
        Debug.Assert(head.Next != null);
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
