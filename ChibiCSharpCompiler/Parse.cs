namespace ChibiCSharpCompiler;

// 生成規則 Generation rules ← EBNF
// expr (Expression) 式
// mul (Multiplication) 乗算
// primary (Primary Expression) 基本要素

// program    = stmt*
// stmt = expr ";"
// expr       = assign
// assign = equality("=" assign) ?
// equality   = relational("==" relational | "!=" relational)*
// relational = add("<" add | "<=" add | ">" add | ">=" add) *
// add = mul("+" mul | "-" mul) *
// mul = unary("*" unary | "/" unary) *
// unary = ("+" | "-") ? primary
// primary = num | ident | "(" expr ")"

// 再帰下降構文解析

internal static class Parse
{
    internal enum NodeKind
    {
        Add,   // +
        Sub,   // -
        Mul,   // *
        Div,   // /
        Eq,    // ==
        Assign,// =
        Ne,    // !=
        Lt,    // <
        Le,    // <=
        Num,   // 整数
        LocalVariable,   //ローカル変数
    }

    // BinaryTree
    internal record Node(NodeKind Kind, Node Left, Node Right, int Value, int Offset, string Name);
    private static Node NewNode(NodeKind kind, Node left, Node right) => new(kind, left, right, 0, 0, "");
    // 数字の場合は最後尾なので便宜上null!を使う。それ以外ではnull使わない。
    private static Node NewNodeNum(int val) => new(NodeKind.Num, null!, null!, val, 0, "");
    private static Node NewLocalVariable(int offset, string name) => new(NodeKind.Num, null!, null!, 0, offset, name);

    internal static Node ToNode(this Tokenize.Token token)
    {
        Token = token;
        // TODO:program nothitng
        return Stmt();
    }

    private static Tokenize.Token Token { get; set; } = null!;

    private static Node Stmt()
    {
        var node = Expr();
        Expect(";");
        return node;
    }

    private static Node Expr()
    {
        return Assign();
    }

    private static Node Assign()
    {
        var node = Equality();
        if (Consume("="))
        {
            node = NewNode(NodeKind.Assign, node, Assign());
        }
        return node;
    }

    private static Node Equality()
    {
        var node = Relational();
        while (true)
        {
            if (Consume("=="))
            {
                node = NewNode(NodeKind.Eq, node, Relational());
            }
            else if (Consume("!="))
            {
                node = NewNode(NodeKind.Ne, node, Relational());
            }
            else
            {
                return node;
            }
        }
    }

    private static Node Relational()
    {
        var node = Add();
        while (true)
        {
            if (Consume("<"))
            {
                node = NewNode(NodeKind.Lt, node, Add());
            }
            else if (Consume("<="))
            {
                node = NewNode(NodeKind.Le, node, Add());
            }
            else if (Consume(">"))
            {
                node = NewNode(NodeKind.Lt, Add(), node);
            }
            else if (Consume(">="))
            {
                node = NewNode(NodeKind.Le, Add(), node);
            }
            else
            {
                return node;
            }
        }
    }

    private static Node Add()
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
        var token = ConsumeIdnet();
        if (token != null)
        {
            return NewLocalVariable((Token.Str[0] - 'a' + 1) * 8, token.Str);
        }
        return NewNodeNum(ExpectNumber());
    }


    private static bool Consume(string op)
    {
        if (Token.Kind != Tokenize.TokenKind.Reserved || Token.Str != op)
        {
            return false;
        }
        Token = Token.Next;
        return true;
    }

    private static Tokenize.Token? ConsumeIdnet()
    {
        if (Token.Kind != Tokenize.TokenKind.Identifier)
        {
            return null;
        }
        Token = Token.Next;
        return Token;
    }

    private static void Expect(string op)
    {
        if (Token.Kind != Tokenize.TokenKind.Reserved || Token.Str != op)
        {
            throw new Exception($"'{op}'ではありません");
        }
        Token = Token.Next;
    }

    private static int ExpectNumber()
    {
        if (Token.Kind != Tokenize.TokenKind.Number)
        {
            throw new Exception($"整数ではありません");
        }
        var val = Token.Value;
        Token = Token.Next;
        return val;
    }
}
