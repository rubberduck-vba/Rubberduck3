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
using System.Collections.Specialized;
using System.Linq;
using System.Threading;

namespace Rubberduck.Core.Editor
{
    public class EditorShellViewModel : ViewModelBase, IEditorShellViewModel, INotifyCollectionChanged
    {
        private readonly IEditorShellContext _context;
        private readonly ICodePaneViewModelProvider _vmProvider;
        private readonly IDictionary<QualifiedModuleName, ICodePaneViewModel> _modules = new Dictionary<QualifiedModuleName, ICodePaneViewModel>();

        public EditorShellViewModel(IStatusBarViewModel status, ICodePaneViewModelProvider vmProvider, IShellToolTabProvider toolTabsProvider)
        {
            Status = status;

            var tabs = toolTabsProvider.GetShellToolTabs();
            ToolTabs = new ObservableCollection<IShellToolTab>(tabs);
            SelectedToolTab = ToolTabs.FirstOrDefault();

            _vmProvider = vmProvider;
            _context = new EditorShellContext(this);
        }

        public TextDocument GetDocument(QualifiedModuleName module)
        {
            if (_modules.TryGetValue(module, out var vm))
            {
                return vm.Document;
            }

            return null;
        }

        private ICodePaneViewModel _selectedDocumentTab;

        public ICodePaneViewModel SelectedModuleDocumentTab
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

        private IShellToolTab _selectedToolTab;

        public IShellToolTab SelectedToolTab
        {
            get => _selectedToolTab;
            set
            {
                if (value != _selectedToolTab)
                {
                    _selectedToolTab = value;
                    OnPropertyChanged();
                }
            }
        }

        public bool LoadModule(QualifiedModuleName module, string content, IMemberProviderViewModel memberProvider)
        {
            if (_modules.ContainsKey(module))
            {
                return false;
            }

            var vm = _vmProvider.GetViewModel(this, module, memberProvider, content);
            _modules.Add(module, vm);
            ModuleDocumentTabs.Add(vm);
            OnPropertyChanged(nameof(ModuleDocumentTabs));
            OnCollectionChanged(NotifyCollectionChangedAction.Add);

            Status.ShowDocumentStatusItems = _modules.Any();
            return true;
        }

        public bool UnloadModule(QualifiedModuleName module)
        {
            if (!_modules.TryGetValue(module, out var vm))
            {
                return false;
            }

            _modules.Remove(module);
            ModuleDocumentTabs.Remove(vm);
            OnPropertyChanged(nameof(ModuleDocumentTabs));
            OnCollectionChanged(NotifyCollectionChangedAction.Remove);

            Status.ShowDocumentStatusItems = _modules.Any();
            return true;
        }

        public ICodePaneViewModel GetModule(QualifiedModuleName module)
        {
            if (_modules.TryGetValue(module, out var vm))
            {
                return vm;
            }

            return null;
        }

        public void ActivateModuleDocumentTab(QualifiedModuleName module)
        {
            if (_modules.TryGetValue(module, out var vm))
            {
                SelectedModuleDocumentTab = vm;
            }
        }

        public event NotifyCollectionChangedEventHandler CollectionChanged;
        public ObservableCollection<ICodePaneViewModel> ModuleDocumentTabs { get; } = new ObservableCollection<ICodePaneViewModel>();

        private void OnCollectionChanged(NotifyCollectionChangedAction action)
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(action));
        }

        public IEnumerable<ISyntaxErrorViewModel> SyntaxErrors => _modules.Values.SelectMany(e => e.SyntaxErrors);

        public IStatusBarViewModel Status { get; }

        public IEnumerable<IShellToolTab> ToolTabs { get; }
    }
}