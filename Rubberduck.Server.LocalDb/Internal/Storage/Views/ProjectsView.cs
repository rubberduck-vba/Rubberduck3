using Rubberduck.Server.LocalDb.Internal.Model;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using System.Data;

namespace Rubberduck.Server.LocalDb.Internal.Storage
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
    }
}
