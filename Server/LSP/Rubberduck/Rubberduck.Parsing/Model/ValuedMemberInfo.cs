using Rubberduck.InternalApi.Model;

namespace Rubberduck.Parsing.Model
{
    public class ValuedMemberInfo : TypedMemberInfo
    {
        public ValuedMemberInfo(string name, DocumentOffset offset, MemberType memberType, string typeName, string value) : base(name, offset, memberType, typeName, null, null)
        {
            DeclaredValue = value;
        }

        public string DeclaredValue { get; }
    }
}
