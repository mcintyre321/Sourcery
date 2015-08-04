using System;
using System.Linq.Expressions;
using ExpressionBuilder;

namespace Sourcery.Tests
{
    [Serializable]
    public class LambdaCommand<T> :CommandBase where T:class
    {
        public EditableExpression Action { get; set; }
        public LambdaCommand(Expression<Action<T>> action):this()
        {
            this.Action = EditableExpression.CreateEditableExpression(action);
        }
        public LambdaCommand(){} 


        protected override object Invoke(object o)
        {
            return ((LambdaExpression) Action.ToExpression()).Compile().DynamicInvoke(o);
        }
    }
}