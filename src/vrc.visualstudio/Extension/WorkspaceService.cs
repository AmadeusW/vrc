using Microsoft.CodeAnalysis;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.LanguageServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace vrc.visualstudio.Extension
{
    public class WorkspaceService
    {
        VisualStudioWorkspace workspace;

        public WorkspaceService(IComponentModel componentModel)
        {
            workspace = componentModel.GetService<VisualStudioWorkspace>();
        }

        public Document GetDocument(string filePath)
        {
            var documentId = workspace.CurrentSolution.GetDocumentIdsWithFilePath(filePath).Single();
            return workspace.CurrentSolution.GetDocument(documentId);
        }

        internal Document GetDocument(ITextBuffer buffer)
        {
            var document = buffer.CurrentSnapshot.GetOpenDocumentInCurrentContextWithChanges();
            return document;

        }

        internal async Task UpdateSolution(Solution solution)
        {
            if (!workspace.TryApplyChanges(solution))
            {
                throw new InvalidOperationException("Error applying changes");
            }
        }
    }
}
