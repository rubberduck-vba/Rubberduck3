using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command.SyncPanel;
using System;
using System.Linq;

namespace Rubberduck.Core.Editor.Commands
{
    public class OpenCommand : EditorShellCommand<ISyncPanelModuleViewModel>, IOpenCommand
    {
        public OpenCommand() 
        {
            AddToCanExecuteEvaluation(p => ((ISyncPanelModuleViewModel)p).ModuleType != Parsing.Model.ModuleType.None);
        }

        protected override void ExecuteInternal(IEditorShellViewModel shell, ISyncPanelModuleViewModel param)
        {
            var vm = shell.ModuleDocumentTabs.SingleOrDefault(e => e.ModuleInfo.QualifiedModuleName.Equals(param.QualifiedModuleName));
            if (vm != null)
            {
                shell.ActivateModuleDocumentTab(param.QualifiedModuleName);
            }
        }
    }
}
