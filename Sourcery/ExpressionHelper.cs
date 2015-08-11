using System;
using System.Linq;
using System.Linq.Expressions;

namespace Sourcery
{
    public class ExpressionHelper
    {
        public static object GetValueFromExpression(Expression exp)
        {
            var constant = exp as ConstantExpression;
            if (constant != null)
            {
                return constant.Value;
            }
            var memberAccess = exp as MemberExpression;
            if (memberAccess != null)
            {
                var constantSelector = (ConstantExpression)memberAccess.Expression;
                return ((dynamic)memberAccess.Member).GetValue(constantSelector.Value);
            }
            if (exp.NodeType == ExpressionType.TypeAs)
            {
                var mc = (UnaryExpression) exp;
                return GetValueFromExpression(mc.Operand);
            }
            throw new NotImplementedException();
        }
    }
}