using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.Text;
using Microsoft.VisualStudio.ComponentModelHost;
using Microsoft.VisualStudio.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using vrc.csharp;
using vrc.visualstudio.Extension;

namespace vrc.visualstudio
{
    public class Core
    {
        private ITextBuffer currentBuffer;
        private WorkspaceService workspaceService;

        public Core(WorkspaceService workspaceService, ITextBuffer currentBuffer)
        {
            this.workspaceService = workspaceService;
            this.currentBuffer = currentBuffer;
        }

        internal Task Sample()
        {
            var reader = new CodeReader(workspaceService.GetDocument(currentBuffer));

            throw new NotImplementedException();
        }
    }
}
