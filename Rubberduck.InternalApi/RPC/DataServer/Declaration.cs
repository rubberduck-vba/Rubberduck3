using Rubberduck.InternalApi.Model;

namespace Rubberduck.InternalApi.RPC.DataServer
{

    public class Declaration
    {
        public int Id { get; set; }
        public DeclarationType DeclarationType { get; set; }
        public string IdentifierName { get; set; }
        public bool IsUserDefined { get; set; }
        public string DocString { get; set; }
        public bool IsArray { get; set; }
        public string TypeHint { get; set; }

        public int? AsTypeDeclarationId { get; set; }
        public string AsTypeName { get; set; }
        public int? ParentDeclarationId { get; set; }

        public LocationInfo LocationInfo { get; set; }
    }
}
