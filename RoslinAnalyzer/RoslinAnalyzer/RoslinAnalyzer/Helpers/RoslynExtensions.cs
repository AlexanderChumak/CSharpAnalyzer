using Microsoft.CodeAnalysis;
using System.Linq;

namespace RoslinAnalyzer.Helpers
{
    public static class RoslynExtensions
    {
        public static string GetFullTypeString(this INamedTypeSymbol type)
        {
            string result = type.Name;

            if (type.TypeArguments.Count() > 0)
            {
                result += "<";

                bool isFirstIteration = true;
                foreach (INamedTypeSymbol typeArg in type.TypeArguments)
                {
                    if (isFirstIteration)
                    {
                        isFirstIteration = false;
                    }
                    else
                    {
                        result += ", ";
                    }

                    result += typeArg.GetFullTypeString();
                }

                result += ">";
            }

            return result;
        }
    }
}
