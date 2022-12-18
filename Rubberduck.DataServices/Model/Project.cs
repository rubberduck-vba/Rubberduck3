using ProtoBuf;
using Rubberduck.DataServices.Repositories;
using System;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Model
{
    public class Project
    {
        public int Id { get; set; }

        public Declaration Declaration { get; set; }
        public string VBProjectId { get; set; }
        public Guid? Guid { get; set; }
        public int? MajorVersion { get; set; }
        public int? MinorVersion { get; set; }
        public string Path { get; set; }

        internal Entities.Project ToEntity()
        {
            return new Entities.Project
            {
                Id = Id,
                DeclarationId = Declaration.Id,
                VBProjectId = VBProjectId,
                Guid = Guid?.ToString(),
                MajorVersion = MajorVersion,
                MinorVersion = MinorVersion,
                Path = Path,
            };
        }

        internal async Task<Project> FromEntityAsync(Entities.Project entity, Repository<Entities.Declaration> repository)
        {
            var result = new Project
            {
                Id = entity.Id,
                Guid = entity.Guid is null ? (Guid?)null : System.Guid.Parse(entity.Guid),
                VBProjectId = entity.VBProjectId,
                MajorVersion = entity.MajorVersion,
                MinorVersion = entity.MinorVersion,
                Path = entity.Path,
            };

            var declaration = await repository.GetByIdAsync(entity.DeclarationId);
            result.Declaration = await Declaration.FromEntityAsync(declaration, repository);

            return result;
        }
    }
}
