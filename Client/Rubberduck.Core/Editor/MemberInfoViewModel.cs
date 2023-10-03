using Rubberduck.InternalApi.Model;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace Rubberduck.Core.Editor
{
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
}