using Rubberduck.SettingsProvider.Model;
using Rubberduck.UI.Command;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Windows.Input;

namespace Rubberduck.UI.NewProject
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
        private readonly RubberduckSettings _settings;

        private static bool Validate(object? param)
        {
            var vm = param as NewProjectWindowViewModel 
                ?? throw new ArgumentException("Parameter was not of the expected type.");

            return !vm.HasErrors;
        }

        public NewProjectWindowViewModel(RubberduckSettings settings, IEnumerable<VBProjectInfo?> projects, MessageActionsProvider actions, ICommand showSettingsCommand)
            : base("New Project", actions.OkCancel(Validate), showSettingsCommand)
        {
            _settings = settings;
            VBProjects = projects;
            ResetToDefaults();
        }

        protected override void ResetToDefaults()
        {
            var vbProject = VBProjects.FirstOrDefault() ?? new VBProjectInfo { Name = "VBAProject", ProjectId = Guid.NewGuid().ToString() };
            _projectName = vbProject.Name;
            _workspaceLocation = (string.IsNullOrWhiteSpace(vbProject.Location)
                ? _settings.LanguageClientSettings.DefaultWorkspaceRoot
                : new Uri(vbProject.Location)).ToString();
        }

        private string _projectName = string.Empty;
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
                    OnSourcePathChanged();
                }
            }
        }

        private string _workspaceLocation = string.Empty;
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
                    OnSourcePathChanged();
                }
            }
        }

        private void OnSourcePathChanged() => OnPropertyChanged(nameof(SourcePath));
        public string SourcePath => Path.Combine(WorkspaceLocation, ProjectName, SourceFolderName);

        public IEnumerable<VBProjectInfo?> VBProjects { get; init; }

        private VBProjectInfo _selectedVBProject;
        public VBProjectInfo SelectedVBProject 
        {
            get => _selectedVBProject;
            set
            {
                if (_selectedVBProject != value)
                {
                    _selectedVBProject = value;
                    OnPropertyChanged();

                    if (_selectedVBProject != default)
                    {
                        ProjectName = _selectedVBProject.Name;
                    }
                }
            }
        }
    }
}
