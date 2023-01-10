using ProtoBuf;
using Rubberduck.InternalApi.Model;
using System;

namespace Rubberduck.InternalApi.RPC.DataServer
{
    [ProtoContract]
    public class Project
    {
        [ProtoMember(1)]
        public int Id { get; set; }
        [ProtoMember(2)]
        public string Path { get; set; }
        [ProtoMember(3)]
        public Guid? Guid { get; set; }
        [ProtoMember(4)]
        public string VBProjectId { get; set; }
        [ProtoMember(5)]
        public int? MajorVersion { get; set; }
        [ProtoMember(6)]
        public int? MinorVersion { get; set; }

        [ProtoMember(7)]
        public int DeclarationId { get; set; }
        [ProtoMember(8)]
        public int? ParentDeclarationId { get; set; }
        [ProtoMember(9)]
        public DeclarationType DeclarationType { get; set; }
        [ProtoMember(10)]
        public string IdentifierName { get; set; }
        [ProtoMember(11)]
        public string DocString { get; set; }
        [ProtoMember(12)]
        public bool IsUserDefined { get; set; }
    }
}
