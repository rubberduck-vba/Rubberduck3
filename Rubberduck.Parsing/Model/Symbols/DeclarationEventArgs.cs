using System;

namespace Rubberduck.Parsing.Model.Symbols
{
    public class DeclarationEventArgs : EventArgs
    {
        private readonly Declaration _declaration;

        public DeclarationEventArgs(Declaration declaration)
        {
            _declaration = declaration;
        }

        public Declaration Declaration { get { return _declaration; } }
    }
}
