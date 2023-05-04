using ICSharpCode.AvalonEdit.Document;
using Rubberduck.InternalApi.Model;
using Rubberduck.UI.Abstract;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using System;
using Rubberduck.VBEditor;
using System.Linq;

///<summary>
///This file provides design-time data for the <see cref="RubberduckEditorControl.xaml"/> control.
///This is only to support working in the XAML designer and nothing in this file should be used otherwise.
///</summary>
namespace Rubberduck.UI.Xaml.Controls
{
    internal class CodePaneDesignViewModel : ICodePaneViewModel
    {
        public IModuleInfoViewModel ModuleInfo { get; set; } = new ModuleInfoDesignViewModel();
        public ObservableCollection<IMemberProviderViewModel> MemberProviders { get; set; } = new ObservableCollection<IMemberProviderViewModel>
        {
            new MemberProviderDesignViewModel { Name = "Module1" },
            new MemberProviderDesignViewModel { Name = "Class1" }
        };
        public IMemberProviderViewModel SelectedMemberProvider { 
            get => MemberProviders.FirstOrDefault();
            set { }
        }
        public string Content { get; set; } = "Sub MySub()\n    Debug.Print \"Hello World\"\nEnd Sub";
        public IEditorSettings EditorSettings { get; set; } = new EditorSettingsDesignViewModel();

        public TextDocument Document { get; set; }
        public IEnumerable<ISyntaxErrorViewModel> SyntaxErrors => throw new NotImplementedException();
        public IStatusBarViewModel Status => new StatusBarDesignViewModel();
        public ICommand CloseCommand => new DummyCommand(); // => throw new NotImplementedException();

        public event EventHandler SelectedMemberProviderChanged;
        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal class DummyCommand : ICommand
    {
        public event EventHandler CanExecuteChanged;

        public bool CanExecute(object parameter) => true;

        public void Execute(object parameter) { }
    }

    internal class ModuleInfoDesignViewModel : IModuleInfoViewModel
    {
        public IQualifiedModuleName QualifiedModuleName { get; set; } = new QualifiedModuleName("projectName", "projectPath", "componentName");
        public string Name { get; set; } = "Module1";
        public ModuleType ModuleType { get; set; } = ModuleType.StandardModule;
        public string Folder { get; set; } = "VBAProject";
        public Selection EditorPosition { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal class MemberProviderDesignViewModel : IMemberProviderViewModel
    {
        public string Name { get; set; } = "Module1";
        public ModuleType ModuleType { get; set; } = ModuleType.StandardModule;
        public IMemberInfoViewModel CurrentMember
        {
            get => Members.FirstOrDefault();
            set { }
        }
        public ICollection<IMemberInfoViewModel> Members { get; set; } = new Collection<IMemberInfoViewModel>
        {
            new MemberInfoDesignViewModel { Name = "MySub" },
            new MemberInfoDesignViewModel { Name = "Other" }
        };
        public IQualifiedModuleName QualifiedModuleName { get; set; } = new QualifiedModuleName("projectName", "projectPath", "componentName");
        public event PropertyChangedEventHandler PropertyChanged;
        public void AddMember(string name, MemberType memberType, DocumentOffset offset)
        {
            throw new NotImplementedException();
        }
        public void ClearMemberSelectedHandlers()
        {
            throw new NotImplementedException();
        }
    }

    internal class MemberInfoDesignViewModel : IMemberInfoViewModel
    {
        public string Name { get; set; } = "MySub";
        public MemberType MemberType { get; set; } = MemberType.Procedure;
        public bool HasImplementation { get; set; }
        public bool IsUserDefined { get; set; }
        public string Signature { get; set; } = "";
        public IParameterInfoViewModel CurrentParameter { get; set; }
        public ObservableCollection<IParameterInfoViewModel> Parameters => throw new NotImplementedException();
        public DocumentOffset Offset { get; set; }
        public int StartLine { get; set; }
        public int EndLine { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
    }

    internal class EditorSettingsDesignViewModel : IEditorSettings
    {
        public string FontFamily { get; set; } = "Arial";
        public string FontSize { get; set; } = "14";
        public bool ShowLineNumbers { get; set; } = false;
        public double IdleTimeoutSeconds { get; set; } = 99;
    }
}
