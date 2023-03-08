using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.ServerPlatform.Model.Entities;
using System.Data;

namespace Rubberduck.DatabaseServer.Internal.Storage
{
    internal class ProjectsView : View<ProjectInfo>
    {
        public ProjectsView(IDbConnection connection) 
            : base(connection) { }

        protected override string[] ColumnNames { get; } = new[]
        {
            "Id",
            "VBProjectId",
            "Guid",
            "MajorVersion",
            "MinorVersion",
            "Path",
            "DeclarationId",
            "DeclarationType",
            "IdentifierName",
            "IsUserDefined",
        };

        protected override string Source { get; } = "[Projects_v1]";

        public override Task<IEnumerable<ProjectInfo>> GetByOptionsAsync<TOptions>(TOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
