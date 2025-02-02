namespace ChibiCSharpCompiler;

// 生成規則 Generation rules ← EBNF
// expr (Expression) 式
// mul (Multiplication) 乗算
// primary (Primary Expression) 基本要素
// expr = mul ("+" mul | "-" mul)*
// mul     = unary  ("*" unary  | "/" unary )*
// unary = ("+" | "-")? primary
// primary = num | "(" expr ")"
internal static class CompilerToNode
{
    internal enum NodeKind
    {
        Add,   // +
        Sub,   // -
        Mul,   // *
        Div,   // /
        Num,   // 整数
    }

    // BinaryTree
    internal record Node(NodeKind Kind, Node Left, Node Right, int Value);
    private static Node NewNode(NodeKind kind, Node left, Node right) => new(kind, left, right, 0);
    // 数字の場合は最後尾なので便宜上null!を使う。それ以外ではnull使わない。
    private static Node NewNodeNum(int val) => new(NodeKind.Num, null!, null!, val);

    internal static Node ToNode(this CompilerTokenize.Token token)
    {
        Token = token;
        return Expr();
    }

    private static CompilerTokenize.Token Token { get; set; } = null!;

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
        var node = Unary();
        while (true)
        {
            if (Consume("*"))
            {
                node = NewNode(NodeKind.Mul, node, Unary());
            }
            else if (Consume("/"))
            {
                node = NewNode(NodeKind.Div, node, Unary());
            }
            else
            {
                return node;
            }
        }
    }
    private static Node Unary()
    {
        if (Consume("+"))
        {
            return Unary();
        }
        else if (Consume("-"))
        {
            return NewNode(NodeKind.Sub, NewNodeNum(0), Unary());
        }
        return Primary();
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


    private static bool Consume(string op)
    {
        if (Token.Kind != CompilerTokenize.TokenKind.Reserved || Token.Str != op)
        {
            return false;
        }
        Token = Token.Next;
        return true;
    }

    private static void Expect(string op)
    {
        if (Token.Kind != CompilerTokenize.TokenKind.Reserved || Token.Str != op)
        {
            throw new Exception($"'{op}'ではありません");
        }
        Token = Token.Next;
    }

    private static int ExpectNumber()
    {
        if (Token.Kind != CompilerTokenize.TokenKind.Number)
        {
            throw new Exception($"整数ではありません");
        }
        var val = Token.Value;
        Token = Token.Next;
        return val;
    }
}
