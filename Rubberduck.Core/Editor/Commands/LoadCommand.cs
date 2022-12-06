using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command.SyncPanel;
using Rubberduck.VBEditor.ComManagement;
using Rubberduck.VBEditor.SourceCodeHandling;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using static Rubberduck.UI.WPF.Converters.DeclarationToMemberSignatureConverter;

namespace Rubberduck.Core.Editor.Commands
{
    public class LoadCommand : EditorShellCommand<ISyncPanelModuleViewModel>, ILoadCommand
    {
        private readonly ITempSourceFileHandler _tempFile;
        private readonly IProjectsProvider _projectsProvider;
        private readonly ICodePaneViewModelProvider _vmProvider;

        public LoadCommand(ITempSourceFileHandler tempFile, IProjectsProvider projectsProvider, ICodePaneViewModelProvider vmProvider)
        {
            _tempFile = tempFile;
            _projectsProvider = projectsProvider;
            _vmProvider = vmProvider;
            
            AddToCanExecuteEvaluation(p => ((ISyncPanelModuleViewModel)p).ModuleType != Parsing.Model.ModuleType.None);
        }

        protected override void ExecuteInternal(IEditorShellViewModel shell, ISyncPanelModuleViewModel param)
        {
            try
            {
                var component = _projectsProvider.Component(param.QualifiedModuleName);
                var content = _tempFile.Read(component);

                var vm = _vmProvider.GetViewModel(shell, component.QualifiedModuleName, content);
                shell.LoadedModules.Add(vm);

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
