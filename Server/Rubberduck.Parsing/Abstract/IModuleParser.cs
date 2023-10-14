using Antlr4.Runtime;
using Rubberduck.Parsing.Model;
using Rubberduck.Unmanaged.Model;

namespace Rubberduck.Parsing.Abstract;

public interface IModuleParser
{
    ModuleParseResults Parse(QualifiedModuleName module, CancellationToken cancellationToken, TokenStreamRewriter rewriter = null);
}

public interface IQuickModuleParser 
{
    ModuleParseResults QuickParse(QualifiedModuleName module, CancellationToken cancellationToken, TokenStreamRewriter rewriter = null);
}
