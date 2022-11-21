using ICSharpCode.AvalonEdit.Document;
using Rubberduck.VBEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;

namespace Rubberduck.UI.Abstract
{
    public enum ModuleType
    {
        None,
        StandardModule,
        ClassModule,
        ClassModuleInterface,
        ClassModulePrivate,
        ClassModulePredeclared,
        UserFormModule,
    }

    public enum MemberType
    {
        None,
        Const,
        ConstPrivate,
        ConstFriend,
        Enum,
        EnumPrivate,
        EnumFriend,
        EnumMember,
        Event,
        EventPrivate,
        EventFriend,
        Field,
        FieldPrivate,
        FieldFriend,
        Function,
        FunctionPrivate,
        FunctionFriend,
        Procedure,
        ProcedurePrivate,
        ProcedureFriend,
        PropertyGet,
        PropertyGetPrivate,
        PropertyGetFriend,
        PropertyLet,
        PropertyLetPrivate,
        PropertyLetFriend,
        PropertySet,
        PropertySetPrivate,
        PropertySetFriend,
        TestMethod,
        UserDefinedType,
        UserDefinedTypePrivate,
        UserDefinedTypeFriend,
        UserDefinedTypeMember,
    }

    public class NavigateToMemberEventArgs : EventArgs
    {
        public NavigateToMemberEventArgs(IMemberInfoViewModel memberInfo)
        {
            MemberInfo = memberInfo;
        }

        public IMemberInfoViewModel MemberInfo { get; }
    }

    public interface ICodePaneViewModel : INotifyPropertyChanged, IStatusUpdate
    {
        string Title { get; set; }
        string Content { get; set; }
        IModuleInfoViewModel ModuleInfo { get; set; }
        ObservableCollection<IMemberProviderViewModel> MemberProviders { get; set; }
        IMemberProviderViewModel SelectedMemberProvider { get; set; }
        IEditorSettings EditorSettings { get; set; }
    }

    public interface IMemberProviderViewModel : INotifyPropertyChanged
    {
        event EventHandler<NavigateToMemberEventArgs> MemberSelected;

        ObservableCollection<IMemberInfoViewModel> Members { get; }
        IMemberInfoViewModel CurrentMember { get; set; }

        void SetCurrentMember(int line);

        void AddMember(string name, MemberType memberType, (ITextAnchor start, ITextAnchor end) anchors);
    }

    public interface IModuleInfoViewModel : IMemberProviderViewModel
    {
        string Folder { get; set; }
        string Name { get; set; }
        ModuleType ModuleType { get; set; }
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
        MemberType MemberType { get; set; }
        /// <summary>
        /// The identifier name.
        /// </summary>
        string Name { get; set; }
        /// <summary>
        /// The name and parameters.
        /// </summary>
        string Signature { get; }
        ObservableCollection<IParameterInfoViewModel> Parameters { get; }
        int StartLine { get; }
        int EndLine { get; }
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
