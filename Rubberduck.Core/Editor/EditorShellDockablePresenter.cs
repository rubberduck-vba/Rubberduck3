using ICSharpCode.AvalonEdit.Document;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.WinForms;
using Rubberduck.VBEditor;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rubberduck.Core.Editor
{
    public class MemberInfoViewModel : ViewModelBase, IMemberInfoViewModel, IEquatable<MemberInfoViewModel>
    {
        private static readonly IDictionary<string, string> _displayNamesByMemberType =
            Enum.GetValues(typeof(MemberType)).Cast<MemberType>().ToDictionary(m => m.ToString(), m => m.GetType().GetCustomAttribute<DisplayAttribute>()?.Name);

        public MemberInfoViewModel(int offset) : this()
        {
            _startOffset = offset;
        }

        public MemberInfoViewModel()
        {
            Parameters.CollectionChanged += OnParametersCollectionChanged;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(Name, _startOffset);
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

            return other.Name == Name && other._startOffset == _startOffset;
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

        private int _endOffset;
        public int EndOffset
        {
            get => _endOffset;
            set
            {
                if (_endOffset != value)
                {
                    _endOffset = value;
                    OnPropertyChanged();
                }
            }
        }

        private int _startOffset;
        public int StartOffset
        {
            get => _startOffset;
            set
            {
                if (_startOffset != value)
                {
                    _startOffset = value;
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
            : $"{_displayNamesByMemberType[_memberType.ToString()]} {_name}({string.Join(", ", Parameters.Select(p => $"{p.Name} As {p.AsType}"))})";


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
        public EditorShellDockablePresenter(IVBE vbe, IAddIn addin, EditorShellWindow view) 
            : base(vbe, addin, view)
        {
        }
    }
}