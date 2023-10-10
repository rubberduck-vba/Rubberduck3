using System.Collections.Generic;
using Rubberduck.Parsing.Model.ComReflection;
using Rubberduck.Parsing.Model.Symbols;

namespace Rubberduck.Parsing.COM.Abstract
{
    public interface IDeclarationsFromComProjectLoader
    {
        IReadOnlyCollection<Declaration> LoadDeclarations(ComProject type, string projectId = null);
    }
}