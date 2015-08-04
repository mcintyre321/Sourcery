using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using Castle.DynamicProxy;
using IInterceptor = Castle.DynamicProxy.IInterceptor;

namespace Sourcery.Proxy
{
    public class SourcererInterceptor : IInterceptor, ISourcererProxy
    {
        private readonly object _target;
        internal readonly ISourcerer _sourcerer;
        internal readonly ProxyExtension.MakeCommandDelegate _makeCommand;
        private readonly string _fragment;
        protected readonly SourcererInterceptor _parent;

        public SourcererInterceptor(object target, ISourcerer sourcerer, ProxyExtension.MakeCommandDelegate makeCommand, string fragment = null, SourcererInterceptor parent = null)
        {
            _target = target;
            _sourcerer = sourcerer;
            _makeCommand = makeCommand;
            _fragment = fragment;
            _parent = parent;
            _target.SetInnerType(sourcerer.ReadModel.GetType());
        }

        public void BeforeInvoke(IInvocation info)
        {

        }

        object LogCommand(IInvocation info)
        {
            using (ThreadScoper.Use(new Gateway()))
            {
                return _sourcerer.ApplyCommandAndLog(_makeCommand(info, Path));
            }
        }
        public virtual void Intercept(IInvocation info)
        {
            if (info.Method.DeclaringType == typeof(SourcererInterceptor) || info.Method.DeclaringType == typeof(ISourcererProxy))
            {
                info.Proceed();
                return;
            }
            object result = null;


            if (!IsSideEffectFree(info))
            {
                result = LogCommand(info);
            }
            else
            {

                result = info.MethodInvocationTarget.Invoke(_target,
                                                   BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                                                   null, info.Arguments, null);
                //result = info.ReturnValue;
            }
            var name = info.Method.Name.StartsWith("get_") ? info.Method.Name.Substring(4) : info.Method.Name;

            HandleResult(info, result, name);
            return;
        }

        protected void HandleResult(IInvocation info, object result, string fragment)
        {
            if (result != null && !IsImmutable(info.Method.ReturnType) && info.Method.DeclaringType != typeof (object))
            {

                var newSourcerer = result.Sourcerer() ?? _sourcerer;
                if (newSourcerer != this._sourcerer)
                {
                    info.ReturnValue = result;
                    return;
                }
                var proxyType = result.GetType();
                if (proxyType.IsSealed || proxyType.GetMethods().Any(p => p.IsVirtual == false))
                {
                    proxyType = info.Method.ReturnType;
                    if (proxyType.IsSealed)
                    {
                        info.ReturnValue = result;
                        return;
                    }
                }

                if (typeof (IEnumerator<object>).IsAssignableFrom(info.Method.ReturnType))
                {
                    info.ReturnValue = ProxyExtension.Wrap((IEnumerator<object>) result, info.Method.ReturnType, _sourcerer, fragment, _makeCommand, this);
                    return;
                }

                info.ReturnValue = ProxyExtension.Wrap(result, proxyType, newSourcerer, fragment, _makeCommand, this);
                return;
            }

            info.ReturnValue = result;
            return;
        }

        private bool IsImmutable(Type returnType)
        {
            return returnType.IsValueType || returnType == typeof(string);
        }

        public static IList<Func<IInvocation, bool>> Rules = new List<Func<IInvocation, bool>>()
            {
                i => i.Method.Name.StartsWith("get_"),
                i => i.Method.Name.StartsWith("Allow"),
                i => i.Method.Name.StartsWith("ToString"),
                i => i.Method.Name == "GetEnumerator",
            };
        private bool IsSideEffectFree(IInvocation info)
        {
            
            return Rules.Any(r => r(info));
        }

        public void AfterInvoke(IInvocation info, object returnValue)
        {

        }

        public string[] Path
        {
            get { return this.Recurse(i => i._parent).Reverse().Select(p => p.Fragment).Where(s => s != null).ToArray(); }
        }

        public object Inner
        {
            get { return _target; }
        }

        public ISourcererProxy Parent
        {
            get { return _parent; }
        }

        public ISourcerer RootSourcerer
        {
            get { return _sourcerer; }
        }

        public string Fragment
        {
            get
            {
                return _fragment;
            }
        }
    }


}