namespace ChibiCSharp;

internal static class TypeResolver
{
    internal static void AddType(ChibiCSharp.Function function)
    {
        for (var fn = function; fn != null; fn = fn.Next)
        {
            for (var node = fn.Node; node != null; node = node.Next!)
            {
                Visit(node);
            }
        }
    }

    private static void Visit(ChibiCSharp.Node? node)
    {
        if (node == null)
        {
            return;
        }
        Visit(node.Left);
        Visit(node.Right);
        Visit(node.Condition);
        Visit(node.Then);
        Visit(node.Else);
        Visit(node.Init);
        Visit(node.Inc);

        for (var n = node.Body; n != null; n = n.Next)
        {
            Visit(n);
        }
        for (var n = node.Argument; n != null; n = n.Next)
        {
            Visit(n);
        }
        switch (node.Kind)
        {
            case ChibiCSharp.NodeKind.Mul:
            case ChibiCSharp.NodeKind.Div:
            case ChibiCSharp.NodeKind.Eq:
            case ChibiCSharp.NodeKind.Ne:
            case ChibiCSharp.NodeKind.Lt:
            case ChibiCSharp.NodeKind.Le:
            case ChibiCSharp.NodeKind.FunctionCall:
            case ChibiCSharp.NodeKind.Num:
                node.Type = new(ChibiCSharp.TypeKind.Int);
                break;

            case ChibiCSharp.NodeKind.Add:
            case ChibiCSharp.NodeKind.Sub:
            case ChibiCSharp.NodeKind.Assign:
                node.Type = node.Left?.Type;
                break;

            case ChibiCSharp.NodeKind.Variable:
                node.Type = node.Variable?.Type;
                break;
        }
    }
}
