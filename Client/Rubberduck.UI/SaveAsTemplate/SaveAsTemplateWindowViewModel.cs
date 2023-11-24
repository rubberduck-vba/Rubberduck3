using Rubberduck.Resources.Menus;
using Rubberduck.UI.Command;
using Rubberduck.UI.Services;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rubberduck.UI.SaveAsTemplate
{
    public interface IWorkspaceFileViewModel
    {
        public string Uri { get; }
        public string Name { get; }
        public bool IsSourceFile { get; }

        public bool IsSelected { get; set; }
    }

    public class SaveAsTemplateWindowViewModel : DialogWindowViewModel
    {
        private readonly UIServiceHelper _service;
        private readonly IFileSystem _fileSystem;

        public SaveAsTemplateWindowViewModel(UIServiceHelper service, MessageActionCommand[] actions,
            IEnumerable<IWorkspaceFileViewModel> files,
            IFileSystem fileSystem)
            : base("Export Project Template", actions)
        {
            _service = service;
            _fileSystem = fileSystem;
            TemplateFiles = files;
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
                    ValidateName();
                }
            }
        }

        private void ValidateName()
        {
            ClearErrors();

            if (string.IsNullOrWhiteSpace(Name))
            {
                AddError(nameof(Name), "A template name is required.");
                return;
            }

            var illegalChars = _fileSystem.Path.GetInvalidPathChars();
            if (illegalChars.Any(invalid => Name.Contains(invalid)))
            {
                AddError(nameof(Name), $"Template names cannot contain any of the following characters: [{string.Join(',', illegalChars.Select(e => $"'{e}'"))}].");
                return;
            }
        }

        public IEnumerable<IWorkspaceFileViewModel> TemplateFiles { get; }

        protected override void ResetToDefaults(){ }
    }
}
