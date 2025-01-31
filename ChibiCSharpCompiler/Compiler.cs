using System.Text;

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

    private class Token(TokenKind Kind, string Str, int Value)
    {
        public TokenKind Kind { get; } = Kind;
        public Token? Next { get; set; }
        public string Str { get; } = Str;
        public int Value { get; } = Value;
    }
    private Token CurrentToken { get; set; }


    private enum NodeKind
    {
        Add,   // +
        Sub,   // -
        Mul,   // *
        Div,   // /
        Num,   // 整数
    }
    // TODO:
    // EBNF
    // expr = mul ("+" mul | "-" mul)*
    // mul     = primary ("*" primary | "/" primary)*
    // primary = num | "(" expr ")"

    //BinaryTree
    private record Node(NodeKind Kind, Node? Left, Node? Right, int Value);

    private Node NewNode(NodeKind kind, Node left, Node right)
    {
        return new Node(kind, left, right, 0);
    }

    private Node NewNodeNum(int val)
    {
        return new Node(NodeKind.Num, null, null, val);
    }

    private bool Consume(string op)
    {
        if (CurrentToken.Kind != TokenKind.Reserved || CurrentToken.Str != op)
        {
            return false;
        }
        CurrentToken = CurrentToken.Next;
        return true;
    }

    private void Expect(string op)
    {
        if (CurrentToken.Kind != TokenKind.Reserved || CurrentToken.Str != op)
        {
            throw new Exception($"'{op}'ではありません");
        }
        CurrentToken = CurrentToken.Next;
    }
    private int ExpectNumber()
    {
        if (CurrentToken.Kind != TokenKind.Number)
        {
            throw new Exception($"数ではありません");
        }
        var val = CurrentToken.Value;
        CurrentToken = CurrentToken.Next;
        return val;
    }

    private Node Expr()
    {
        var node = Mul();
        while(true)
        {
            if (Consume("+"))
            {
                node = NewNode(NodeKind.Add, node, Mul());
            }
            else if (Consume("-"))
            {
                node = NewNode(NodeKind.Sub, node, Mul());
            }
            else
            {
                return node;
            }
        }
    }

    private Node Mul()
    {
        var node = Primary();
        while (true)
        {
            if (Consume("*"))
            {
                node = NewNode(NodeKind.Mul, node, Primary());
            }
            else if (Consume("/"))
            {
                node = NewNode(NodeKind.Div, node, Primary());
            }
            else
            {
                return node;
            }
        }
    }

    private Node Primary()
    {
        if (Consume("("))
        {
            var node = Expr();
            Expect(")");
            return node;
        }
        return NewNodeNum(ExpectNumber());
    }

    private static Token Tokenize(string p)
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
        return head.Next;
    }

    void Gen(Node node, StringBuilder stringBuilder)
    {
        if(node.Kind == NodeKind.Num)
        {
            stringBuilder.AppendLine($"    ldc.i4 {node.Value}");
            return;
        }
        Gen(node.Left, stringBuilder);
        Gen(node.Right, stringBuilder);
        switch(node.Kind)
        {
            case NodeKind.Add:
                stringBuilder.AppendLine($"    add");
                break;
            case NodeKind.Sub:
                stringBuilder.AppendLine($"    sub");
                break;
            case NodeKind.Mul:
                stringBuilder.AppendLine($"    mul");
                break;
            case NodeKind.Div:
                stringBuilder.AppendLine($"    div");
                break;
        }
    }

    public string Compile(string arg)
    {
        CurrentToken = Tokenize(arg);
        var node = Expr();

        var result = new StringBuilder();
        result.AppendLine(".assembly AddExample { }");
        result.AppendLine(".method static void Main() cil managed {");
        result.AppendLine("    .entrypoint");
        Gen(node, result);
        result.AppendLine("    call void [mscorlib]System.Console::WriteLine(int32)");
        result.AppendLine("    ret");
        result.AppendLine("}");
        return result.ToString();
    }
}
