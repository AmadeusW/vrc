using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Text.Projection;

namespace vrc.visualstudio.Extension
{
    internal class CurrentFileHelper
    {
        IVsTextManager textManager;

        public CurrentFileHelper(IVsTextManager textManager)
        {
            this.textManager = textManager;
        }

        /// <summary>
        /// Gets the active IWpfTextView, if one exists.
        /// </summary>
        /// <returns>The active IWpfTextView, or null if no such IWpfTextView exists.</returns>
        public ITextBuffer GetActiveTextBuffer()
        {
            IWpfTextView view = null;
            IVsTextView vTextView = null;

            textManager.GetActiveView(
                fMustHaveFocus: 1
                , pBuffer: null
                , ppView: out vTextView);

            IVsUserData userData = vTextView as IVsUserData;
            if (null != userData)
            {
                IWpfTextViewHost viewHost;
                object holder;
                Guid guidViewHost = DefGuidList.guidIWpfTextViewHost;
                userData.GetData(ref guidViewHost, out holder);
                viewHost = (IWpfTextViewHost)holder;
                view = viewHost.TextView;
            }
            return view.TextBuffer;
        }
    }
}
