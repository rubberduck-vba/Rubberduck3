using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Abstract;
using Rubberduck.VBEditor;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;

///<summary>
///This file provides design-time data for the <see cref="SyncPanelToolControl.xaml"/> control.
///This is only to support working in the XAML designer and nothing in this file should be used otherwise.
///</summary>
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


