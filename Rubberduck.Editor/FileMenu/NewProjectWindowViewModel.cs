using Rubberduck.Editor.Command;
using Rubberduck.Editor.Message;
using Rubberduck.InternalApi.Settings;
using Rubberduck.Resources;
using Rubberduck.SettingsProvider.Model;
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
    public interface INewProjectWindowViewModel
    {
        IEnumerable<VBProjectInfo?> VBProjects { get; }

        string ProjectName { get; set; }
        string WorkspaceLocation { get; set; }
    }

    public readonly record struct VBProjectInfo
    {
        public string Name { get; init; }
        public string ProjectId { get; init; }
        public string? Location { get; init; }
    }

    public class NewProjectWindowViewModel : DialogWindowViewModel, INewProjectWindowViewModel
    {
        public static readonly string SourceFolderName = "src";



        private static bool Validate(object? param)
        {
            var vm = param as NewProjectWindowViewModel 
                ?? throw new ArgumentException("Parameter was not of the expected type.");

            return !vm.HasErrors;
        }

        public NewProjectWindowViewModel(RubberduckSettings settings, IEnumerable<VBProjectInfo?> projects, MessageActionsProvider actions)
            : base("New Project", actions.OkCancel(Validate))
        {
            VBProjects = projects;
            var vbProject = projects.FirstOrDefault() ?? new VBProjectInfo { Name = "NewProject", ProjectId = Guid.NewGuid().ToString() };

            _projectName = vbProject.Name;
            _workspaceLocation = vbProject.Location ?? settings.LanguageClientSettings.DefaultWorkspaceRoot;
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

        public IEnumerable<VBProjectInfo?> VBProjects { get; init; }

        private VBProjectInfo? _selectedVBProject;
        public VBProjectInfo? SelectedVBProject 
        {
            get => _selectedVBProject;
            set
            {
                if (_selectedVBProject != value)
                {
                    _selectedVBProject = value;
                    OnPropertyChanged();

                    if (_selectedVBProject.HasValue)
                    {
                        ProjectName = _selectedVBProject.Value.Name;
                    }
                }
            }
        }
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
