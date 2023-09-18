using System;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.UI.Xaml.Controls
{
    public class MemberEditor
    {
        public static MemberType GetMemberType(string line)
        {
            if (line.StartsWith("Private", StringComparison.InvariantCultureIgnoreCase))
            {
                switch (line.Substring(8, 4).ToLowerInvariant())
                {
                    case "cons":
                        return MemberType.ConstPrivate;
                    case "enum":
                        return MemberType.EnumPrivate;
                    case "even":
                        return MemberType.EventPrivate;
                    case "func":
                        return MemberType.FunctionPrivate;
                    case "sub ":
                        return MemberType.ProcedurePrivate;
                    case "type":
                        return MemberType.UserDefinedTypePrivate;
                    case "prop":
                        switch (line.Substring(17, 3).ToLowerInvariant())
                        {
                            case "get":
                                return MemberType.PropertyGetPrivate;
                            case "let":
                                return MemberType.PropertyLetPrivate;
                            case "set":
                                return MemberType.PropertySetPrivate;
                            default:
                                return MemberType.None;
                        }
                    default:
                        return MemberType.FieldPrivate;
                }
            }
            else if (line.StartsWith("Friend", StringComparison.InvariantCultureIgnoreCase))
            {
                switch (line.Substring(7, 4).ToLowerInvariant())
                {
                    case "cons":
                        return MemberType.ConstFriend;
                    case "enum":
                        return MemberType.EnumFriend;
                    case "even":
                        return MemberType.EventFriend;
                    case "func":
                        return MemberType.FunctionFriend;
                    case "sub ":
                        return MemberType.ProcedureFriend;
                    case "type":
                        return MemberType.UserDefinedTypeFriend;
                    case "prop":
                        switch (line.Substring(16, 3).ToLowerInvariant())
                        {
                            case "get":
                                return MemberType.PropertyGetFriend;
                            case "let":
                                return MemberType.PropertyLetFriend;
                            case "set":
                                return MemberType.PropertySetFriend;
                            default:
                                return MemberType.None;
                        }
                    default:
                        return MemberType.FieldFriend;
                }
            }
            else
            {
                switch (line.Substring(0, 4).ToLowerInvariant())
                {
                    case "cons":
                        return MemberType.Const;
                    case "enum":
                        return MemberType.Enum;
                    case "even":
                        return MemberType.Event;
                    case "func":
                        return MemberType.Function;
                    case "sub ":
                        return MemberType.Procedure;
                    case "type":
                        return MemberType.UserDefinedType;
                    case "prop":
                        switch (line.Substring(16, 3).ToLowerInvariant())
                        {
                            case "get":
                                return MemberType.PropertyGet;
                            case "let":
                                return MemberType.PropertyLet;
                            case "set":
                                return MemberType.PropertySet;
                            default:
                                return MemberType.None;
                        }
                    case "dim ":
                        return MemberType.Field;

                    default:
                        return MemberType.None;
                }
            }
        }
    }
}
