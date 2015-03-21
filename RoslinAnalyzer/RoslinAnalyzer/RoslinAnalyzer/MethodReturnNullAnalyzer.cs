using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.Diagnostics;
using RoslinAnalyzer.Helpers;

namespace RoslinAnalyzer
{
    [DiagnosticAnalyzer(LanguageNames.CSharp)]
    public class MethodReturnNullAnalyzer : DiagnosticAnalyzer
    {
        public const string DiagnosticId = "RN1";

        // You can change these strings in the Resources.resx file. If you do not want your analyzer to be localize-able, you can use regular strings for Title and MessageFormat.
        internal static readonly LocalizableString Title = new LocalizableResourceString(nameof(Resources.AnalyzerTitle), Resources.ResourceManager, typeof(Resources));
        internal static readonly LocalizableString MessageFormat = new LocalizableResourceString(nameof(Resources.NullReturnAnalyzerMessageFormat), Resources.ResourceManager, typeof(Resources));
        internal static readonly LocalizableString Description = new LocalizableResourceString(nameof(Resources.AnalyzerDescription), Resources.ResourceManager, typeof(Resources));
        internal const string Category = "Naming";

        internal static DiagnosticDescriptor Rule = new DiagnosticDescriptor(DiagnosticId, Title, MessageFormat, Category, DiagnosticSeverity.Warning, isEnabledByDefault: true);

        public override ImmutableArray<DiagnosticDescriptor> SupportedDiagnostics { get { return ImmutableArray.Create(Rule); } }

        public override void Initialize(AnalysisContext context)
        {
            // TODO: Consider registering other actions that act on syntax instead of or in addition to symbols
            //context.RegisterSymbolAction(AnalyzeSymbol, SymbolKind.NamedType);

            context.RegisterSyntaxNodeAction(
              AnalyzeNode, SyntaxKind.ReturnStatement);
        }

        private void AnalyzeNode(SyntaxNodeAnalysisContext context)
        {
            var returnStatement =
              context.Node as ReturnStatementSyntax;
            if (returnStatement == null) return;

            var md = returnStatement.AncestorsAndSelf().OfType<MethodDeclarationSyntax>().FirstOrDefault();
            if (md == null) return;

            var rt = context.SemanticModel.GetDeclaredSymbol(md).ReturnType.ToMetadataName();

            Type returnTypeType = Type.GetType(rt);
            if (returnTypeType == null) return;
            if (!IsTypeIEnumerable(returnTypeType)) return;

            var isReturnNullValue = false;

            isReturnNullValue = (returnStatement.Expression as LiteralExpressionSyntax)?.Kind() == SyntaxKind.NullLiteralExpression;

            //if(!isReturnNullValue)
            //{
            //    var identifierName = returnStatement.Expression as IdentifierNameSyntax;
            //}

            if (isReturnNullValue)
            {
                var methodName = md.Identifier;
                var diagnostic = Diagnostic.Create(Rule, returnStatement.GetLocation(), methodName);

                context.ReportDiagnostic(diagnostic); 
            }

            

            //returnStatement.
            //var memberSymbol = context.SemanticModel.
            //  GetSymbolInfo(memberAccessExpr).Symbol as IMethodSymbol;
            //if (!memberSymbol?.ToString().StartsWith(
            //  "System.Text.RegularExpressions.Regex.Match") ?? true)
            //    return;
            //var argumentList = invocationExpr.ArgumentList as ArgumentListSyntax;
            //if ((argumentList?.Arguments.Count ?? 0) < 2) return;
            //var regexLiteral =
            //  argumentList.Arguments[1].Expression as LiteralExpressionSyntax;
            //if (regexLiteral == null) return;
            //var regexOpt = context.SemanticModel.GetConstantValue(regexLiteral);
            //if (!regexOpt.HasValue) return;
            //var regex = regexOpt.Value as string;
            //if (regex == null) return;

        }
        

        public IEnumerable<String> Foo()
        {
            IEnumerable<String> o = null;
            var test = @"";
            if (test == null)
                return new String[1] { "" };
            else
                return o;
            return null;
        }

        public static Boolean IsTypeIEnumerable(Type type)
        {
            if (type.IsArray)
                return true;

            if (!type.GetTypeInfo().IsGenericType)
                return false;

            if (type.GetTypeInfo().GetGenericTypeDefinition() == typeof(IEnumerable<>))
                return true;

            return type.GetTypeInfo().ImplementedInterfaces.Any(
            i => i.GetTypeInfo().IsGenericType &&
            i.GetGenericTypeDefinition() == typeof(IEnumerable<>));
        }

        //private static void AnalyzeSymbol(SymbolAnalysisContext context)
        //{
        //    // TODO: Replace the following code with your own analysis, generating Diagnostic objects for any issues you find
        //    var namedTypeSymbol = (INamedTypeSymbol)context.Symbol;

        //    // Find just those named type symbols with names containing lowercase letters.
        //    if (namedTypeSymbol.Name.ToCharArray().Any(char.IsLower))
        //    {
        //        // For all such symbols, produce a diagnostic.
        //        var diagnostic = Diagnostic.Create(Rule, namedTypeSymbol.Locations[0], namedTypeSymbol.Name);

        //        context.ReportDiagnostic(diagnostic);
        //    }
        //}
    }
}
