
using System.Diagnostics;

namespace ChibiCSharpCompiler;

// 生成規則 Generation rules ← EBNF
// 再帰下降構文解析
internal class Parse
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
        If,                     // If
        ExpressionStatement,    // 式のステートメント
        Variable,               // 変数
        Num,                    // 整数
    }

    internal record Program(Node Node, Variable Variable, int StackSize);

    internal class Node(NodeKind kind, Node? next, Node? left, Node? right, Node? cond, Node? then, Node? els, Variable? variable, int value)
    {
        public NodeKind Kind { get; } = kind;
        public Node? Next { get; set; } = next;
        public Node? Left { get; } = left;
        public Node? Right { get; } = right;
        public Node? Condition { get; } = cond;
        public Node? Then { get;} = then;
        public Node? Else { get;  } = els;
        public Variable? Variable { get; } = variable;
        public int Value { get; } = value;

        public static Node NewNode(NodeKind kind, Node? left, Node? right) => new(kind, null, left, right, null, null, null, null, 0);
        public static Node NewNodeIf(Node cond, Node then, Node? els) => new(NodeKind.If, null, null, null, cond, then, els, null, 0);
        public static Node NewNodeVariable(Variable variable) => new(NodeKind.Variable, null, null, null, null, null, null, variable, 0);
        public static Node NewNodeNum(int val) => new(NodeKind.Num, null, null, null, null, null, null, null, val);
    }

    internal class Variable(Variable? next, string name, int offset)
    {
        public Variable? Next { get; } = next;
        public string Name { get; } = name;
        public int Offset { get; set; } = offset;
    }

    internal Parse(Tokenize.Token token)
    {
        Token = token;
        Locals = new(null, "", 0);
    }

    private Tokenize.Token Token { get; set; }

    private Variable Locals { get; set; }

    internal Program ToProgram()
    {
        Node head = Node.NewNode(NodeKind.ExpressionStatement, null, null);
        Node current = head;
        while (Token.Kind != Tokenize.TokenKind.Eof)
        {
            current.Next = Stmt();
            current = current.Next;
        }
        // 例外の時はhead.Nextがnull
        Program program = new(head.Next!, Locals, 0);
        return program;
    }

    private Node Stmt()
    {
        if (Consume("return"))
        {
            var rnode = Node.NewNode(NodeKind.Return, Expr(), null);
            Expect(";");
            return rnode;
        }
        if (Consume("if"))
        {
            Expect("(");
            var expr = Expr();
            Expect(")");
            var then = Stmt();
            Node? els = null;
            if (Consume("else"))
            {
                els = Stmt();
            }
            return Node.NewNodeIf(expr, then, els);
        }
        var node = Node.NewNode(NodeKind.ExpressionStatement, Expr(), null);
        Expect(";");
        return node;
    }

    private Node Expr()
    {
        return Assign();
    }

    private Node Assign()
    {
        var node = Equality();
        if (Consume("="))
        {
            node = Node.NewNode(NodeKind.Assign, node, Assign());
        }
        return node;
    }

    private Node Equality()
    {
        var node = Relational();
        while (true)
        {
            if (Consume("=="))
            {
                node = Node.NewNode(NodeKind.Eq, node, Relational());
            }
            else if (Consume("!="))
            {
                node = Node.NewNode(NodeKind.Ne, node, Relational());
            }
            else
            {
                return node;
            }
        }
    }

    private Node Relational()
    {
        var node = Add();
        while (true)
        {
            if (Consume("<"))
            {
                node = Node.NewNode(NodeKind.Lt, node, Add());
            }
            else if (Consume("<="))
            {
                node = Node.NewNode(NodeKind.Le, node, Add());
            }
            else if (Consume(">"))
            {
                node = Node.NewNode(NodeKind.Lt, Add(), node);
            }
            else if (Consume(">="))
            {
                node = Node.NewNode(NodeKind.Le, Add(), node);
            }
            else
            {
                return node;
            }
        }
    }

    private Node Add()
    {
        var node = Mul();
        while (true)
        {
            if (Consume("+"))
            {
                node = Node.NewNode(NodeKind.Add, node, Mul());
            }
            else if (Consume("-"))
            {
                node = Node.NewNode(NodeKind.Sub, node, Mul());
            }
            else
            {
                return node;
            }
        }
    }

    private Node Mul()
    {
        var node = Unary();
        while (true)
        {
            if (Consume("*"))
            {
                node = Node.NewNode(NodeKind.Mul, node, Unary());
            }
            else if (Consume("/"))
            {
                node = Node.NewNode(NodeKind.Div, node, Unary());
            }
            else
            {
                return node;
            }
        }
    }

    private Node Unary()
    {
        if (Consume("+"))
        {
            return Unary();
        }
        else if (Consume("-"))
        {
            return Node.NewNode(NodeKind.Sub, Node.NewNodeNum(0), Unary());
        }
        return Primary();
    }

    private Node Primary()
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
            var variable = FindVariable(token) ?? PushVariable(token.Str);
            return Node.NewNodeVariable(variable);
        }
        return Node.NewNodeNum(ExpectNumber());
    }

    private Variable PushVariable(string str)
    {
        Variable variable = new(Locals, str, 0);
        Locals = variable;
        return variable;
    }

    private Variable? FindVariable(Tokenize.Token token)
    {
        for(Variable var = Locals; var != null; var = var.Next!)
        {
            if (var.Name.Length == token.Str.Length && var.Name == token.Str)
            {
                return var;
            }
        }
        return null;
    }

    private bool Consume(string op)
    {
        if (Token.Kind != Tokenize.TokenKind.Reserved || Token.Str != op)
        {
            return false;
        }
        Debug.Assert(Token.Next != null);
        Token = Token.Next;
        return true;
    }

    private Tokenize.Token? ConsumeIdnet()
    {
        if (Token.Kind != Tokenize.TokenKind.Identifier)
        {
            return null;
        }
        var token = Token;
        Debug.Assert(Token.Next != null);
        Token = Token.Next;
        return token;
    }

    private void Expect(string op)
    {
        if (Token.Kind != Tokenize.TokenKind.Reserved || Token.Str != op)
        {
            throw new Exception($"'{op}'ではありません");
        }
        Debug.Assert(Token.Next != null);
        Token = Token.Next;
    }

    private int ExpectNumber()
    {
        if (Token.Kind != Tokenize.TokenKind.Number)
        {
            throw new Exception($"整数ではありません");
        }
        var val = Token.Value;
        Debug.Assert(Token.Next != null);
        Token = Token.Next;
        return val;
    }
}
