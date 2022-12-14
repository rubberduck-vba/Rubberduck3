using Rubberduck.Parsing.Model;

namespace Rubberduck.Parsing.Exceptions
{
    public class ParsePassErrorListenerBase : RubberduckParseErrorListenerBase
    {
        protected string ModuleName { get; }

        public ParsePassErrorListenerBase(string moduleName, CodeKind codeKind) 
        :base(codeKind)
        {
            ModuleName = moduleName;
        }
    }
}
