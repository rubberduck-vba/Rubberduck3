using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Annotations;

namespace Rubberduck.Core.Editor
{
    public class CodePaneViewModel : ViewModelBase, ICodePaneViewModel
    {
        private string _currentStatus;

        public CodePaneViewModel()
        {
            MemberProviders = new ObservableCollection<IMemberProviderViewModel>(
                new[]
                {
                    new MemberProviderViewModel
                    {
                        Name = "(General)",
                        ModuleType = ModuleType.StandardModule,
                        Members = new ObservableCollection<IMemberInfoViewModel>(new MemberInfoViewModel[]
                        {
                            new MemberInfoViewModel()
                            {
                                Name = "(Declarations)",
                                MemberType = MemberType.None,
                            },
                        })
                    }
                });
        }

        public EditorShellViewModel Shell { get; set; }

        private string _title;
        public string Title 
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _content;
        public string Content 
        {
            get => _content;
            set
            {
                if (_content != value)
                {
                    _content = value;
                    OnPropertyChanged();
                }
            }
        }
        public IEditorSettings EditorSettings { get; set; }

        private IMemberProviderViewModel _selectedProvider;
        public IMemberProviderViewModel SelectedMemberProvider
        {
            get => _selectedProvider;
            set
            {
                if (_selectedProvider != value)
                {
                    _selectedProvider?.ClearMemberSelectedHandlers();
                    _selectedProvider = value;
                    OnPropertyChanged();

                    SelectedMemberProviderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event EventHandler SelectedMemberProviderChanged;

        public ObservableCollection<IMemberProviderViewModel> MemberProviders { get; set; } 

        public string CurrentStatus 
        { 
            get => _currentStatus;
            set
            {
                if (_currentStatus != value)
                {
                    _currentStatus = value;
                    OnPropertyChanged();
                }
            }
        }

        public IModuleInfoViewModel ModuleInfo { get; set; }
        public ObservableCollection<IMemberInfoViewModel> Members { get; }

        public void UpdateStatus(string status)
        {
            Shell.UpdateStatus(status);
        }
    }
}