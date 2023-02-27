using Rubberduck.RPC.Platform.Model.LocalDb;
using Rubberduck.Server.LocalDb.Internal.Storage.Abstract;
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

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

        public override Task<IEnumerable<ProjectInfo>> GetByOptionsAsync<TOptions>(TOptions options)
        {
            throw new NotImplementedException();
        }
    }
}
