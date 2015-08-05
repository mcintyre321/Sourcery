using System;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace Sourcery
{
    public static class ApplyExtension
    {
        public static TOut ApplyCommandAndLog<T, TOut>(this ISourcerer<T> sourcerer, Expression<Func<T, TOut>> action)
        {
            return (TOut) sourcerer.ApplyCommandAndLog(MethodCommand.CreateFromLambda(action));
        }
        public static void ApplyCommandAndLog<T>(this ISourcerer<T> sourcerer, Expression<Action<T>> action)
        {
            using (new PushScope<Gateway>(Gateway.Stack, new Gateway()))
            {
                sourcerer.ApplyCommandAndLog(MethodCommand.CreateFromLambda(action));
            }
        }
        //private static KeyValuePair<Type, object>[] ResolveArgs<T>(Expression<Func<T, object>> expression)
        //{
        //    var body = (System.Linq.Expressions.MethodCallExpression)expression.Body;
        //    var values = new List<KeyValuePair<Type, object>>();

        //    foreach (var argument in body.Arguments)
        //    {
        //        var exp = ResolveMemberExpression(argument);
        //        var type = argument.Type;

        //        var value = GetValue(exp);

        //        values.Add(new KeyValuePair<Type, object>(type, value));
        //    }

        //    return values.ToArray();
        //}
        //private static KeyValuePair<Type, object>[] ResolveArgs<T>(Expression<Action<T>> expression)
        //{
        //    var body = (System.Linq.Expressions.MethodCallExpression)expression.Body;
        //    var values = new List<KeyValuePair<Type, object>>();

        //    foreach (var argument in body.Arguments)
        //    {
        //        var exp = ResolveMemberExpression(argument);
        //        var type = argument.Type;

        //        var value = GetValue(exp);

        //        values.Add(new KeyValuePair<Type, object>(type, value));
        //    }

        //    return values.ToArray();
        //}
        //static MemberExpression ResolveMemberExpression(Expression expression)
        //{

        //    if (expression is MemberExpression)
        //    {
        //        return (MemberExpression)expression;
        //    }
        //    else if (expression is UnaryExpression)
        //    {
        //        // if casting is involved, Expression is not x => x.FieldName but x => Convert(x.Fieldname)
        //        return (MemberExpression)((UnaryExpression)expression).Operand;
        //    }
        //    else
        //    {
        //        throw new NotSupportedException(expression.ToString());
        //    }
        //}

        //private static object GetValue(MemberExpression exp)
        //{
        //    // expression is ConstantExpression or FieldExpression
        //    if (exp.Expression is ConstantExpression)
        //    {
        //        return (((ConstantExpression)exp.Expression).Value)
        //            .GetType()
        //            .GetField(exp.Member.Name)
        //            .GetValue(((ConstantExpression)exp.Expression).Value);
        //    }
        //    else if (exp.Expression is MemberExpression)
        //    {
        //        return GetValue((MemberExpression)exp.Expression);
        //    }
        //    else
        //    {
        //        throw new NotImplementedException();
        //    }
        //}
    }
}