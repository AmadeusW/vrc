using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.CodeAnalysis.MSBuild;
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
                Console.WriteLine($"Processing {project.AssemblyName}");
                foreach (var document in project.Documents)
                {
                    Console.WriteLine($"-Document {document.Name}");
                    await ProcessDocument(document);
                }
            }
        }

        private static async Task ProcessDocument(Document document)
        {
            var semantics = await document.GetSemanticModelAsync();
            var syntax = await document.GetSyntaxRootAsync();
            foreach (var typeSyntax in syntax.DescendantNodes().OfType<TypeDeclarationSyntax>())
            {
                Console.WriteLine($"--Type     {typeSyntax.Identifier}");
                var typeSymbol = semantics.GetSymbolInfo(typeSyntax);
                foreach (var methodSyntax in typeSyntax.DescendantNodes().OfType<MethodDeclarationSyntax>())
                {
                    Console.WriteLine($"---Method  {typeSyntax.Identifier}");
                    var methodSymbol = semantics.GetSymbolInfo(methodSyntax);
                }
                foreach (var identifierSyntax in typeSyntax.DescendantNodes().OfType<IdentifierNameSyntax>())
                {
                    Console.WriteLine($"---Identif.{identifierSyntax.Identifier}");
                    var identifierSymbol = semantics.GetSymbolInfo(identifierSyntax);
                }
                foreach (var memberSyntax in typeSyntax.Members)
                {
                    Console.WriteLine($"---Member  {memberSyntax.GetText()}");
                    var memberSymbol = semantics.GetSymbolInfo(memberSyntax);
                }
            }
        }
    }
}
