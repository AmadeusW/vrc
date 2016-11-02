using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Collections.Generic;

namespace vrc.csharp
{
    public class VrcMethod
    {
        public IReadOnlyList<string> Modifiers { get; private set; }
        public string Name { get; private set; }
        public IReadOnlyList<string> Parameters { get; private set; }

        internal static VrcMethod FromSyntax(MethodDeclarationSyntax syntax)
        {
            return new VrcMethod
            {
                Name = syntax.Identifier.ToString(),
                Parameters = syntax.ParameterList.Parameters.Select(n => n.ToString()).ToList(),
                Modifiers = syntax.Modifiers.Select(n => n.ToString()).ToList(),
            };
        }
    }
}