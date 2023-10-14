using Rubberduck.VBEditor;
using System.Collections.Generic;
using System.Linq;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Annotations;
using Rubberduck.InternalApi.Model;

namespace Rubberduck.Parsing.Model.Symbols
{
    public sealed class ExternalProcedureDeclaration : Declaration, IParameterizedDeclaration
    {
        private readonly List<ParameterDeclaration> _parameters;

        public ExternalProcedureDeclaration(
            QualifiedMemberName name,
            Declaration parent,
            Declaration parentScope,
            DeclarationType declarationType,
            string asTypeName,
            VBAParser.AsTypeClauseContext asTypeContext,
            Accessibility accessibility,
            DocumentOffset offset,
            bool isUserDefined,
            IEnumerable<IParseTreeAnnotation> annotations,
            Attributes attributes)
            : base(
                  name,
                  parent,
                  parentScope,
                  asTypeName,
                  null,
                  false,
                  false,
                  accessibility,
                  declarationType,
                  offset,
                  false,
                  asTypeContext.AS() != null,
                  isUserDefined,
                  annotations,
                  attributes)
        {
            _parameters = new List<ParameterDeclaration>();
        }

        public IReadOnlyList<ParameterDeclaration> Parameters => _parameters.ToList();

        public void AddParameter(ParameterDeclaration parameter)
        {
            _parameters.Add(parameter);
        }
    }
}
