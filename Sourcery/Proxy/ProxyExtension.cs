using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using Castle.DynamicProxy;

namespace Sourcery.Proxy
{
    public static class ProxyExtension
    {
        public delegate CommandBase MakeCommandDelegate(IInvocation info, string[] path);
        static readonly MyProxyGenerator Factory = new MyProxyGenerator();

        class MyProxyGenerator : ProxyGenerator
        {
            public object CreateClassProxyWithoutRunningCtor(Type type, ProxyGenerationOptions pgo, SourcererInterceptor sourcererInterceptor)
            {
                var prxType = this.CreateClassProxyType(type, new Type[] { }, pgo);
                var instance = FormatterServices.GetUninitializedObject(prxType);
                SetInterceptors(instance, new IInterceptor[]{sourcererInterceptor});
                return instance;
            }
             

            private void SetInterceptors(object proxy, params IInterceptor[] interceptors)
            {
                var field = proxy.GetType().GetField("__interceptors");
                field.SetValue(proxy, interceptors);
            }

           
        }
        static ProxyExtension()
        {
            MakeCommand = (invocation, path) => (CommandBase)new MethodCommand(invocation.TargetType.FullName, invocation.Method.Name, invocation.Arguments, path);
        }



        internal static object Wrap(object target, Type type, ISourcerer sourcerer, string fragment, MakeCommandDelegate makeCommand, SourcererInterceptor parent = null)
        {
            var sourcererInterceptor = new SourcererInterceptor(target, sourcerer, makeCommand, fragment, parent);
            var pgo = new ProxyGenerationOptions();
            pgo.AddMixinInstance(sourcererInterceptor);
            pgo.Hook = new MustBeProxyableHook();
            if (type.IsInterface)
            {
                var proxy = Factory.CreateInterfaceProxyWithTarget(type, target, pgo, sourcererInterceptor);
                target.SetProxy(proxy);
                return proxy;
            }
            else
            {
                var proxy = Factory.CreateClassProxyWithoutRunningCtor(type, pgo, sourcererInterceptor);
                target.SetProxy(proxy);
                return proxy;
            }
        }

        internal static object Wrap(IEnumerator<object> target, Type type, ISourcerer sourcerer, string fragment, MakeCommandDelegate makeCommand, SourcererInterceptor parent)
        {
            var sourcererEnumeratorInterceptor = new SourcererEnumeratorInterceptor(target, sourcerer, makeCommand ?? MakeCommand, fragment, parent);
            var options = new ProxyGenerationOptions();
            options.AddMixinInstance(sourcererEnumeratorInterceptor);
            return Factory.CreateInterfaceProxyWithTargetInterface(type, target, options, sourcererEnumeratorInterceptor);
        }

        internal static MakeCommandDelegate MakeCommand { get; set; }

        public static T Proxy<T>(this ISourcerer<T> sourcerer)
        {
            var proxy = (T)Wrap(sourcerer.ReadModel, typeof(T), sourcerer, null, MakeCommand);

            return proxy;
        }
        private static readonly ConditionalWeakTable<object, Type> InnerTypeLookup = new ConditionalWeakTable<object, Type>();
        public static Type InnerType(this object o)
        {
            return InnerTypeLookup.GetValue(o, c => null);
        }
        public static void SetInnerType(this object o, Type type)
        {
            InnerTypeLookup.Remove(o);
            InnerTypeLookup.Add(o, type);
        }


    }

    internal class MustBeProxyableHook : AllMethodsHook
    {
        public override void NonProxyableMemberNotification(Type type, System.Reflection.MemberInfo memberInfo)
        {
            throw new ProxyGenerationException("Cannot generate proxy - " + type.FullName + " " + memberInfo.Name + " was not proxyable");
        }
    }
}