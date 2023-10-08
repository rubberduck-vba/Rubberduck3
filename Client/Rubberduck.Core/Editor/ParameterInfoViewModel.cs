using Rubberduck.UI;
using Rubberduck.UI.Abstract;

namespace Rubberduck.Core.Editor
{
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
}