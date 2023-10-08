using Rubberduck.InternalApi.Model;
using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.Unmanaged.Model;
using Rubberduck.Unmanaged.Model.Abstract;

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

        private IQualifiedModuleName _qualifiedModuleName;
        public IQualifiedModuleName QualifiedModuleName 
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
}