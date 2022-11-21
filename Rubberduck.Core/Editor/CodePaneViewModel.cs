using ICSharpCode.AvalonEdit.Document;
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
    public class MemberProviderViewModel : ViewModelBase, IMemberProviderViewModel
    {
        private string _name;
        public string Name 
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private ModuleType _moduleType;
        public ModuleType ModuleType 
        {
            get => _moduleType;
            set
            {
                if (_moduleType != value)
                {
                    _moduleType = value;
                    OnPropertyChanged();
                }
            }
        }
        public ObservableCollection<IMemberInfoViewModel> Members { get; set; } = new ObservableCollection<IMemberInfoViewModel>();

        private IMemberInfoViewModel _currentMember;

        public event EventHandler<NavigateToMemberEventArgs> MemberSelected;

        public IMemberInfoViewModel CurrentMember
        {
            get => _currentMember;
            set
            {
                if (_currentMember != value)
                {
                    _currentMember = value;
                    OnPropertyChanged();

                    MemberSelected?.Invoke(this, new NavigateToMemberEventArgs(_currentMember));
                }
            }
        }

        public void SetCurrentMember(int line)
        {
            var member = Members.SingleOrDefault(e => e.StartLine >= line && e.EndLine <= line);
            _currentMember = member;
            OnPropertyChanged(nameof(CurrentMember));
        }

        public void AddMember(string name, MemberType memberType, (ITextAnchor start, ITextAnchor end) anchors)
        {
            var vm = new MemberInfoViewModel(anchors.start, anchors.end)
            {
                Name = name,
                MemberType = memberType,
                HasImplementation = true,
            };
            Members.Add(vm);
            CurrentMember = vm;
        }
    }

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

            SelectedMemberProvider = MemberProviders.FirstOrDefault();
            SelectedMemberProvider.CurrentMember = SelectedMemberProvider.Members.FirstOrDefault();
        }

        public EditorShellViewModel Shell { get; set; }

        public string Title { get; set; }

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
                    _selectedProvider = value;
                    OnPropertyChanged();
                }
            }
        }

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