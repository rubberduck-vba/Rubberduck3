using ICSharpCode.AvalonEdit.Document;
using Rubberduck.Core.Editor.Tools;
using Rubberduck.Parsing;
using Rubberduck.Settings;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.VBEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;

namespace Rubberduck.Core.Editor
{
    public class EditorShellViewModel : ViewModelBase, IEditorShellViewModel
    {
        public EditorShellViewModel(IStatusBarViewModel status, IShellToolTabProvider toolTabsProvider)
        {
            Status = status;

            var tabs = toolTabsProvider.GetShellToolTabs();
            ToolTabs = new ObservableCollection<IShellToolTab>(tabs);
        }

        public TextDocument GetDocument(QualifiedModuleName module)
        {
            return DocumentTabs.FirstOrDefault(e => e.ModuleInfo.QualifiedModuleName.Equals(module))?.Document;
        }

        private IEnumerable<ICodePaneViewModel> DocumentTabs => LoadedModules.Where(e => e.IsTabOpen);

        private ICodePaneViewModel _selectedDocumentTab;
        public ICodePaneViewModel SelectedDocumentTab
        {
            get => _selectedDocumentTab;
            set
            {
                if (value != _selectedDocumentTab)
                {
                    _selectedDocumentTab = value;
                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ICodePaneViewModel> LoadedModules { get; } = new ObservableCollection<ICodePaneViewModel>();
        public IEnumerable<ISyntaxErrorViewModel> SyntaxErrors => LoadedModules.SelectMany(e => e.SyntaxErrors);

        public IStatusBarViewModel Status { get; }

        public ObservableCollection<IShellToolTab> ToolTabs { get; }
    }
}