using Rubberduck.Parsing.Model;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command.SyncPanel;
using Rubberduck.VBEditor.ComManagement;
using Rubberduck.VBEditor.Events;
using Rubberduck.VBEditor.SafeComWrappers;

namespace Rubberduck.Core.Editor.Tools
{
    public class SyncPanelModuleViewModelProvider : ISyncPanelModuleViewModelProvider
    {
        private readonly IProjectsProvider _projectsProvider;
        private readonly ILoadCommand _loadCommand;
        private readonly IOpenCommand _openCommand;
        private readonly ISyncCommand _syncCommand;

        public SyncPanelModuleViewModelProvider(IProjectsProvider projectsProvider,
            ILoadCommand loadCommand, IOpenCommand openCommand, ISyncCommand syncCommand)
        {
            _projectsProvider = projectsProvider;
            _loadCommand = loadCommand;
            _openCommand = openCommand;
            _syncCommand = syncCommand;
        }

        public ISyncPanelModuleViewModel Create(ComponentEventArgs info)
        {
            var moduleType = ModuleType.None;
            using (var component = _projectsProvider.Component(info.QualifiedModuleName))
            {
                switch (component.Type)
                {
                    case ComponentType.ComComponent:
                        break;
                    case ComponentType.Undefined:
                        break;
                    case ComponentType.StandardModule:
                        moduleType = ModuleType.StandardModule;
                        break;
                    case ComponentType.ClassModule:
                        moduleType = ModuleType.ClassModule;
                        break;
                    case ComponentType.UserForm:
                        moduleType = ModuleType.UserFormModule;
                        break;
                    case ComponentType.ResFile:
                        break;
                    case ComponentType.VBForm:
                        break;
                    case ComponentType.MDIForm:
                        break;
                    case ComponentType.PropPage:
                        break;
                    case ComponentType.UserControl:
                        break;
                    case ComponentType.DocObject:
                        break;
                    case ComponentType.RelatedDocument:
                        break;
                    case ComponentType.ActiveXDesigner:
                        break;
                    case ComponentType.Document:
                        moduleType = ModuleType.DocumentModule;
                        break;
                }
            }

            return new SyncPanelModuleViewModel(_loadCommand, _openCommand, _syncCommand)
            {
                ModuleType = moduleType,
                QualifiedModuleName = info.QualifiedModuleName,
                State = ModuleSyncState.NotLoaded
            };
        }
    }
}
