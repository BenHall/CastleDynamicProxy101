using System;
using System.Linq;
using System.Reflection;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;

namespace CastleDynamicProxy101
{
    internal interface ITestability
    {
        T MakeUntestableTestable<T>(object dispatchTarget) where T : class;
        T MakeUntestableTestable<T>(Type dispatchTarget) where T : class;
    }

    internal class Testability : ITestability
    {
        public T MakeUntestableTestable<T>(object dispatchTarget) where T : class
        {
            var proxyGenerator = new ProxyGenerator();
            return proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(new InstanceMethodDispatcher(dispatchTarget));
        }

        public T MakeUntestableTestable<T>(Type dispatchTarget) where T : class
        {
            var proxyGenerator = new ProxyGenerator();
            return proxyGenerator.CreateInterfaceProxyWithoutTarget<T>(new StaticMethodDispatcher(dispatchTarget));
        }
    }

    internal class StaticMethodDispatcher : IInterceptor
    {
        private readonly Type _nasty;

        public StaticMethodDispatcher(Type nasty)
        {
            _nasty = nasty;
        }

        public void Intercept(IInvocation invocation)
        {
            //var methodInfo = _nasty.GetMethod(invocation.Method.Name, BindingFlags.Static | 
            //                                                           BindingFlags.Instance | 
            //                                                           BindingFlags.Public);

            var methodInfos = _nasty.GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);
            var methodInfo = methodInfos.Single(x => x.Name == invocation.Method.Name && x.GetParameters().Count() == invocation.Arguments.Count());

            invocation.ReturnValue = methodInfo.Invoke(_nasty, invocation.Arguments);
        }
    }

    internal class InstanceMethodDispatcher : IInterceptor
    {
        private readonly object _nasty;

        public InstanceMethodDispatcher(object nasty)
        {
            _nasty = nasty;
        }

        public void Intercept(IInvocation invocation)
        {
            var methodInfo = _nasty.GetType().GetMethod(invocation.Method.Name, BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public);

            invocation.ReturnValue = methodInfo.Invoke(_nasty, new object[] { });
        }
    }

    public interface INice
    {
        string Blah();
    }

    sealed class NastySealed
    {
        public string Blah()
        {
            return "Untestable horrible sealed object";
        }
    }

    static class NastyStatic
    {
        public static string Blah()
        {
            return "Untestable horrible static object";
        }
    }
}