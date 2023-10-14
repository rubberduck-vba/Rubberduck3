using Rubberduck.InternalApi.Model;

namespace Rubberduck.Parsing.Model;

public class MemberInfo
{
    public MemberInfo(string name, DocumentOffset offset, MemberType memberType, IEnumerable<ParameterInfo> parameters = null, IEnumerable<MemberInfo> members = null)
    {
        Name = name;
        Offset = offset;
        MemberType = memberType;
        Parameters = parameters ?? Enumerable.Empty<ParameterInfo>();
        Members = members ?? Enumerable.Empty<MemberInfo>();
    }

    public string Name { get; }
    public DocumentOffset Offset { get; }
    public MemberType MemberType { get; }
    public IEnumerable<ParameterInfo> Parameters { get; }
    public IEnumerable<MemberInfo> Members { get; }
}
