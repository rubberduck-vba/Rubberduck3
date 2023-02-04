﻿using System.Data;
using Rubberduck.Server.LocalDb.Internal;
using Rubberduck.DataServer.Storage.Entities;

namespace Rubberduck.DataServer.Storage.Views
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