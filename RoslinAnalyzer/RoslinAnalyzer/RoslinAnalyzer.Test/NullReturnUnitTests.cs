using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using System;
using System.Linq;
using TestHelper;
using RoslinAnalyzer;
using NUnit.Framework;
using System.Collections.Generic;

namespace RoslinAnalyzer.Test
{
    [TestFixture]
    public class NullReturnUnitTests : CodeFixVerifier
    {

        //No diagnostics expected to show up
        [Test]
        public void TestMethodWithVoid()
        {
            var test = @"";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase("IEnumerable<Object>")]
        [TestCase("IEnumerable<String>")]
        [TestCase("String[]")]
        [TestCase("Dictionary<String,Int32>")]
        public void TestMethodReturnNullForIEnumerable(String returnType)
        {
            var test = $@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {{
        class TYPENAME
        {{   
            public {returnType} FooMethodWithReturn()
            {{
                return null;
            }}
        }}
    }}";
            var expected = new DiagnosticResult
            {
                Id = MethodReturnNullAnalyzer.DiagnosticId,
                Message = String.Format("Method name '{0}' return null value", "FooMethodWithReturn"),
                Severity = DiagnosticSeverity.Warning,
                Locations =
                    new[] {
                            new DiagnosticResultLocation("Test0.cs", 15, 17)
                        }
            };

            VerifyCSharpDiagnostic(test, expected);
        }

        [TestCase("IEnumerable<Object>")]
        [TestCase("IEnumerable<String>")]
        [TestCase("String[]")]
        //[TestCase("Dictionary<String,Int32>")]
        public void TestMethodReturnNotNullForIEnumerable(String returnType)
        { 
            var test = $@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {{
        class TYPENAME
        {{   
            public {returnType} FooMethodWithReturn()
            {{
                return new String[0];
            }}
        }}
    }}";

            VerifyCSharpDiagnostic(test);
        }

        [TestCase("Object")]
        [TestCase("void")]
        public void TestMethodReturnNullForNotIEnumerable(String returnType)
        {
            var test = $@"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {{
        class TYPENAME
        {{   
            public {returnType} FooMethodWithReturn()
            {{
                return null;
            }}
        }}
    }}";

            VerifyCSharpDiagnostic(test);
        }

        protected override DiagnosticAnalyzer GetCSharpDiagnosticAnalyzer()
        {
            return new MethodReturnNullAnalyzer();
        }

        //protected override CodeFixProvider GetCSharpCodeFixProvider()
        //{
        //    return new RoslinAnalyzerCodeFixProvider();
        //}

        [TestCase(typeof(IEnumerable<>), Result = true)]
        [TestCase(typeof(IEnumerable<String>), Result = true)]
        [TestCase(typeof(String[]), Result = true)]
        [TestCase(typeof(String), Result = false)]
        [TestCase(typeof(Object), Result = false)]
        public Boolean TestIsTypeIEnumerable(Type type)
        {
            return MethodReturnNullAnalyzer.IsTypeIEnumerable(type);
        }
    }
}