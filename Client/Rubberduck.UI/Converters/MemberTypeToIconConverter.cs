using Rubberduck.InternalApi.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Media;
using Icons = Rubberduck.Resources.MemberTypeIcons;

namespace Rubberduck.UI.Converters
{
    public class MemberTypeToIconConverter : ImageSourceConverter
    {
        private static readonly IDictionary<MemberType, ImageSource> MemberTypeIcons = new Dictionary<MemberType, ImageSource>
        {
            [MemberType.Const] = ToImageSource(Icons.ObjectConstant),
            [MemberType.ConstPrivate] = ToImageSource(Icons.ObjectConstantPrivate),
            [MemberType.ConstFriend] = ToImageSource(Icons.ObjectConstantFriend),
            [MemberType.Enum] = ToImageSource(Icons.ObjectEnum),
            [MemberType.EnumPrivate] = ToImageSource(Icons.ObjectEnumPrivate),
            [MemberType.EnumFriend] = ToImageSource(Icons.ObjectEnumFriend),
            [MemberType.EnumMember] = ToImageSource(Icons.ObjectEnumItem),
            [MemberType.Event] = ToImageSource(Icons.ObjectEvent),
            [MemberType.EventPrivate] = ToImageSource(Icons.ObjectEventPrivate),
            [MemberType.EventFriend] = ToImageSource(Icons.ObjectEventFriend),
            [MemberType.Field] = ToImageSource(Icons.ObjectField),
            [MemberType.FieldPrivate] = ToImageSource(Icons.ObjectFieldPrivate),
            [MemberType.FieldFriend] = ToImageSource(Icons.ObjectFieldFriend),
            [MemberType.Function] = ToImageSource(Icons.ObjectMethod),
            [MemberType.FunctionPrivate] = ToImageSource(Icons.ObjectMethodPrivate),
            [MemberType.FunctionFriend] = ToImageSource(Icons.ObjectMethodFriend),
            [MemberType.Procedure] = ToImageSource(Icons.ObjectMethod),
            [MemberType.ProcedurePrivate] = ToImageSource(Icons.ObjectMethodPrivate),
            [MemberType.ProcedureFriend] = ToImageSource(Icons.ObjectMethodFriend),
            [MemberType.PropertyGet] = ToImageSource(Icons.ObjectProperties),
            [MemberType.PropertyGetPrivate] = ToImageSource(Icons.ObjectPropertiesPrivate),
            [MemberType.PropertyGetFriend] = ToImageSource(Icons.ObjectPropertiesFriend),
            [MemberType.PropertyLet] = ToImageSource(Icons.ObjectProperties),
            [MemberType.PropertyLetPrivate] = ToImageSource(Icons.ObjectPropertiesPrivate),
            [MemberType.PropertyLetFriend] = ToImageSource(Icons.ObjectPropertiesFriend),
            [MemberType.PropertySet] = ToImageSource(Icons.ObjectProperties),
            [MemberType.PropertySetPrivate] = ToImageSource(Icons.ObjectPropertiesPrivate),
            [MemberType.PropertySetFriend] = ToImageSource(Icons.ObjectPropertiesFriend),
            [MemberType.UserDefinedType] = ToImageSource(Icons.ObjectValueType),
            [MemberType.UserDefinedTypePrivate] = ToImageSource(Icons.ObjectValueTypePrivate),
            [MemberType.UserDefinedTypeFriend] = ToImageSource(Icons.ObjectValueTypeFriend),
            [MemberType.UserDefinedTypeMember] = ToImageSource(Icons.ObjectField),
        };

        public override object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is null)
            {
                return null!;
            }

            var memberType = (MemberType)value;
            if (MemberTypeIcons.TryGetValue(memberType, out var icon))
            {
                return icon;
            }

            return null!;
        }
    }
}