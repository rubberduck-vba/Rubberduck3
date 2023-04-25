using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Abstract;
using Rubberduck.VBEditor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

namespace Rubberduck.UI.Xaml.Controls
{
    internal class SyncPanelDesignViewModel : ISyncPanelViewModel
    {
        public ObservableCollection<ISyncPanelModuleViewModel> VBIDEModules => new ObservableCollection<ISyncPanelModuleViewModel>
        {
             new SyncPanelModuleDesignViewModel
            {
                ModuleType = ModuleType.StandardModule,
                QualifiedModuleName = new QualifiedModuleName("projectName", "projectPath", "Module1")
            },
            new SyncPanelModuleDesignViewModel
            {
                ModuleType = ModuleType.ClassModule,
                QualifiedModuleName = new QualifiedModuleName("projectName", "projectPath", "Class1")
            }
        };

        public ISyncPanelModuleViewModel SelectedVBIDEModule
        {
            get => VBIDEModules.FirstOrDefault();
            set { }
        }

        public IEditorShellViewModel Shell { get; set; }

        public ICommand ReloadCommand => new DummyCommand();
        public ICommand SyncCommand => new DummyCommand();

        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal class SyncPanelModuleDesignViewModel : ISyncPanelModuleViewModel
    {
        public IQualifiedModuleName QualifiedModuleName { get; set; } = new QualifiedModuleName("projectName", "projectPath", "Module1");
        public ModuleType ModuleType { get; set; } = ModuleType.StandardModule;
        public ModuleSyncState State { get; set; } = ModuleSyncState.OK;

        public ICommand SyncCommand => new DummyCommand();
        public ICommand LoadCommand => new DummyCommand();

        public event PropertyChangedEventHandler PropertyChanged;
    }
}


