using System.Linq.Expressions;

namespace Raiqub.Expressions;

internal sealed class ReplaceParameterExpressionVisitor : ExpressionVisitor
{
    private readonly ParameterExpression _from;
    private readonly ParameterExpression _to;

    public ReplaceParameterExpressionVisitor(ParameterExpression from, ParameterExpression to)
    {
        _from = from;
        _to = to;
    }

    protected override Expression VisitParameter(ParameterExpression node)
    {
        return node == _from ? _to : node;
    }
}
