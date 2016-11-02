using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace vrc.csharp
{
    public class CodeReader
    {
        private SyntaxNode root;

        public CodeReader(SyntaxNode syntaxTreeRoot)
        {
            root = syntaxTreeRoot;
        }

        public IEnumerable<VrcProperty> Properties => root.DescendantNodes().OfType<PropertyDeclarationSyntax>().Select(n => VrcProperty.FromSyntax(n));

        public IEnumerable<VrcMethod> Methods => root.DescendantNodes().OfType<MethodDeclarationSyntax>().Select(n => VrcMethod.FromSyntax(n));
    }
}
