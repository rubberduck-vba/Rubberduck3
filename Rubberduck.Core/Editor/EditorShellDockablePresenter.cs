using Rubberduck.InternalApi.Model;
using Rubberduck.Parsing.Model;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.WinForms;
using Rubberduck.VBEditor;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Rubberduck.Core.Editor
{
    public class ModuleInfoViewModel : ViewModelBase, IModuleInfoViewModel
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

        private string _folder;
        public string Folder
        {
            get => _folder;
            set
            {
                if (_folder != value)
                {
                    _folder = value;
                    OnPropertyChanged();
                }
            }
        }

        private Selection _position;
        public Selection EditorPosition
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    OnPropertyChanged();
                }
            }
        }
    }
    public class MemberInfoViewModel : ViewModelBase, IMemberInfoViewModel, IEquatable<MemberInfoViewModel>
    {
        private static readonly IDictionary<string, string> _displayNamesByMemberType =
            Enum.GetValues(typeof(MemberType)).Cast<MemberType>().ToDictionary(m => m.ToString(), m => m.GetType().GetCustomAttribute<DisplayAttribute>()?.Name);

        public MemberInfoViewModel(DocumentOffset offset) : this()
        {
            _offset = offset;
        }

        public MemberInfoViewModel()
        {
            Parameters.CollectionChanged += OnParametersCollectionChanged;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, _offset.Start);
        }

        public override bool Equals(object obj)
        {
            if (obj is null)
            {
                return false;
            }

            if (ReferenceEquals(this, obj))
            {
                return true;
            }

            if (obj.GetType() != GetType())
            {
                return false;
            }

            return Equals((MemberInfoViewModel)obj);
        }

        private void OnParametersCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            Console.WriteLine("OnParametersCollectionChanged:TODO");
        }

        public bool Equals(MemberInfoViewModel other)
        {
            if (other is null)
            {
                return false;
            }

            return other.Name == Name && other.Offset.Start == Offset.Start;
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
                    OnPropertyChanged(nameof(Signature));
                }
            }
        }

        public bool IsUserDefined { get; set; }

        private DocumentOffset _offset;
        public DocumentOffset Offset
        {
            get => _offset;
            set
            {
                if (_offset.Equals(value))
                {
                    _offset = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _startLine;
        public int StartLine 
        {
            get => _startLine;
            set
            {
                if (_startLine != value)
                {
                    _startLine = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _endLine;
        public int EndLine 
        {
            get => _endLine;
            set
            {
                if (_endLine != value)
                {
                    _endLine = value;
                    OnPropertyChanged();
                }
            }
        }

        private IParameterInfoViewModel _currentParameter;
        public IParameterInfoViewModel CurrentParameter
        {
            get => _currentParameter;
            set
            {
                if (_currentParameter != value)
                {
                    _currentParameter = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Signature));
                }
            }
        }

        public string Signature => MemberType == MemberType.None
            ? _name
            : $"{_displayNamesByMemberType[_memberType.ToString()]} {_name}({string.Join(", ", Parameters.Select(p => $"{p.IsOptional} {p.Name} As {p.AsType}"))})";


        private bool _hasImplementation;
        public bool HasImplementation 
        {
            get => _hasImplementation;
            set
            {
                if (_hasImplementation != value)
                {
                    _hasImplementation = value;
                    OnPropertyChanged();
                }
            }
        }

        private MemberType _memberType;
        public MemberType MemberType 
        {
            get => _memberType;
            set
            {
                if (_memberType != value)
                {
                    _memberType = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(Signature));
                }
            }
        }

        public ObservableCollection<IParameterInfoViewModel> Parameters { get; } = new ObservableCollection<IParameterInfoViewModel>();
    }
    public class ParameterInfoViewModel : ViewModelBase, IParameterInfoViewModel
    {
        private bool _isSelected;
        /// <summary>
        /// Whether the parameter is currently being supplied an argument expression at a call site.
        /// </summary>
        /// <remarks>
        /// The parameter would be bolded in a parameter info tooltip.
        /// </remarks>
        public bool IsSelected 
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// The '@Param docstring for this parameter.
        /// </summary>
        public string DocString { get; set; }
        /// <summary>
        /// Whether the parameter is optional or required.
        /// </summary>
        public bool IsOptional { get; set; }
        /// <summary>
        /// The ByRef/ByVal modifier, whether explicitly specified or not.
        /// </summary>
        public string Modifier { get; set; }
        /// <summary>
        /// The identifier name of the parameter.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// The compile-time type of the parameter, whether explicitly specified or not.
        /// </summary>
        public string AsType { get; set; }

        private bool _hasReferences = true;
        /// <summary>
        /// Whether or not the parameter is referenced or assigned in the parent procedure body.
        /// </summary>
        public bool HasReferences 
        {
            get => _hasReferences;
            set
            {
                if (_hasReferences != value)
                {
                    _hasReferences = value;
                    OnPropertyChanged();
                }
            }
        }
    }

    public class EditorShellDockablePresenter : DockableToolwindowPresenter
    {
        public EditorShellDockablePresenter(IVBE vbe, IAddIn addin, IEditorShellWindowProvider viewFactory) 
            : base(vbe, addin, viewFactory.Create())
        {
        }
    }
}