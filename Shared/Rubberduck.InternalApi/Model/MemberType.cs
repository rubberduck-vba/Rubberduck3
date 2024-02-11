using System.ComponentModel.DataAnnotations;

namespace Rubberduck.InternalApi.Model;

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
