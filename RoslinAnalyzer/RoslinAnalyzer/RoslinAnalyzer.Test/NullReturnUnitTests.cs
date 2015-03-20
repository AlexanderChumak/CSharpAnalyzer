using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CodeFixes;
using Microsoft.CodeAnalysis.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using TestHelper;
using RoslinAnalyzer;
using NUnit.Framework;

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
        
        [Test]
        public void TestMethodReturnNullForIEnumerable()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
            public IEnumerable FooMethodWithReturn()
            {
                return null;
            }
        }
    }";
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

        [Test]
        public void TestMethodReturnNullForNotIEnumerable()
        {
            var test = @"
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Diagnostics;

    namespace ConsoleApplication1
    {
        class TYPENAME
        {   
            public Object FooMethodWithReturn()
            {
                return null;
            }
        }
    }";

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

    }
}