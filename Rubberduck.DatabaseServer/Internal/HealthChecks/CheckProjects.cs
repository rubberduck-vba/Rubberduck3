using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.ServerPlatform.Model;
using Rubberduck.ServerPlatform.Model.Entities;

namespace Rubberduck.DatabaseServer.Internal.HealthChecks
{
    internal class CheckProjects : HealthCheck
    {
        protected async override Task<HealthCheckReport> GetReportAsync(IUnitOfWork context)
        {
            var items = new List<HealthCheckReport.Item>();

            var repo = context.GetRepository<Project>();
            var tableRecords = await repo.GetCountAsync();

            items.Add(new HealthCheckReport.Item
            { 
                Name = "RecordCount",
                Value = tableRecords.ToString("N0"),
                ValueDescription = "The number of records in the [Projects] table"
            });

            var view = context.GetView<ProjectInfo>();
            var viewRecords = await repo.GetCountAsync();

            items.Add(new HealthCheckReport.Item
            {
                Name = "RecordCount",
                Value = tableRecords.ToString("N0"),
                ValueDescription = "The number of records in the [ProjectInfo] view"
            });

            return HealthCheckReport.Success(Name, SuccessMessage, items.ToArray());
        }
    }
}
