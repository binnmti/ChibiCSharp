namespace ChibiCSharpCompiler;
internal static class CompilerToNode
{
    private static CompilerTokenize.Token Token { get; set; } = null!;

    internal enum NodeKind
    {
        Add,   // +
        Sub,   // -
        Mul,   // *
        Div,   // /
        Num,   // 整数
    }

    // BinaryTree
    internal record Node(NodeKind Kind, Node? Left, Node? Right, int Value);


    internal static Node ToNode(this CompilerTokenize.Token token)
    {
        Token = token;
        return Expr();
    }

    // EBNF
    // expr = mul ("+" mul | "-" mul)*
    // mul     = primary ("*" primary | "/" primary)*
    // primary = num | "(" expr ")"
    private static Node Expr()
    {
        var node = Mul();
        while (true)
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

    private static Node Mul()
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

    private static Node Primary()
    {
        if (Consume("("))
        {
            var node = Expr();
            Expect(")");
            return node;
        }
        return NewNodeNum(ExpectNumber());
    }

    private static Node NewNode(NodeKind kind, Node left, Node right) => new (kind, left, right, 0);

    private static Node NewNodeNum(int val) => new (NodeKind.Num, null, null, val);

    private static bool Consume(string op)
    {
        if (Token.Kind != CompilerTokenize.TokenKind.Reserved || Token.Str != op)
        {
            return false;
        }
        Token = Token.Next!;
        return true;
    }

    private static void Expect(string op)
    {
        if (Token.Kind != CompilerTokenize.TokenKind.Reserved || Token.Str != op)
        {
            throw new Exception($"'{op}'ではありません");
        }
        Token = Token.Next!;
    }

    private static int ExpectNumber()
    {
        if (Token.Kind != CompilerTokenize.TokenKind.Number)
        {
            throw new Exception($"数ではありません");
        }
        var val = Token.Value;
        Token = Token.Next!;
        return val;
    }
}
