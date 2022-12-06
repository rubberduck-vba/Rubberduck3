using Rubberduck.Parsing;
using Rubberduck.Parsing.Abstract;
using Rubberduck.UI.Abstract;
using Rubberduck.VBEditor;
using System.Collections.Generic;

namespace Rubberduck.Core.Editor
{
    public class CodePaneViewModelProvider : ICodePaneViewModelProvider
    {
        private readonly ICodeParserService _parserService;
        private readonly IEditorSettings _editorSettings;

        //private readonly IDictionary<QualifiedModuleName, ICodePaneViewModel> _cache = new Dictionary<QualifiedModuleName, ICodePaneViewModel>();

        public CodePaneViewModelProvider(ICodeParserService parserService, IEditorSettings editorSettings)
        {
            _parserService = parserService;
            _editorSettings = editorSettings;
        }

        public ICodePaneViewModel GetViewModel(IEditorShellViewModel shell, QualifiedModuleName module, string content)
        {
            //if (_cache.TryGetValue(module, out var viewModel))
            //{
            //    return viewModel;
            //}

            var vm = new CodePaneViewModel(_parserService, _editorSettings)
            {
                Shell = shell,
                Title = module.Name,
                Content = content,
            };
            //_cache.Add(module, vm);
            return vm;
        }
    }
}