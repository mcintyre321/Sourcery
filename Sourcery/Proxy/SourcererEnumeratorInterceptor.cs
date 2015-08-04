using System;
using System.Collections.Generic;
using System.Linq;
using Castle.DynamicProxy;

namespace Sourcery.Proxy
{
    public class SourcererEnumeratorInterceptor : SourcererInterceptor
    {
        private readonly IEnumerator<object> _enumerator;

        public SourcererEnumeratorInterceptor(IEnumerator<object> enumerator, ISourcerer sourcerer, ProxyExtension.MakeCommandDelegate makeCommand, string fragment, SourcererInterceptor parent) 
            : base(enumerator, sourcerer, makeCommand, fragment, parent)
        {
            _enumerator = enumerator;
        }

        private int pos = -1;
        public override void Intercept(IInvocation info)
        {
            if (info.Method.Name == "MoveNext")
            {
                pos++;

                info.ReturnValue = _enumerator.MoveNext();
                return;
            }
            else if (info.Method.Name == "Reset")
            {
                _enumerator.Reset();
                pos = -1;
                info.ReturnValue = null;
                return;
            }
            else if (info.Method.Name == "get_Current")
            {
                var result = _enumerator.Current;
                HandleResult(info, result, pos.ToString());
                return;
            }
            else
            {
                base.Intercept(info);
                return;
            }

        }
    }
}