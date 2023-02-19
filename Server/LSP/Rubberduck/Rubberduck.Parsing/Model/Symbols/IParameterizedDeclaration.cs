using System.Collections.Generic;

namespace Rubberduck.Parsing.Model.Symbols
{
    public interface IParameterizedDeclaration
    {
        IReadOnlyList<ParameterDeclaration> Parameters { get; }
        void AddParameter(ParameterDeclaration parameter);
    }
}
