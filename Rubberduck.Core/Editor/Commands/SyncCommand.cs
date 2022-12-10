using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command.SyncPanel;
using Rubberduck.VBEditor;
using Rubberduck.VBEditor.ComManagement;
using Rubberduck.VBEditor.SourceCodeHandling;
using System;
using System.Linq;

namespace Rubberduck.Core.Editor.Commands
{
    public class SyncCommand : EditorShellCommand<ISyncPanelModuleViewModel>, ISyncCommand
    {
        private readonly IProjectsProvider _projectsProvider;
        private readonly IComponentSourceCodeHandler _source;
        public SyncCommand(IProjectsProvider projectsProvider, IComponentSourceCodeHandler source)
        {
            _projectsProvider = projectsProvider;
            _source = source;
        }

        protected override void ExecuteInternal(IEditorShellViewModel shell, ISyncPanelModuleViewModel param)
        {
            var component = _projectsProvider.Component(param.QualifiedModuleName);
            using (var collection = component.Collection)
            {
                var vm = shell.ModuleDocumentTabs.SingleOrDefault(e => e.ModuleInfo.QualifiedModuleName.Equals(param.QualifiedModuleName));
                if (vm != null)
                {
                    try
                    {
                        collection.DetachEvents();
                        _source.SubstituteCode(component, vm.Content);
                    }
                    finally
                    {
                        collection.AttachEvents();
                    }
                }
            }
        }
    }
}
