using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChibiCSharp;

internal class ChibiCSharp
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
        Return,                 //
        If,                     //
        While,                  //
        For,                    //
        Block,                  // ブロック
        FunctionCall,           // 関数呼び出し
        ExpressionStatement,    // 式のステートメント
        Variable,               // 変数
        Num,                    // 整数
    }

    internal record Program(Node Node, Variable Variable, int StackSize);

    internal class Node(NodeKind kind, Node? next, Node? left, Node? right, Node? cond, Node? then, Node? els, Node? init, Node? inc, Node? body, string functionName, Variable? variable, int value)
    {
        public NodeKind Kind { get; } = kind;
        public Node? Next { get; set; } = next;
        public Node? Left { get; } = left;
        public Node? Right { get; } = right;
        public Node? Condition { get; } = cond;
        public Node? Then { get; } = then;
        public Node? Else { get; } = els;
        public Node? Init { get; } = init;
        public Node? Inc { get; } = inc;
        public Node? Body { get; } = body;
        public string FunctionName { get; } = functionName;
        public Variable? Variable { get; } = variable;
        public int Value { get; } = value;

        public static Node NewNode() => new(NodeKind.ExpressionStatement, null, null, null, null, null, null, null, null, null, "", null, 0);
        public static Node NewNode(NodeKind kind, Node? left, Node? right) => new(kind, null, left, right, null, null, null, null, null, null, "", null, 0);
        public static Node NewNodeBranch(NodeKind kind, Node cond, Node then, Node? els) => new(kind, null, null, null, cond, then, els, null, null, null, "", null, 0);
        public static Node NewNodeFor(NodeKind kind, Node? cond, Node then, Node? init, Node? inc) => new(kind, null, null, null, cond, then, null, init, inc, null, "", null, 0);
        public static Node NewNodeVariable(Variable variable) => new(NodeKind.Variable, null, null, null, null, null, null, null, null, null, "", variable, 0);
        public static Node NewNodeBody(Node body) => new(NodeKind.Block, null, null, null, null, null, null, null, null, body, "", null, 0);
        public static Node NewNodeNum(int val) => new(NodeKind.Num, null, null, null, null, null, null, null, null, null, "", null, val);
        public static Node NewNodeFunctionCall(string functionName) => new(NodeKind.FunctionCall, null, null, null, null, null, null, null, null, null, functionName, null, 0);
    }

    internal class Variable(Variable? next, string name, int offset)
    {
        public Variable? Next { get; } = next;
        public string Name { get; } = name;
        public int Offset { get; set; } = offset;
    }

    internal class Function(string name, Node node, Variable variable)
    {
        public Function? Next { get; set; }
        public string Name { get; } = name;
        public Node Node { get; } = node;
        public Variable Variable { get; } = variable;
        public int StackSize { get; set; }  
    }
}
