using Rubberduck.Parsing.Model;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command.SyncPanel;
using Rubberduck.VBEditor.ComManagement;
using Rubberduck.VBEditor.SourceCodeHandling;
using System;
using System.Linq;

namespace Rubberduck.Core.Editor.Commands
{
    public class SyncCommand : EditorShellCommand<ISyncPanelModuleViewModel>, ISyncCommand
    {
        private readonly IProjectsProvider _projectsProvider;
        private readonly IComponentSourceCodeHandler _sourceCodeHandler;

        public SyncCommand(IProjectsProvider projectsProvider, IComponentSourceCodeHandler sourceCodeHandler)
        {
            _projectsProvider = projectsProvider;
            _sourceCodeHandler = sourceCodeHandler;

            AddToCanExecuteEvaluation(param => 
            // TODO rephrase this abomination
                param is ISyncPanelModuleViewModel moduleVM 
                    ? EditorShellContext.Current.Shell.ModuleDocumentTabs.Any(tab =>
                        tab.ModuleInfo.QualifiedModuleName.Equals(((ISyncPanelModuleViewModel)param).QualifiedModuleName))
                    : param is ISyncPanelViewModel vm ? vm.VBIDEModules.Any(m => m.SyncCommand.CanExecute(m)) : false);
        }

        protected override void ExecuteInternal(IEditorShellViewModel shell, ISyncPanelModuleViewModel param)
        {
            if (param.ModuleType == ModuleType.DocumentModule)
            {
                // TODO prompt user for confirmation about losing attributes, if applicable
            }

            var component = _projectsProvider.Component(param.QualifiedModuleName);
            using (var collection = component.Collection)
            {
                var vm = shell.ModuleDocumentTabs.SingleOrDefault(e => e.ModuleInfo.QualifiedModuleName.Equals(param.QualifiedModuleName));
                if (vm != null)
                {
                    try
                    {
                        collection.DetachEvents();
                        _sourceCodeHandler.SubstituteCode(component, vm.Content);
                    }
                    catch (Exception exception)
                    {
                        param.State = ModuleSyncState.SyncError;
                        // TODO log exception details
                        Console.Error.WriteLine(exception.ToString());
                    }
                    finally
                    {
                        collection.AttachEvents();
                    }
                }
                else
                {
                    // document tab not found?
                }
            }
        }
    }
}
