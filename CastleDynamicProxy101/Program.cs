using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Castle.Core.Interceptor;
using Castle.DynamicProxy;

namespace CastleDynamicProxy101
{
    class Program
    {
        static void Main(string[] args)
        {
            var instance = Activator.CreateInstance<Model>();
            instance.String1 = "Test";
            Console.WriteLine(instance.String1);


            var proxyGenerator = new ProxyGenerator();
            var options = new ProxyGenerationOptions(new ProxyGenerationHook());
            var instanceMissing = proxyGenerator.CreateInterfaceProxyWithTarget<IModel>(new Model(), options, new MethodMissingInterceptor());
            instanceMissing.String1 = "Test2";
            instanceMissing.Get();
            Console.WriteLine(instanceMissing.String1);

            TestabilityTest.CreateUntestableTestableObject();
            TestabilityTest.CreateUntestableTestableFileReader();

            Console.ReadLine();
        }
    }

    internal class MethodMissingInterceptor : IInterceptor
    {
        public void Intercept(IInvocation invocation)
        {
            Console.WriteLine("Calling... " + invocation.Method);
            invocation.Proceed();
        }
    }

    public class ProxyGenerationHook : IProxyGenerationHook
    {
        public bool ShouldInterceptMethod(Type type, MethodInfo memberInfo)
        {
            return !memberInfo.Name.StartsWith("get_", StringComparison.Ordinal);
        }

        public void NonVirtualMemberNotification(Type type, MemberInfo memberInfo)
        {
        }

        public void MethodsInspected()
        {
        }
    }


    public interface IModel
    {
        string String1 { get; set; }
        string String2 { get; set; }
        string String3 { get; set; }
        bool Bool1 { get; set; }
        void Get();
    }

    public class Model : IModel
    {
        public string String1 { get; set; }
        public string String2 { get; set; }
        public string String3 { get; set; }
        public bool Bool1 { get; set; }

        public void Get()
        {
            Console.WriteLine("Getting...");
        }
    }
}
