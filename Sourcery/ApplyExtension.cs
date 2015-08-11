using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using Sourcery.Helpers;

namespace Sourcery
{
    public static class ApplyExtension
    {
        public static TOut ApplyCommandAndLog<T, TOut>(this ISourcedObject<T> sourcedObject, Expression<Func<T, TOut>> action)
        {
            return (TOut) sourcedObject.ApplyCommandAndLog(MethodCommand.CreateFromLambda(action));
        }

        public static void ApplyCommandAndLog<T>(this ISourcedObject<T> sourcedObject, Expression<Action<T>> action)
        {
            sourcedObject.ApplyCommandAndLog(MethodCommand.CreateFromLambda(action));
        }

        
    }
}