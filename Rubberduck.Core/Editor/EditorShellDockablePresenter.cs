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
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Rubberduck.Core.Editor
{
    public class MemberInfoViewModel : ViewModelBase, IMemberInfoViewModel
    {
        private readonly ITextAnchor _start = null;
        private readonly ITextAnchor _end = null;

        private readonly Selection? _span = null;

        public MemberInfoViewModel()
        {
        }

        public MemberInfoViewModel(Selection span)
        {
            _span = span;
        }

        public MemberInfoViewModel(ITextAnchor start, ITextAnchor end)
        {
            _start = start;
            _start.MovementType = AnchorMovementType.BeforeInsertion;
            _end = end;
            _end.MovementType = AnchorMovementType.BeforeInsertion;
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

        public string Signature => MemberType == MemberType.None
            ? _name
            : $"{_name}({string.Join(", ", Parameters.Select(p => $"{p.Name} As {p.AsType}"))})";

        public int StartLine => !(_start?.IsDeleted ?? true) ? _start?.Line ?? _span?.StartLine ?? 1 : 1;
        public int EndLine => !(_end?.IsDeleted ?? true) ? _end?.Line ?? _span?.EndLine ?? 1 : 1;

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