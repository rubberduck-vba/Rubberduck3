using ProtoBuf;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.Client.RPC
{
    [ProtoContract]
    public class Declaration
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public DeclarationType DeclarationType { get; set; }
        [ProtoMember(3)]
        public string IdentifierName { get; set; }
        [ProtoMember(4)]
        public bool IsUserDefined { get; set; }
        [ProtoMember(5)]
        public string DocString { get; set; }
        [ProtoMember(6)]
        public bool IsArray { get; set; }
        [ProtoMember(7)]
        public string TypeHint { get; set; }

        [ProtoMember(8)]
        public int? AsTypeDeclarationId { get; set; }
        [ProtoMember(9)]
        public string AsTypeName { get; set; }
        [ProtoMember(10)]
        public int? ParentDeclarationId { get; set; }

        [ProtoMember(11)]
        public LocationInfo LocationInfo { get; set; }
    }
}
