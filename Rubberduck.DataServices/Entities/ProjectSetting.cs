using Rubberduck.DataServices.Repositories;
using Rubberduck.InternalApi.Model.RPC;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Entities
{
    internal class ProjectSetting
    {
        public int Id { get; set; }
        public int ProjectId { get; set; }
        public int DataType { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        internal static ProjectSetting FromModel(InternalApi.Model.RPC.ProjectSetting model)
        {
            return new ProjectSetting
            {
                Id = model.Id,
                ProjectId = model.Project.Id,
                DataType = (int)model.DataType,
                Key = model.Key,
                Value = model.Value,
            };
        }

        internal async Task<InternalApi.Model.RPC.ProjectSetting> FromEntityAsync(Repository<Project> repository, Repository<Entities.Declaration> declarations)
        {
            var result = new InternalApi.Model.RPC.ProjectSetting
            {
                Id = Id,
                DataType = (ProjectSettingDataType)DataType,
                Key = Key,
                Value = Value,
            };

            var project = await repository.GetByIdAsync(ProjectId);
            result.Project = await project.ToModelAsync(declarations);

            return result;
        }
    }
}
