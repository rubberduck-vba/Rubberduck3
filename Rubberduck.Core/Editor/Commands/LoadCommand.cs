using Rubberduck.Parsing.Model;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command.SyncPanel;
using Rubberduck.VBEditor.ComManagement;
using Rubberduck.VBEditor.SafeComWrappers;
using Rubberduck.VBEditor.SourceCodeHandling;
using System.Collections.Generic;

namespace Rubberduck.Core.Editor.Commands
{
    public class LoadCommand : EditorShellCommand<ISyncPanelModuleViewModel>, ILoadCommand
    {
        private readonly ITempSourceFileHandler _tempFile;
        private readonly IProjectsProvider _projectsProvider;

        public LoadCommand(ITempSourceFileHandler tempFile, IProjectsProvider projectsProvider)
        {
            _tempFile = tempFile;
            _projectsProvider = projectsProvider;

            AddToCanExecuteEvaluation(p => p != null && ((ISyncPanelModuleViewModel)p).ModuleType != Parsing.Model.ModuleType.None);
        }

        private static IDictionary<ComponentType, ModuleType> _moduleTypeByComponentType = new Dictionary<ComponentType, ModuleType>
        {
            [ComponentType.Document] = ModuleType.DocumentModule,
            [ComponentType.StandardModule] = ModuleType.StandardModule,
            [ComponentType.UserForm] = ModuleType.UserFormModule,
            [ComponentType.ClassModule] = ModuleType.ClassModule,
        };

        protected override void ExecuteInternal(IEditorShellViewModel shell, ISyncPanelModuleViewModel param)
        {
            try
            {
                var component = _projectsProvider.Component(param.QualifiedModuleName);
                if (!_moduleTypeByComponentType.TryGetValue(component.Type, out var moduleType))
                {
                    // component type isn't supported. throw?
                    return;
                }

                var content = _tempFile.Read(component);

                var vm = MemberProviderViewModel.Default(param.QualifiedModuleName, moduleType);
                shell.LoadModule(param.QualifiedModuleName, content, vm);
                shell.ActivateModuleDocumentTab(param.QualifiedModuleName);

                param.State = ModuleSyncState.OK;
            }
            catch
            {
                // TODO log error
                param.State = ModuleSyncState.LoadError;
            }
        }
    }
}
