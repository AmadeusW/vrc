using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrc.csharp.tests.Helpers
{
    class TestData
    {
        public Document Document { get; private set; }
        public SyntaxTree SyntaxTree { get; private set; }
        public SemanticModel SemanticModel { get; private set; }

        public Compilation Compilation => this.SemanticModel.Compilation;
        public Project Project => this.Document.Project;
        public SyntaxNode SyntaxTreeRoot => this.SyntaxTree.GetRoot();

        private TestData()
        {
        }

        internal static async Task<TestData> FromCode(string sourceCode)
        {
            var project = buildProjectForSourceCode(sourceCode);
            var compilation = await project.GetCompilationAsync();
            var document = project.Documents.First();
            var syntaxTree = await document.GetSyntaxTreeAsync();
            var semanticModel = compilation.GetSemanticModel(syntaxTree);

            return new TestData()
            {
                Document = document,
                SyntaxTree = syntaxTree,
                SemanticModel = semanticModel,
            };
        }

        private static Project buildProjectForSourceCode(string sourceCode)
        {
            var workspace = new AdhocWorkspace();
            var references = GetReferences();
            var projectId = ProjectId.CreateNewId();
            var versionStamp = VersionStamp.Create();
            var options = new CSharpCompilationOptions(OutputKind.DynamicallyLinkedLibrary);

            var projectInfo = ProjectInfo.Create(projectId, versionStamp, "TestProject", "TestProject", LanguageNames.CSharp, metadataReferences: references, compilationOptions: options);
            workspace.AddProject(projectInfo);
            var sourceText = SourceText.From(sourceCode);

            var document = workspace.AddDocument(projectId, "TestDocument", sourceText);
            return document.Project;
        }

        private static List<MetadataReference> GetReferences()
        {
            var assemblyPath = Path.GetDirectoryName(typeof(object).Assembly.Location);
            var references = new List<MetadataReference>();
            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "mscorlib.dll")));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.dll")));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Core.dll")));
            references.Add(MetadataReference.CreateFromFile(Path.Combine(assemblyPath, "System.Runtime.dll")));
            return references;
        }
    }
}
