using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using vrc.csharp.tests.Helpers;
using vrc.csharp;
using System.Linq;

namespace vrc.csharp.tests
{
    [TestClass]
    public class DocumentTests
    {
        [TestMethod]
        public async Task ReadProperties()
        {
            var reader = new CodeReader((await ClassReady).SyntaxTreeRoot);
            var properties = reader.Properties.First();
            var methods = reader.Methods.First();
        }

        internal static Task<TestData> ClassReady => TestData.FromCode(@"
            namespace MyNamespace
            {
                class MyClass
                {
                    public string Prop1 {get;set;}
                    internal static string Prop2 {get;set;}
                    string _field;

                    public static void Test()
                    {

                    }
                }
            }");
    }
}
