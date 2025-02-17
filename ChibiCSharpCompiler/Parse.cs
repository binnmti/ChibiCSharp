namespace ChibiCSharpCompiler;

// 生成規則 Generation rules ← EBNF
// expr (Expression) 式
// mul (Multiplication) 乗算
// primary (Primary Expression) 基本要素

// program    = stmt*
// stmt = expr ";" | "return" expr ";"
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
        Add,    // +
        Sub,    // -
        Mul,    // *
        Div,    // /
        Eq,     // ==
        Ne,     // !=
        Lt,     // <
        Le,     // <=
        Assign, // =
        Return,                 // リターン
        ExpressionStatement,    // 式のステートメント
        Variable,               // 変数
        Num,                    // 整数
    }

    // BinaryTree
    public class Node(NodeKind kind, Node next, Node left, Node right, Variable variable, int value, int offset, string name)
    {
        public NodeKind Kind { get; } = kind;
        public Node Next { get; set; } = next;
        public Node Left { get; } = left;
        public Node Right { get; } = right;
        public int Value { get; } = value;
        public Variable Variable { get; } = variable;
        public int Offset { get; } = offset;
        public string Name { get; } = name;
    }

    internal class Variable(Variable next, string name, int offset)
    {
        public Variable Next { get; } = next;
        public string Name { get; } = name;
        public int Offset { get; set; } = offset;
    }

    internal record Program(Node Node, Variable Variable, int StackSize);
    private static Node NewNode(NodeKind kind, Node left, Node right) => new(kind, null!, left, right, null!, 0, 0, "");
    // 数字の場合は最後尾なので便宜上null!を使う。それ以外ではnull使わない。
    private static Node NewNodeNum(int val) => new(NodeKind.Num, null!, null!, null!, null!, val, 0, "");
    private static Node NewArray(NodeKind kind, Node expr) => new(kind, null!, expr, null!, null!, 0, 0, "");
    private static Node NewVariable(Variable variable) => new(NodeKind.Variable, null!, null!, null!, variable, 0, 0, "");
    
    private static Variable? Locals;

    internal static Program ToProgram(this Tokenize.Token token)
    {
        Token = token;
        Locals = null;
        Node head = NewNode(NodeKind.ExpressionStatement, null!, null!);
        Node current = head;
        while (Token.Kind != Tokenize.TokenKind.Eof)
        {
            current.Next = Stmt();
            current = current.Next;
        }
        Program program = new(head.Next, Locals, 0);
        return program;
    }

    private static Tokenize.Token Token { get; set; } = null!;

    private static Node Stmt()
    {
        if (Consume("return"))
        {
            var rnode = NewArray(NodeKind.Return, Expr());
            Expect(";");
            return rnode;
        }
        var node = NewArray(NodeKind.ExpressionStatement, Expr());
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
            var variable = FindVarible(token) ?? PushVariable(token.Str);
            return NewVariable(variable);
        }
        return NewNodeNum(ExpectNumber());
    }


    private static Variable? PushVariable(string str)
    {
        Variable variable = new(Locals, str, 0);
        Locals = variable;
        return variable;
    }

    private static Variable? FindVarible(Tokenize.Token token)
    {
        for(Variable var = Locals; var != null; var = var.Next)
        {
            if (var.Name.Length == token.Str.Length && var.Name == token.Str)
            {
                return var;
            }
        }
        return null;
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
        var token = Token;
        Token = Token.Next;
        return token;
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
