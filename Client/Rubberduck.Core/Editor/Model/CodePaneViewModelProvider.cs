using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Abstract;
using Rubberduck.VBEditor.ComManagement;
using System.Linq;

namespace Rubberduck.Core.Editor
{
    public interface ICodePaneViewModelProvider
    {
        ICodePaneViewModel GetViewModel(IEditorShellViewModel shell, IQualifiedModuleName module, IMemberProviderViewModel memberProvider, string content);
    }

    public class CodePaneViewModelProvider : ICodePaneViewModelProvider
    {
        private readonly IProjectsRepository _projectsRepository;
        //private readonly ICodeParserService _parserService;
        private readonly IEditorSettings _editorSettings;

        public CodePaneViewModelProvider(IProjectsRepository projectsRepository, /*ICodeParserService parserService,*/ IEditorSettings editorSettings)
        {
            _projectsRepository = projectsRepository;
            //_parserService = parserService;
            _editorSettings = editorSettings;
        }

        public ICodePaneViewModel GetViewModel(IEditorShellViewModel shell, IQualifiedModuleName module, IMemberProviderViewModel memberProvider, string content)
        {
            var vm = shell.ModuleDocumentTabs.SingleOrDefault(e => e.ModuleInfo.QualifiedModuleName.Equals(module));
            if (vm is null)
            {
                _projectsRepository.Component(module);

                var info = new ModuleInfoViewModel
                {
                    Name = module.ComponentName,
                    EditorPosition = Selection.Home,
                    QualifiedModuleName = module,
                    ModuleType = memberProvider.ModuleType
                };

                var memberProviders = new[]
                {
                    memberProvider,
                    // TODO add implemented interfaces, WithEvents variables, etc.
                };

                vm = new CodePaneViewModel(/*_parserService,*/ _editorSettings, memberProviders)
                {
                    Content = content,
                    ModuleInfo = info,
                };
            }

            return vm;
        }
    }
}