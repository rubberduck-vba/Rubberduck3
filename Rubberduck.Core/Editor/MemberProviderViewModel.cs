using ICSharpCode.AvalonEdit.Document;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

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

        public void ClearMemberSelectedHandlers()
        {
            MemberSelected = null;
        }

        public void AddMember(string name, MemberType memberType, int startOffset, int endOffset)
        {
            var vm = Members.SingleOrDefault(m => m.HasImplementation && m.MemberType == memberType 
                && (m.StartOffset == startOffset || string.Equals(m.Name, name, StringComparison.InvariantCultureIgnoreCase)));

            if (vm != null)
            {
                vm.Name = name;
                vm.StartOffset = startOffset;
                vm.EndOffset = endOffset;
            }
            else
            {
                vm = new MemberInfoViewModel
                {
                    Name = name,
                    MemberType = memberType,
                    HasImplementation = true,
                    IsUserDefined = true,
                    StartOffset = startOffset,
                    EndOffset = endOffset
                };
                Members.Add(vm);
            }

            CurrentMember = vm;
        }
    }
}