using System;

namespace Rubberduck.Parsing.Model.Symbols
{
    public class MemberProcessedEventArgs : EventArgs
    {
        private readonly string _name;

        public MemberProcessedEventArgs(string name)
        {
            _name = name;
        }

        public string MemberName { get { return _name; } }
    }
}
