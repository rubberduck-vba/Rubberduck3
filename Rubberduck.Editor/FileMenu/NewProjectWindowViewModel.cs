using Rubberduck.Editor.Command;
using Rubberduck.Editor.Message;
using Rubberduck.Resources;
using Rubberduck.UI;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rubberduck.Editor.FileMenu
{
    public interface IDialogService<TViewModel>
        where TViewModel : IDialogWindowViewModel
    {
        TViewModel ShowDialog();
    }

    public interface INewProjectDialogService : IDialogService<NewProjectWindowViewModel> { }

    public class NewProjectDialogService : INewProjectDialogService
    {
        public NewProjectWindowViewModel ShowDialog()
        {
            throw new NotImplementedException();
        }
    }

    public class NewProjectWindowViewModel : DialogWindowViewModel
    {
        public static readonly string SourceFolderName = "src";

        private static bool Validate(object? param)
        {
            var vm = param as NewProjectWindowViewModel 
                ?? throw new ArgumentException("Parameter was not of the expected type.");

            return !vm.HasErrors;
        }

        public NewProjectWindowViewModel(string name, string location, MessageActionsProvider actions)
            : base("New Project", actions.OkCancel(Validate))
        {
            _projectName = name;
            _workspaceLocation = location;
        }

        private string _projectName;
        [Required, MaxLength(31)]
        public string ProjectName 
        {
            get => _projectName;
            set
            {
                if (_projectName != value)
                {
                    _projectName = value;
                    OnPropertyChanged();
                }
            }
        }

        private string _workspaceLocation;
        [Required, MaxLength(1023)]
        public string WorkspaceLocation
        {
            get => _workspaceLocation;
            set
            {
                if (_workspaceLocation != value)
                {
                    _workspaceLocation = value;
                    OnPropertyChanged();
                }
            }
        }

        public string SourcePath => Path.Combine(WorkspaceLocation, ProjectName, SourceFolderName);
    }

    public abstract class DialogWindowViewModel : ViewModelBase, IDialogWindowViewModel
    {
        public static MessageAction[] ActionCloseOnly { get; } = new[] { MessageAction.CloseAction };
        public static MessageAction[] ActionAcceptCancel { get; } = new[] { MessageAction.AcceptAction, MessageAction.CancelAction };

        protected DialogWindowViewModel(string title, MessageActionCommand[] actions)
        {
            Title = title;
            Actions = actions;
            IsEnabled = true;
        }

        public MessageActionCommand[] Actions { get; init; }

        public MessageAction? SelectedAction { get; set; }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public string Title { get; init; }
    }
}
