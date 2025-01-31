namespace ChibiCSharpCompiler;
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
            else if ("+-*/()".Contains(c))
            {
                current.Next = new Token(TokenKind.Reserved, c.ToString(), 0);
                current = current.Next;
                continue;
            }
            else if (char.IsDigit(c))
            {
                string num = c.ToString();
                while (true)
                {
                    if (i + 1 == p.Length) break;

                    var next = p[i + 1];
                    if (!char.IsDigit(next)) break;
                    num = num + next.ToString();
                    i++;
                }
                current.Next = new Token(TokenKind.Number, num.ToString(), int.Parse(num));
                current = current.Next;
            }
            else
            {
                current.Next = new Token(TokenKind.Error, c.ToString(), 0);
                current = current.Next;
            }
        }
        current.Next = new Token(TokenKind.Eof, "", 0);
        // current.Nextを入れているのでNextがnullになることはない
        return head.Next!;
    }
}
