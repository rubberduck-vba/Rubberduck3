using Rubberduck.DataServices.Repositories;
using System.Threading.Tasks;

namespace Rubberduck.DataServices.Model
{
    public enum ProjectSettingDataType
    {
        Bool = 0,
        String = 1,
        Integer = 2,
        Float = 3,
    }

    public class ProjectSetting
    {
        public int Id { get; set; }
        public Project Project { get; set; }
        public ProjectSettingDataType DataType { get; set; }
        public string Key { get; set; }
        public string Value { get; set; }

        internal Entities.ProjectSetting ToEntity()
        {
            return new Entities.ProjectSetting
            {
                Id = Id,
                ProjectId = Project.Id,
                DataType = (int)DataType,
                Key = Key,
                Value = Value,
            };
        }

        internal async Task<ProjectSetting> FromEntityAsync(Entities.ProjectSetting entity, Repository<Entities.Project> repository, Repository<Entities.Declaration> declarations)
        {
            var result = new ProjectSetting
            {
                Id = entity.Id,
                DataType = (ProjectSettingDataType)entity.DataType,
                Key = entity.Key,
                Value = entity.Value,
            };

            var project = await repository.GetByIdAsync(entity.ProjectId);
            result.Project = await Project.FromEntityAsync(project, declarations);

            return result;
        }
    }
}
