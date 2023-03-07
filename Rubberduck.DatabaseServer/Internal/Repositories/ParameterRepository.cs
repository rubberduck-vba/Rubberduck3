using System.Data;
using Dapper;
using Rubberduck.DatabaseServer.Internal.Abstract;
using Rubberduck.RPC.Platform.Model.Database;

namespace Rubberduck.DatabaseServer.Internal.Storage
{
    internal class ParameterRepository : Repository<Parameter>
    {
        public ParameterRepository(IDbConnection connection) 
            : base(connection) { }

        protected override string Source { get; } = "[Parameters]";
        protected override string[] ColumnNames { get; } = new[]
        { 
            "DeclarationId",
            "Position",
            "IsParamArray",
            "IsOptional",
            "IsByRef",
            "IsByVal",
            "IsModifierImplicit",
            "DefaultValue",
        };

        public override async Task SaveAsync(Parameter entity)
        {
            if (entity.Id == default)
            {
                entity.Id = await Database.ExecuteAsync(InsertSql, new 
                    {
                        declarationId = entity.DeclarationId,
                        position = entity.Position,
                        isParamArray = entity.IsParamArray,
                        isOptional = entity.IsOptional,
                        isByRef = entity.IsByRef,
                        isByVal = entity.IsByVal,
                        isModifierImplicit = entity.IsModifierImplicit,
                        defaultValue = entity.DefaultValue,
                    });
            }
            else
            {
                await Database.ExecuteAsync(UpdateSql, new 
                    {
                        declarationId = entity.DeclarationId,
                        position = entity.Position,
                        isParamArray = entity.IsParamArray,
                        isOptional = entity.IsOptional,
                        isByRef = entity.IsByRef,
                        isByVal = entity.IsByVal,
                        isModifierImplicit = entity.IsModifierImplicit,
                        defaultValue = entity.DefaultValue,
                        id = entity.Id,
                    });
            }
        }
    }
}
