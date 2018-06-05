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
            if (args.Length == 0)
                throw new ArgumentException("Argument must be a solution path");
            if (!File.Exists(args[0]))
                throw new ArgumentException("Argument must be a solution path");
            var solutionPath = args[0];
            await ProcessSolution(solutionPath);
        }

        private static async Task ProcessSolution(string solutionPath)
        {
            var workspace = MSBuildWorkspace.Create();
            await workspace.OpenSolutionAsync(solutionPath);
            Console.WriteLine($"Opened {solutionPath}");
            var solution = workspace.CurrentSolution;
            foreach (var project in solution.Projects)
            {
                foreach (var document in project.Documents)
                {
                    await ProcessDocument(document);
                }
            }
        }

        private static async Task ProcessDocument(Document document)
        {
            var semantics = await document.GetSemanticModelAsync();
            var filePath = document.FilePath;
            var fileName = Path.GetFileName(filePath);
            Console.WriteLine($"Processing {filePath}");
            var syntax = await document.GetSyntaxRootAsync();
            foreach (var typeSyntax in syntax.DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                var typeSymbol = semantics.GetDeclaredSymbol(typeSyntax);

                // TODO: Find all types used in the document.
                // this includes identifiers and return types
                // see which assembly they are from
                /*
                foreach (var methodSyntax in typeSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    Console.WriteLine($"---Method  {typeSyntax.Identifier}");
                    var methodSymbol = semantics.GetDeclaredSymbol(methodSyntax);
                }
                */
                foreach (var identifierSyntax in typeSyntax.DescendantNodes().OfType<IdentifierNameSyntax>())
                {
                    var symbol = semantics.GetDeclaredSymbol(identifierSyntax);
                    if (symbol == null)
                    {
                        symbol = semantics.GetSymbolInfo(identifierSyntax).Symbol;
                        if (symbol == null)
                        {
                            Console.WriteLine($"---Identif.{identifierSyntax.Identifier} has no symbol");
                            continue;
                        }
                    }

                    //var declaringLine = TextLine.FromSpan(syntax.GetText(), identifierSyntax.Span);
                    var a = symbol.ContainingAssembly.MetadataName;
                    if (symbol is IFieldSymbol fieldSymbol)
                    {
                        var p = fieldSymbol.Type;
                        var pa = fieldSymbol.Type.ContainingAssembly.MetadataName;

                        //Console.WriteLine($"{fileName}:{declaringLine.LineNumber} - {identifierSyntax.Identifier} from {a}");
                        Console.WriteLine($"{fileName} - {symbol.Kind} {identifierSyntax.Identifier} is {p.Name} from {pa}");
                    }
                    else if (symbol is ILocalSymbol localSymbol)
                    {
                        var p = localSymbol.Type;
                        var pa = localSymbol.Type.ContainingAssembly.MetadataName;

                        //Console.WriteLine($"{fileName}:{declaringLine.LineNumber} - {identifierSyntax.Identifier} from {a}");
                        Console.WriteLine($"{fileName} - {symbol.Kind} {identifierSyntax.Identifier} is {p.Name} from {pa}");
                    }
                    else
                    {
                        //Console.WriteLine($"{fileName}:{declaringLine.LineNumber} - {identifierSyntax.Identifier} from {a}");
                        Console.WriteLine($"{fileName} - {symbol.Kind} {identifierSyntax.Identifier} from {a}");
                    }
                }
            }
        }
    }
}
