using ICSharpCode.AvalonEdit.Document;
using Rubberduck.Parsing;
using Rubberduck.Parsing.Model;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.VBEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Rubberduck.Core.Editor
{
    public class MemberProviderViewModel : ViewModelBase, IMemberProviderViewModel
    {
        public static MemberProviderViewModel Default(QualifiedModuleName module, ModuleType moduleType)
        {
            return new MemberProviderViewModel
            {
                Name = "(General)",
                QualifiedModuleName = module,
                ModuleType = moduleType,
                Members = new ObservableCollection<IMemberInfoViewModel>(
                    new MemberInfoViewModel[]
                    {
                        new MemberInfoViewModel
                        {
                            Name = "(Declarations)",
                            MemberType = MemberType.None,
                            IsUserDefined = false,
                            HasImplementation = false,
                            
                        },
                    })
            };
        }

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

        private QualifiedModuleName _qualifiedModuleName;
        public QualifiedModuleName QualifiedModuleName
        {
            get => _qualifiedModuleName;
            set
            {
                if (!_qualifiedModuleName.Equals(value))
                {
                    _qualifiedModuleName = value;
                    OnPropertyChanged();
                }
            }
        }

        private ICollection<IMemberInfoViewModel> _members = new List<IMemberInfoViewModel>();
        public ICollection<IMemberInfoViewModel> Members 
        {
            get => _members;
            set
            {
                if (_members != value)
                {
                    _members = value;
                    OnPropertyChanged();
                }
            }
        } 

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

        public void AddMember(string name, MemberType memberType, DocumentOffset offset)
        {
            var vm = Members.SingleOrDefault(m => m.HasImplementation && m.MemberType == memberType 
                && (m.Offset.Start == offset.Start || string.Equals(m.Name, name, StringComparison.InvariantCultureIgnoreCase)));

            if (vm != null)
            {
                vm.Name = name;
                vm.Offset = offset;
            }
            else
            {
                vm = new MemberInfoViewModel
                {
                    Name = name,
                    MemberType = memberType,
                    HasImplementation = true,
                    IsUserDefined = true,
                    Offset = offset
                };
                Members.Add(vm);
            }

            CurrentMember = vm;
        }
    }
}