using ProtoBuf;

namespace Rubberduck.DataServices.Entities
{
    internal class Module : DbEntity
    {
        public int DeclarationId { get; set; }
        public string Folder { get; set; }
    }
}
