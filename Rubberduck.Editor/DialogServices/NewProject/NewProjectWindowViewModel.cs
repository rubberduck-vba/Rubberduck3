using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.SettingsProvider.Model;
using Rubberduck.SettingsProvider.Model.LanguageClient;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.SharedHandlers;
using Rubberduck.UI.NewProject;
using Rubberduck.UI.Services;
using Rubberduck.UI.Windows;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Rubberduck.Editor.DialogServices.NewProject
{
    public class NewProjectWindowViewModel : DialogWindowViewModel, INewProjectWindowViewModel
    {
        public static readonly string SourceFolderName = "src";
        private readonly RubberduckSettings _settings;

        private static bool Validate(object? param)
        {
            if (param is null)
            {
                return true;
            }

            var vm = (NewProjectWindowViewModel)(param as NewProjectWindow
                ?? throw new ArgumentException("Parameter was not of the expected type.")).DataContext;

            return !vm.HasErrors;
        }

        public NewProjectWindowViewModel(UIServiceHelper service, IEnumerable<VBProjectInfo?> projects, IEnumerable<ProjectTemplate> projectTemplates, 
            MessageActionsProvider actions, ICommand showSettingsCommand)
            : base(service, "New Project", actions.OkCancel(Validate), showSettingsCommand, typeof(LanguageClientSettings).Name)
        {
            _settings = service.Settings;
            _rootUri = _settings.LanguageClientSettings.WorkspaceSettings.DefaultWorkspaceRoot;

            VBProjects = projects;
            SelectedVBProject = VBProjects.FirstOrDefault();
            HasVBProjects = projects.Any();

            ProjectTemplates = projectTemplates;
            SelectedProjectTemplate = HasVBProjects ? null : projectTemplates.FirstOrDefault();
            HasProjectTemplates = projectTemplates.Any();

            CloseWindowCommand = Actions[1];

            ResetToDefaults();
            CommandBindings = new CommandBinding[]
            {
                new(NavigationCommands.Search, DialogCommandHandlers.BrowseLocationCommandBinding_Executed, DialogCommandHandlers.BrowseLocationCommandBinding_CanExecute),
            };
        }

        public override IEnumerable<CommandBinding> CommandBindings { get; }
        public ICommand CloseWindowCommand { get; }

        protected override void ResetToDefaults()
        {
            var vbProject = VBProjects.FirstOrDefault() ?? new VBProjectInfo { Name = "VBAProject", ProjectId = Guid.NewGuid().ToString() };
            _projectName = vbProject.Name;
            _workspaceLocation = string.IsNullOrWhiteSpace(vbProject.Location)
                ? _settings.LanguageClientSettings.WorkspaceSettings.DefaultWorkspaceRoot.LocalPath
                : vbProject.Location;
        }

        private string _projectName = string.Empty;
        [Required, MaxLength(31)] // todo actually validate this
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

        public bool HasVBProjects { get; init; }

        public bool IsWorkspaceLocationEditable => !_settings.LanguageClientSettings.WorkspaceSettings.RequireDefaultWorkspaceRootHost;

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

                    if (_selectedVBProject != null)
                    {
                        ProjectName = _selectedVBProject.Name;
                    }
                }
            }
        }

        public bool HasProjectTemplates { get; init; }
        public IEnumerable<ProjectTemplate> ProjectTemplates { get; init; }
        private ProjectTemplate? _selectedProjectTemplate;
        public ProjectTemplate? SelectedProjectTemplate
        {
            get => _selectedProjectTemplate;
            set
            {
                if (_selectedProjectTemplate != value)
                {
                    _selectedProjectTemplate = value;
                    OnPropertyChanged();
                }
            }
        }

        private Uri _rootUri;
        Uri IBrowseSelectionModel.RootUri { get => _rootUri; set => _rootUri = value; }
        string IBrowseSelectionModel.Title { get; set; } = "Workspace location"; // TODO localize
        string IBrowseSelectionModel.Selection { get => WorkspaceLocation; set => WorkspaceLocation = value; }
    }
}
