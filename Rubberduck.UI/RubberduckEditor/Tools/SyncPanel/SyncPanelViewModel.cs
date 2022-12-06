using Rubberduck.UI;
using Rubberduck.UI.Abstract;
using Rubberduck.UI.Command;
using Rubberduck.UI.Command.SyncPanel;
using Rubberduck.UI.RubberduckEditor;
using Rubberduck.VBEditor.ComManagement;
using Rubberduck.VBEditor.Events;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using Rubberduck.VBEditor.SourceCodeHandling;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rubberduck.Core.Editor.Tools
{
    public class SyncPanelViewModel : ViewModelBase, ISyncPanelViewModel
    {
        private readonly IProjectsRepository _projectsRepository;
        private readonly ISyncPanelModuleViewModelProvider _vmFactory;

        public SyncPanelViewModel(IProjectsRepository projectsRepository, ISyncPanelModuleViewModelProvider vmFactory, ISyncCommand syncCommand)
        {
            _projectsRepository = projectsRepository;
            _vmFactory = vmFactory;

            var projects = _projectsRepository.ProjectsCollection();
            projects.ProjectAdded += Projects_ProjectAdded;
            projects.ProjectRemoved += Projects_ProjectRemoved;
            projects.ProjectRenamed += Projects_ProjectRenamed;

            ReloadCommand = new DelegateCommand(null, ExecuteReloadCommand);
            SyncCommand = syncCommand;
        }

        public IEditorShellViewModel Shell
        {
            get => EditorShellContext.Current.Shell;
            set => EditorShellContext.Current.Shell = value;
        }

        public ObservableCollection<ISyncPanelModuleViewModel> VBIDEModules { get; } = new ObservableCollection<ISyncPanelModuleViewModel>();

        private ISyncPanelModuleViewModel _selectedVBIDEModule;
        public ISyncPanelModuleViewModel SelectedVBIDEModule
        {
            get => _selectedVBIDEModule;
            set
            {
                if (_selectedVBIDEModule != value) 
                {
                    _selectedVBIDEModule = value;
                    OnPropertyChanged();
                }
            }
        }

        public ICommand ReloadCommand { get; }
        public ICommand SyncCommand { get; }

        private void ExecuteReloadCommand(object param)
        {
            _projectsRepository.Refresh();

            using (var projects = _projectsRepository.ProjectsCollection())
            {
                var projectIds = projects.Select(e => e.ProjectId).Distinct().ToHashSet();

                var existing = VBIDEModules.Select(e => e.QualifiedModuleName.ProjectId).Distinct().ToHashSet();

                var added = projectIds.Where(e => !existing.Contains(e)).ToHashSet();
                var removed = existing.Except(projectIds).ToHashSet();

                if (!added.Any() && !removed.Any())
                {
                    return;
                }

                foreach (var id in added)
                {
                    using (var project = projects.SingleOrDefault(e => e.ProjectId == id))
                    {
                        RegisterProjectComponents(project);
                    }
                }

                foreach (var id in removed)
                {
                    using (var project = projects.SingleOrDefault(e => e.ProjectId == id))
                    {
                        DeregisterProjectComponents(project);
                    }
                }

                OnPropertyChanged(nameof(VBIDEModules));
            }
        }

        public void Deregister()
        {
            var projects = _projectsRepository.ProjectsCollection();
            projects.ProjectAdded -= Projects_ProjectAdded;
            projects.ProjectRemoved -= Projects_ProjectRemoved;
            projects.ProjectRenamed -= Projects_ProjectRenamed;

            foreach (var project in projects)
            {
                DeregisterProjectComponents(project);
            }
        }

        private void RegisterProjectComponents(IVBProject project)
        {
            var components = project.VBComponents;
            foreach (var component in components)
            {
                VBIDEModules.Add(_vmFactory.Create(new ComponentEventArgs(component.QualifiedModuleName)));
            }
            components.ComponentAdded += Components_ComponentAdded;
            components.ComponentRemoved += Components_ComponentRemoved;
            components.ComponentRenamed += Components_ComponentRenamed;
        }
        private void DeregisterProjectComponents(IVBProject project)
        {
            var components = project.VBComponents;
            foreach (var vbModule in VBIDEModules.Where(e => e.QualifiedModuleName?.ProjectId == project.ProjectId))
            {
                VBIDEModules.Remove(vbModule);
            }
            components.ComponentAdded -= Components_ComponentAdded;
            components.ComponentRemoved -= Components_ComponentRemoved;
            components.ComponentRenamed -= Components_ComponentRenamed;
        }

        private void Components_ComponentRenamed(object sender, ComponentRenamedEventArgs e)
        {
            var module = Shell.LoadedModules
                .SingleOrDefault(m => m.ModuleInfo.QualifiedModuleName.ProjectId == e.ProjectId && m.ModuleInfo.Name == e.OldName);
            module.ModuleInfo.Name = e.QualifiedModuleName.Name;
            module.ModuleInfo.QualifiedModuleName = e.QualifiedModuleName;
        }

        private void Components_ComponentRemoved(object sender, ComponentEventArgs e)
        {
            var vm = VBIDEModules
                .SingleOrDefault(m => m.QualifiedModuleName.Equals(e.QualifiedModuleName));
            
            vm.State = ModuleSyncState.DeletedVBE;

            var module = Shell.LoadedModules
                .SingleOrDefault(m => m.ModuleInfo.QualifiedModuleName.Equals(e.QualifiedModuleName));

            Shell.LoadedModules.Remove(module);
            VBIDEModules.Remove(vm);
        }

        private void Components_ComponentAdded(object sender, ComponentEventArgs e)
        {
            var vm = _vmFactory.Create(e);
            VBIDEModules.Add(vm);
        }

        private void Projects_ProjectRenamed(object sender, ProjectRenamedEventArgs e)
        {
            
        }

        private void Projects_ProjectRemoved(object sender, ProjectEventArgs e)
        {
            var project = _projectsRepository.Project(e.ProjectId);
            DeregisterProjectComponents(project);
        }

        private void Projects_ProjectAdded(object sender, ProjectEventArgs e)
        {
            var project = _projectsRepository.Project(e.ProjectId);
            RegisterProjectComponents(project);
        }
    }
}
