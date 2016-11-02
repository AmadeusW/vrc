using System;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Linq;
using System.Collections.Generic;

namespace vrc.csharp
{
    public class VrcProperty
    {
        public IReadOnlyList<string> Accessors { get; private set; }
        public IReadOnlyList<string> Modifiers { get; private set; }
        public string Name { get; private set; }

        internal static VrcProperty FromSyntax(PropertyDeclarationSyntax syntax)
        {
            return new VrcProperty
            {
                Name = syntax.Identifier.ToString(),
                Accessors = syntax.AccessorList.Accessors.Select(n => n.ToString()).ToList(),
                Modifiers = syntax.Modifiers.Select(n => n.ToString()).ToList(),
            };
        }
    }
}