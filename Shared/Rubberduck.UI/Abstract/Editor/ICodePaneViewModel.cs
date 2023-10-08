using ICSharpCode.AvalonEdit.Document;
using Rubberduck.InternalApi.Model;
using Rubberduck.Unmanaged.Model;
using Rubberduck.Unmanaged.Model.Abstract;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;

namespace Rubberduck.UI.Abstract
{
    /* some of these are useless with LSP. TODO: clean this up */
    public interface ICodePaneViewModel : INotifyPropertyChanged
    {
        IModuleInfoViewModel ModuleInfo { get; set; }

        TextDocument Document { get; set; }
        string Content { get; set; }
        ObservableCollection<IMemberProviderViewModel> MemberProviders { get; }
        IMemberProviderViewModel SelectedMemberProvider { get; set; }
        event EventHandler SelectedMemberProviderChanged;
        IEditorSettings EditorSettings { get; }

        IEnumerable<ISyntaxErrorViewModel> SyntaxErrors { get; }
        IStatusBarViewModel Status { get; }

        ICommand CloseCommand { get; }
    }

    public interface IMemberProviderViewModel : INotifyPropertyChanged
    {
        //event EventHandler<NavigateToMemberEventArgs> MemberSelected;

        //IQualifiedModuleName QualifiedModuleName { get; set; }
        string Name { get; set; }
        ModuleType ModuleType { get; set; }

        ICollection<IMemberInfoViewModel> Members { get; set; }
        IMemberInfoViewModel CurrentMember { get; set; }
        void ClearMemberSelectedHandlers();
        void AddMember(string name, MemberType memberType, DocumentOffset offset);
    }

    public interface IModuleInfoViewModel : INotifyPropertyChanged
    {
        //IQualifiedModuleName QualifiedModuleName { get; set; }
        string Name { get; set; }
        ModuleType ModuleType { get; set; }

        /// <summary>
        /// The FolderAnnotation value for this module, if present.
        /// </summary>
        string Folder { get; set; }
        /// <summary>
        /// The last known caret location for this module.
        /// </summary>
        Selection EditorPosition { get; set; }
    }

    public interface IMemberInfoViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// True if the module contains an implementation for this member.
        /// </summary>
        bool HasImplementation { get; set; }
        /// <summary>
        /// True if the member is user-defined (and thus can be modified or removed).
        /// </summary>
        bool IsUserDefined { get; set; }
        MemberType MemberType { get; set; }
        /// <summary>
        /// The identifier name.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The name and parameters.
        /// </summary>
        string Signature { get; }
        IParameterInfoViewModel CurrentParameter { get; set; }
        ObservableCollection<IParameterInfoViewModel> Parameters { get; }
        DocumentOffset Offset { get; set; }
        int StartLine { get; set; }
        int EndLine { get; set; }
    }

    public interface IParameterInfoViewModel : INotifyPropertyChanged
    {
        bool IsSelected { get; set; }
        string DocString { get; set; }

        bool IsOptional { get; set; }
        string Modifier { get; set; }
        string Name { get; set; }
        string AsType { get; set; }
    }
}
