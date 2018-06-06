using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrc.console
{
    class Program
    {
        static async Task Main(string[] args)
        {
            if (args.Length != 2)
                throw new ArgumentException("Arguments must be: 1. a solution path; 2. output path");
            if (!File.Exists(args[0]))
                throw new ArgumentException("Argument must be a solution path");
            if (!Directory.Exists(Path.GetDirectoryName(args[1])))
                throw new ArgumentException("Argument must be a valid location");
            var solutionPath = args[0];
            var outputPath = args[1];
            await ProcessSolution(solutionPath, outputPath);
        }

        private static async Task ProcessSolution(string solutionPath, string outputPath)
        {
            var workspace = MSBuildWorkspace.Create();
            await workspace.OpenSolutionAsync(solutionPath);
            Console.WriteLine($"Opened {solutionPath}");
            var solution = workspace.CurrentSolution;
            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    await ProcessDocument(document, outputPath);
                }
            }
        }

        private static async Task ProcessDocument(Document document, string outputPath)
        {
            var semantics = await document.GetSemanticModelAsync();
            var filePath = document.FilePath;
            var fileName = Path.GetFileName(filePath);
            var sb = new StringBuilder();

            Console.WriteLine($"Processing {filePath}");
            var syntax = await document.GetSyntaxRootAsync();
            foreach (var typeSyntax in syntax.DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                var typeSymbol = semantics.GetDeclaredSymbol(typeSyntax);
                foreach (var identifierSyntax in typeSyntax.DescendantNodes().OfType<IdentifierNameSyntax>())
                {
                    var symbol = semantics.GetDeclaredSymbol(identifierSyntax);
                    if (symbol == null)
                    {
                        symbol = semantics.GetSymbolInfo(identifierSyntax).Symbol;
                        if (symbol == null)
                        {
                            Console.WriteLine($"Identifier {identifierSyntax.Identifier} has no symbol");
                            continue;
                        }
                    }

                    var lineNumber = syntax.GetText().Lines.GetLineFromPosition(identifierSyntax.Span.Start).LineNumber;
                    if (symbol is IFieldSymbol fieldSymbol)
                    {
                        var p = fieldSymbol.Type;
                        var pa = fieldSymbol.Type.ContainingAssembly?.MetadataName;

                        Log(sb, fileName, lineNumber, symbol.Kind, identifierSyntax.Identifier, p.Name, pa);
                    }
                    else if (symbol is ILocalSymbol localSymbol)
                    {
                        var p = localSymbol.Type;
                        var pa = localSymbol.Type.ContainingAssembly?.MetadataName;

                        Log(sb, fileName, lineNumber, symbol.Kind, identifierSyntax.Identifier, p.Name, pa);
                    }
                    else if (symbol is IMethodSymbol methodSymbol)
                    {
                        var p = methodSymbol.ReturnType;
                        var pa = methodSymbol.ReturnType.ContainingAssembly?.MetadataName;

                        Log(sb, fileName, lineNumber, symbol.Kind, identifierSyntax.Identifier, p.Name, pa);
                    }
                    else if (symbol is ITypeParameterSymbol typeParameterSymbol)
                    {
                        var p = typeParameterSymbol;
                        var pa = typeParameterSymbol.ContainingAssembly?.MetadataName;

                        Log(sb, fileName, lineNumber, symbol.Kind, identifierSyntax.Identifier, p.Name, pa);
                    }
                    else if (symbol is IPropertySymbol propertySymbol)
                    {
                        var p = propertySymbol.Type;
                        var pa = propertySymbol.Type?.ContainingAssembly?.MetadataName;

                        Log(sb, fileName, lineNumber, symbol.Kind, identifierSyntax.Identifier, p.Name, pa);
                    }
                    else if (symbol is INamedTypeSymbol namedType)
                    {
                        var p = namedType;
                        var pa = namedType.ContainingAssembly?.MetadataName;

                        Log(sb, fileName, lineNumber, symbol.Kind, identifierSyntax.Identifier, p.Name, pa);
                    }
                    else if (symbol is IArrayTypeSymbol arraySymbol)
                    {
                        var p = arraySymbol.BaseType;
                        var pa = arraySymbol.BaseType?.ContainingAssembly?.MetadataName;

                        Log(sb, fileName, lineNumber, symbol.Kind, identifierSyntax.Identifier, p.Name, pa);
                    }
                    else
                    {
                        var a = symbol.ContainingAssembly?.MetadataName;
                        // We don't support symbol.GetType()
                        Log(sb, fileName, lineNumber, symbol.Kind, identifierSyntax.Identifier, "", a);
                    }
                }
            }
            File.AppendAllText(outputPath, sb.ToString());
        }

        private static void Log(StringBuilder sb, string fileName, int lineNumber, SymbolKind kind, SyntaxToken identifier, string typeName, string typeAssembly)
        {
            Console.WriteLine($"{fileName}:{lineNumber} - {kind} {identifier} is {typeName} from {typeAssembly}");
            sb.AppendLine($"{fileName},{lineNumber},{kind},{identifier},{typeName},{typeAssembly}");
        }
    }
}
