using System.Threading.Tasks;

namespace Rubberduck.InternalApi.Model.RPC
{
    public class Module
    {
        public int Id { get; set; } 

        public Declaration Declaration { get; set; }
        public string Folder { get; set; }
    }
}
