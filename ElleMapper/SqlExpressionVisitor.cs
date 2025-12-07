using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ElleMapper
{
    public class SqlExpressionVisitor : ExpressionVisitor
    {
        private readonly ISqlDialect _dialect;
        private readonly List<object?> _parameters;
        private readonly StringBuilder _sql;

        public SqlExpressionVisitor(ISqlDialect dialect, List<object?> parameters)
        {
            _dialect = dialect;
            _parameters = parameters;
            _sql = new StringBuilder();
        }

        public string Translate(Expression expression)
        {
            _sql.Clear();
            Visit(expression);
            return _sql.ToString();
        }

        protected override Expression VisitLambda<T>(Expression<T> node)
        {
            return this.Visit(node.Body);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            _sql.Append('(');

            Visit(node.Left);

            _sql.Append(' ');
            _sql.Append(GetSqlOperator(node.NodeType));
            _sql.Append(' ');

            Visit(node.Right);

            _sql.Append(')');
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            if (node.Expression is not null && node.Expression.NodeType == ExpressionType.Parameter)
            {
                string columnName = node.Member.Name;
                _sql.Append(_dialect.QuoteIdentifier(columnName));
            }
            else
            {
                var value = Expression.Lambda(node).Compile().DynamicInvoke();
                AppendParameter(value);
            }
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            AppendParameter(node.Value);
            return node;
        }

        private string GetSqlOperator(ExpressionType type)
        {
            return type switch
            {
                ExpressionType.Equal => "=",
                ExpressionType.NotEqual => "<>",
                ExpressionType.GreaterThan => ">",
                ExpressionType.GreaterThanOrEqual => ">=",
                ExpressionType.LessThan => "<",
                ExpressionType.LessThanOrEqual => "<=",
                ExpressionType.AndAlso => "AND",
                ExpressionType.OrElse => "OR",
                _ => throw new NotSupportedException($"Unsupported operator: {type}")
            };
        }

        private void AppendParameter(object? value)
        {
            string parameterName = $"@p{_parameters.Count}";
            _parameters.Add(value);
            _sql.Append(parameterName);
        }

        protected override Expression VisitUnary(UnaryExpression node)
        {
            if (node.NodeType == ExpressionType.Quote)
            {
                // Recursively visit the operand (the actual Lambda Expression)
                return Visit(node.Operand);
            }

            if (node.NodeType == ExpressionType.Not)
            {
                if (node.Operand.NodeType == ExpressionType.MemberAccess)
                {
                    var memberExpression = (MemberExpression)node.Operand;
                    var memberType = Nullable.GetUnderlyingType(memberExpression.Type) ?? memberExpression.Type;

                    if (memberType == typeof(bool))
                    {
                        Visit(node.Operand);

                        _sql.Append(" = 0");

                        return node;
                    }
                }

                _sql.Append("NOT (");
                Visit(node.Operand);
                _sql.Append(")");

                return node;
            }

            throw new NotSupportedException($"Unsupported unary operator: {node.NodeType}");
        }
    }
}
