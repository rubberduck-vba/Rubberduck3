using ProtoBuf;
using Rubberduck.DataServices.Repositories;
using System.Threading.Tasks;
using System;

namespace Rubberduck.DataServices.Entities
{
    internal class Project : DbEntity
    {
        public int DeclarationId { get; set; }
        public string VBProjectId { get; set; }
        public string Guid { get; set; }
        public int? MajorVersion { get; set; }
        public int? MinorVersion { get; set; }
        public string Path { get; set; }

        internal static Project FromModel(InternalApi.Model.RPC.Project model)
        {
            return new Project
            {
                Id = model.Id,
                DeclarationId = model.Declaration.Id,
                VBProjectId = model.VBProjectId,
                Guid = model.Guid?.ToString(),
                MajorVersion = model.MajorVersion,
                MinorVersion = model.MinorVersion,
                Path = model.Path,
            };
        }

        internal async Task<InternalApi.Model.RPC.Project> ToModelAsync(Repository<Declaration> repository)
        {
            var result = new InternalApi.Model.RPC.Project
            {
                Id = Id,
                Guid = Guid is null ? (Guid?)null : System.Guid.Parse(Guid),
                VBProjectId = VBProjectId,
                MajorVersion = MajorVersion,
                MinorVersion = MinorVersion,
                Path = Path,
            };

            var declaration = await repository.GetByIdAsync(DeclarationId);
            result.Declaration = await declaration.ToModelAsync(repository);

            return result;
        }
    }
}
