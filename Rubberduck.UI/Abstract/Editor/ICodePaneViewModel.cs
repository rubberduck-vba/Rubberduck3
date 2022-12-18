using Antlr4.Runtime.Tree;
using ICSharpCode.AvalonEdit.Document;
using Rubberduck.InternalApi.Model;
using Rubberduck.Parsing.Model;
using Rubberduck.VBEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Rubberduck.UI.Abstract
{
    public class NavigateToMemberEventArgs : EventArgs
    {
        public NavigateToMemberEventArgs(IMemberInfoViewModel memberInfo)
        {
            MemberInfo = memberInfo;
        }

        public IMemberInfoViewModel MemberInfo { get; }
    }

    public class ParseTreeEventArgs : EventArgs
    {
        public ParseTreeEventArgs(IParseTree tree, IEnumerable<MemberInfo> members, IEnumerable<BlockFoldingInfo> foldings, IEnumerable<ISyntaxErrorViewModel> syntaxErrors)
        {
            ParseTree = tree;
            MemberInfo = members;
            BlockFoldingInfo = foldings;
            SyntaxErrors = syntaxErrors;
        }

        public IParseTree ParseTree { get; }
        public IEnumerable<MemberInfo> MemberInfo { get; }
        public IEnumerable<BlockFoldingInfo> BlockFoldingInfo { get; }
        public IEnumerable<ISyntaxErrorViewModel> SyntaxErrors { get; }
    }

    public interface ICodePaneViewModel : INotifyPropertyChanged
    {
        IModuleInfoViewModel ModuleInfo { get; set; }

        TextDocument Document { get; set; }
        string Content { get; set; }
        ObservableCollection<IMemberProviderViewModel> MemberProviders { get; }
        IMemberProviderViewModel SelectedMemberProvider { get; set; }
        event EventHandler SelectedMemberProviderChanged;
        IEditorSettings EditorSettings { get; }
        Task ParseAsync(TextReader reader);
        event EventHandler<ParseTreeEventArgs> ParseTreeChanged;

        IEnumerable<ISyntaxErrorViewModel> SyntaxErrors { get; }
        IEnumerable<BlockFoldingInfo> Foldings { get; }
        IStatusBarViewModel Status { get; }

        ICommand CloseCommand { get; }
    }

    public interface IMemberProviderViewModel : INotifyPropertyChanged
    {
        event EventHandler<NavigateToMemberEventArgs> MemberSelected;

        QualifiedModuleName QualifiedModuleName { get; set; }
        string Name { get; set; }
        ModuleType ModuleType { get; set; }

        ICollection<IMemberInfoViewModel> Members { get; set; }
        IMemberInfoViewModel CurrentMember { get; set; }
        void ClearMemberSelectedHandlers();
        void AddMember(string name, MemberType memberType, DocumentOffset offset);
    }

    public interface IModuleInfoViewModel : INotifyPropertyChanged
    {
        QualifiedModuleName QualifiedModuleName { get; set; }
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
        bool HasReferences { get; set; } // TODO figure out how to dim unused/unreachable/dead code
        string DocString { get; set; }

        bool IsOptional { get; set; }
        string Modifier { get; set; }
        string Name { get; set; }
        string AsType { get; set; }
    }
}
