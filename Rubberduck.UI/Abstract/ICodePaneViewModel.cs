using ICSharpCode.AvalonEdit.Document;
using Rubberduck.VBEditor;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

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
        [Display(Name = "Public Const")]
        Const,
        [Display(Name = "Private Const")]
        ConstPrivate,
        [Display(Name = "Friend Const")]
        ConstFriend,
        [Display(Name = "Public Enum")]
        Enum,
        [Display(Name = "Private Enum")]
        EnumPrivate,
        [Display(Name = "Friend Enum")]
        EnumFriend,
        EnumMember,
        [Display(Name = "Public Event")]
        Event,
        [Display(Name = "Private Event")]
        EventPrivate,
        [Display(Name = "Friend Event")]
        EventFriend,
        [Display(Name = "Public")]
        Field,
        [Display(Name = "Private")]
        FieldPrivate,
        [Display(Name = "Friend")]
        FieldFriend,
        [Display(Name = "Public Function")]
        Function,
        [Display(Name = "Private Function")]
        FunctionPrivate,
        [Display(Name = "Friend Function")]
        FunctionFriend,
        [Display(Name = "Public Sub")]
        Procedure,
        [Display(Name = "Private Sub")]
        ProcedurePrivate,
        [Display(Name = "Friend Sub")]
        ProcedureFriend,
        [Display(Name = "Public Property Get")]
        PropertyGet,
        [Display(Name = "Private Property Get")]
        PropertyGetPrivate,
        [Display(Name = "Friend Property Get")]
        PropertyGetFriend,
        [Display(Name = "Public Property Let")]
        PropertyLet,
        [Display(Name = "Private Property Let")]
        PropertyLetPrivate,
        [Display(Name = "Friend Property Let")]
        PropertyLetFriend,
        [Display(Name = "Public Property Set")]
        PropertySet,
        [Display(Name = "Private Property Set")]
        PropertySetPrivate,
        [Display(Name = "Friend Property Set")]
        PropertySetFriend,
        TestMethod,
        [Display(Name = "Public Type")]
        UserDefinedType,
        [Display(Name = "Private Type")]
        UserDefinedTypePrivate,
        [Display(Name = "Friend Type")]
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
        event EventHandler SelectedMemberProviderChanged;
        IEditorSettings EditorSettings { get; set; }
    }

    public interface IMemberProviderViewModel : INotifyPropertyChanged
    {
        event EventHandler<NavigateToMemberEventArgs> MemberSelected;

        ObservableCollection<IMemberInfoViewModel> Members { get; }
        IMemberInfoViewModel CurrentMember { get; set; }
        void ClearMemberSelectedHandlers();
        void AddMember(string name, MemberType memberType, int startOffset, int endOffset);
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
        int StartOffset { get; set; }
        int StartLine { get; set; }
        int EndOffset { get; set; }
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
