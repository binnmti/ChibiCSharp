
using System.Diagnostics;

namespace ChibiCSharp;

// 生成規則 Generation rules ← EBNF
// 再帰下降構文解析
internal class Parse
{
    private Tokenize.Token Token { get; set; }

    private ChibiCSharp.Variable Locals { get; set; }

    internal Parse(Tokenize.Token token)
    {
        Token = token;
        Locals = new(null, "", 0);
    }

    internal ChibiCSharp.Function ToProgram()
    {
        ChibiCSharp.Function function = new("", ChibiCSharp.Node.NewNode(), Locals);
        ChibiCSharp.Function current = function;
        while (Token.Kind != Tokenize.TokenKind.Eof)
        {
            current.Next = Function();
            current = current.Next;
        }
        // 例外の時はhead.Nextがnull?
        Debug.Assert(function.Next != null);
        return function.Next;
    }

    private ChibiCSharp.Function Function()
    {
        var name = ExpectIndent();
        Expect("(");
        Expect(")");
        Expect("{");

        var head = ChibiCSharp.Node.NewNode();
        ChibiCSharp.Node current = head;
        while (!Consume("}"))
        {
            current.Next = Stmt();
            current = current.Next;
        }
        Debug.Assert(head.Next != null);
        var function = new ChibiCSharp.Function(name, head.Next, Locals);
        return function;
    }

    private ChibiCSharp.Node Stmt()
    {
        if (Consume("return"))
        {
            var rnode = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.Return, Expr(), null);
            Expect(";");
            return rnode;
        }
        if (Consume("if"))
        {
            Expect("(");
            var expr = Expr();
            Expect(")");
            var then = Stmt();
            ChibiCSharp.Node? els = null;
            if (Consume("else"))
            {
                els = Stmt();
            }
            return ChibiCSharp.Node.NewNodeBranch(ChibiCSharp.NodeKind.If, expr, then, els);
        }
        if (Consume("while"))
        {
            Expect("(");
            var expr = Expr();
            Expect(")");
            var then = Stmt();
            return ChibiCSharp.Node.NewNodeBranch(ChibiCSharp.NodeKind.While, expr, then, null);
        }
        if (Consume("for"))
        {
            Expect("(");
            ChibiCSharp.Node? init = null;
            if (!Consume(";"))
            {
                init = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.ExpressionStatement, Expr(), null);
                Expect(";");
            }
            ChibiCSharp.Node? expr = null;
            if (!Consume(";"))
            {
                expr = Expr();
                Expect(";");
            }
            ChibiCSharp.Node? inc = null;
            if (!Consume(")"))
            {
                inc = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.ExpressionStatement, Expr(), null);
                Expect(")");
            }
            var then = Stmt();
            return ChibiCSharp.Node.NewNodeFor(ChibiCSharp.NodeKind.For, expr, then, init, inc);
        }
        if (Consume("{"))
        {
            ChibiCSharp.Node head = ChibiCSharp.Node.NewNode();
            ChibiCSharp.Node current = head;
            while (!Consume("}"))
            {
                current.Next = Stmt();
                current = current.Next;
            }
            Debug.Assert(head.Next != null);
            return ChibiCSharp.Node.NewNodeBody(head.Next);
        }
        var node = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.ExpressionStatement, Expr(), null);
        Expect(";");
        return node;
    }

    private ChibiCSharp.Node Expr()
    {
        return Assign();
    }

    private ChibiCSharp.Node Assign()
    {
        var node = Equality();
        if (Consume("="))
        {
            node = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.Assign, node, Assign());
        }
        return node;
    }

    private ChibiCSharp.Node Equality()
    {
        var node = Relational();
        while (true)
        {
            if (Consume("=="))
            {
                node = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.Eq, node, Relational());
            }
            else if (Consume("!="))
            {
                node = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.Ne, node, Relational());
            }
            else
            {
                return node;
            }
        }
    }

    private ChibiCSharp.Node Relational()
    {
        var node = Add();
        while (true)
        {
            if (Consume("<"))
            {
                node = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.Lt, node, Add());
            }
            else if (Consume("<="))
            {
                node = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.Le, node, Add());
            }
            else if (Consume(">"))
            {
                node = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.Lt, Add(), node);
            }
            else if (Consume(">="))
            {
                node = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.Le, Add(), node);
            }
            else
            {
                return node;
            }
        }
    }

    private ChibiCSharp.Node Add()
    {
        var node = Mul();
        while (true)
        {
            if (Consume("+"))
            {
                node = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.Add, node, Mul());
            }
            else if (Consume("-"))
            {
                node = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.Sub, node, Mul());
            }
            else
            {
                return node;
            }
        }
    }

    private ChibiCSharp.Node Mul()
    {
        var node = Unary();
        while (true)
        {
            if (Consume("*"))
            {
                node = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.Mul, node, Unary());
            }
            else if (Consume("/"))
            {
                node = ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.Div, node, Unary());
            }
            else
            {
                return node;
            }
        }
    }

    private ChibiCSharp.Node Unary()
    {
        if (Consume("+"))
        {
            return Unary();
        }
        else if (Consume("-"))
        {
            return ChibiCSharp.Node.NewNode(ChibiCSharp.NodeKind.Sub, ChibiCSharp.Node.NewNodeNum(0), Unary());
        }
        return Primary();
    }

    private ChibiCSharp.Node Primary()
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
            // 関数
            if (Consume("("))
            {
                Expect(")");
                return ChibiCSharp.Node.NewNodeFunctionCall(token.Str);
            }
            // 変数
            else
            {
                var variable = FindVariable(token) ?? PushVariable(token.Str);
                return ChibiCSharp.Node.NewNodeVariable(variable);
            }
        }
        return ChibiCSharp.Node.NewNodeNum(ExpectNumber());
    }

    private ChibiCSharp.Variable PushVariable(string str)
    {
        ChibiCSharp.Variable variable = new(Locals, str, 0);
        Locals = variable;
        return variable;
    }

    private ChibiCSharp.Variable? FindVariable(Tokenize.Token token)
    {
        for (ChibiCSharp.Variable var = Locals; var != null; var = var.Next!)
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

    private string ExpectIndent()
    {
        if (Token.Kind != Tokenize.TokenKind.Identifier)
        {
            throw new Exception($"識別子ではありません");
        }
        var str = Token.Str;
        Debug.Assert(Token.Next != null);
        Token = Token.Next;
        return str;
    }
}