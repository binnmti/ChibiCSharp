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
        Null,                   // NULL
    }

    internal enum TypeKind
    {
        Int,
    }

    internal class Node(NodeKind kind, Node? next, Type? type, Node? left, Node? right, Node? cond, Node? then, Node? els, Node? init, Node? inc, Node? body, Node? argument, string functionName, Variable? variable, int value)
    {
        public NodeKind Kind { get; } = kind;
        public Node? Next { get; set; } = next;
        public Type? Type { get; set; } = type;
        public Node? Left { get; } = left;
        public Node? Right { get; } = right;
        public Node? Condition { get; } = cond;
        public Node? Then { get; } = then;
        public Node? Else { get; } = els;
        public Node? Init { get; } = init;
        public Node? Inc { get; } = inc;
        public Node? Body { get; } = body;
        public Node? Argument { get; } = argument;
        public string FunctionName { get; } = functionName;
        public Variable? Variable { get; } = variable;
        public int Value { get; } = value;

        public static Node NewNode() => new(NodeKind.ExpressionStatement, null, null, null, null, null, null, null, null, null, null, null, "", null, 0);
        public static Node NewNode(NodeKind kind, Node? left, Node? right) => new(kind, null, null, left, right, null, null, null, null, null, null, null, "", null, 0);
        public static Node NewNodeBranch(NodeKind kind, Node cond, Node then, Node? els) => new(kind, null, null, null, null, cond, then, els, null, null, null, null, "", null, 0);
        public static Node NewNodeFor(NodeKind kind, Node? cond, Node then, Node? init, Node? inc) => new(kind, null, null, null, null, cond, then, null, init, inc, null, null, "", null, 0);
        public static Node NewNodeVariable(Variable variable) => new(NodeKind.Variable, null, null, null, null, null, null, null, null, null, null, null, "", variable, 0);
        public static Node NewNodeBody(Node body) => new(NodeKind.Block, null, null, null, null, null, null, null, null, null, body, null, "", null, 0);
        public static Node NewNodeNum(int val) => new(NodeKind.Num, null, null, null, null, null, null, null, null, null, null, null, "", null, val);
        public static Node NewNodeFunctionCall(string functionName, Node? argument) => new(NodeKind.FunctionCall, null, null, null, null, null, null, null, null, null, null, argument, functionName, null, 0);
    }

    internal class Type(TypeKind kind)
    {
        public TypeKind Kind { get; } = kind;
        public Type? Base { get; }
    }

    internal class VariableList(Variable variable)
    {
        public VariableList? Next { get; set; }
        public Variable Variable { get; } = variable;
    }

    internal class Variable(string name, bool isArgument, int offset, Type type)
    {
        public string Name { get; } = name;
        public bool IsArgument { get; } = isArgument;
        public Type Type { get; } = type;
        public int Offset { get; set; } = offset;
    }

    internal class Function(string name, Node node, VariableList variableList, VariableList parameters)
    {
        public Function? Next { get; set; }
        public string Name { get; } = name;
        public VariableList Parameters { get; } = parameters;

        public Node Node { get; } = node;
        public VariableList VariableList { get; } = variableList;

        public int StackSize { get; set; }  
    }
}
