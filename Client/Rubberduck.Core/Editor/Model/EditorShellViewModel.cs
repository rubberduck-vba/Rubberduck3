using Dragablz;
using ICSharpCode.AvalonEdit.Document;
using Rubberduck.InternalApi.Model;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.Unmanaged.Model.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;

namespace Rubberduck.Core.Editor
{
    class EditorShellViewModel : ViewModelBase, IEditorShellViewModel, INotifyCollectionChanged
    {
        private readonly IDictionary<IQualifiedModuleName, ICodePaneViewModel> _modules = new Dictionary<IQualifiedModuleName, ICodePaneViewModel>();

        public EditorShellViewModel(IInterTabClient interTabClient, IStatusBarViewModel status, IShellToolTabProvider toolTabsProvider)
        {
            Status = status;

            var tabs = toolTabsProvider.GetShellToolTabs();
            ToolTabs = new ObservableCollection<IShellToolTab>(tabs);
            SelectedToolTab = ToolTabs.FirstOrDefault()!;

            InterTabClient = interTabClient;
            Partition = _staticPartition;
        }

        private static readonly string _staticPartition = Guid.NewGuid().ToString();

        public object Partition { get; init; }
        public object InterTabClient { get; init; }

        public TextDocument GetDocument(IQualifiedModuleName module)
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

        public bool LoadModule(IQualifiedModuleName module, string content, IMemberProviderViewModel memberProvider)
        {
            if (_modules.ContainsKey(module))
            {
                return false;
            }

            //var vm = _vmProvider.GetViewModel(this, module, memberProvider, content);
            //_modules.Add(module, vm);
            //ModuleDocumentTabs.Add(vm);
            OnPropertyChanged(nameof(ModuleDocumentTabs));
            OnCollectionChanged(NotifyCollectionChangedAction.Add);

            Status.ShowDocumentStatusItems = _modules.Any();
            return true;
        }

        public bool UnloadModule(IQualifiedModuleName module)
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

        public ICodePaneViewModel GetModule(IQualifiedModuleName module)
        {
            if (_modules.TryGetValue(module, out var vm))
            {
                return vm;
            }

            return null;
        }

        public void ActivateModuleDocumentTab(IQualifiedModuleName module)
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