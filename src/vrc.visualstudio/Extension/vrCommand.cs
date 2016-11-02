//------------------------------------------------------------------------------
// <copyright file="vrCommand.cs" company="Company">
//     Copyright (c) Company.  All rights reserved.
// </copyright>
//------------------------------------------------------------------------------

using System;
using System.ComponentModel.Design;
using System.Globalization;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;

namespace vrc.visualstudio.Extension 
{
    /// <summary>
    /// Command handler
    /// </summary>
    internal sealed class vrCommand
    {
        public const int CommandId = 0x0100;
        public static readonly Guid CommandSet = new Guid("6a74ba61-f116-4089-ac8e-7ed12048940d");
        private readonly Package package;
        private readonly CurrentFileHelper fileHelper;
        private readonly WorkspaceService workspaceService;

        /// <summary>
        /// Initializes a new instance of the <see cref="vrCommand"/> class.
        /// Adds our command handlers for menu (commands must exist in the command table file)
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        private vrCommand(Package package, WorkspaceService workspaceService, CurrentFileHelper fileHelper)
        {
            if (package == null)
            {
                throw new ArgumentNullException("package");
            }
            if (workspaceService == null)
            {
                throw new ArgumentNullException(nameof(workspaceService));
            }
            if (fileHelper == null)
            {
                throw new ArgumentNullException(nameof(fileHelper));
            }

            this.package = package;
            this.workspaceService = workspaceService;
            this.fileHelper = fileHelper;

            OleMenuCommandService commandService = this.ServiceProvider.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
            if (commandService != null)
            {
                var menuCommandID = new CommandID(CommandSet, CommandId);
                var menuItem = new MenuCommand(this.MenuItemCallback, menuCommandID);
                commandService.AddCommand(menuItem);
            }
        }

        /// <summary>
        /// Gets the instance of the command.
        /// </summary>
        public static vrCommand Instance
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the service provider from the owner package.
        /// </summary>
        private IServiceProvider ServiceProvider
        {
            get
            {
                return this.package;
            }
        }

        /// <summary>
        /// Initializes the singleton instance of the command.
        /// </summary>
        /// <param name="package">Owner package, not null.</param>
        public static void Initialize(Package package, WorkspaceService workspaceService, CurrentFileHelper fileHelper)
        {
            Instance = new vrCommand(package, workspaceService, fileHelper);
        }

        /// <summary>
        /// This function is the callback used to execute the command when the menu item is clicked.
        /// See the constructor to see how the menu item is associated with this function using
        /// OleMenuCommandService service and MenuCommand class.
        /// </summary>
        /// <param name="sender">Event sender.</param>
        /// <param name="e">Event args.</param>
        private async void MenuItemCallback(object sender, EventArgs e)
        {
            string message = string.Format(CultureInfo.CurrentCulture, "Inside {0}.MenuItemCallback()", this.GetType().FullName);
            string title = "vrCommand";

            var currentBuffer = fileHelper.GetActiveTextBuffer();
            var core = new Core(workspaceService, currentBuffer);
            await core.Sample();

            // Show a message box to prove we were here
            VsShellUtilities.ShowMessageBox(
                this.ServiceProvider,
                message,
                title,
                OLEMSGICON.OLEMSGICON_INFO,
                OLEMSGBUTTON.OLEMSGBUTTON_OK,
                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST);
        }
    }
}
