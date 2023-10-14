using Rubberduck.InternalApi.Model;
using System.Collections.Generic;

namespace Rubberduck.Parsing.Model
{
    public class TypedMemberInfo : MemberInfo
    {
        public TypedMemberInfo(string name, DocumentOffset offset, MemberType memberType, string typeName, IEnumerable<ParameterInfo> parameters = null, IEnumerable<MemberInfo> members = null) 
            : base(name, offset, memberType, parameters, members)
        {
            TypeName = typeName;
        }

        public string TypeName { get; }
    }
}
