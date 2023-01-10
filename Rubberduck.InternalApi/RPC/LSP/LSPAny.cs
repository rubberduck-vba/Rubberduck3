namespace Rubberduck.InternalApi.RPC.LSP
{
    public class LSPAny
    {
        public LSPAny(LSPObject value)
        {
            Value = value;
        }

        public LSPAny(LSPArray value)
        {
            Value = value;
        }

        public LSPAny(string value)
        {
            Value = value;
        }

        public LSPAny(int value)
        {
            Value = value;
        }

        public LSPAny(uint value)
        {
            Value = value;
        }

        public LSPAny(decimal value)
        {
            Value = value;
        }

        public LSPAny(bool value)
        {
            Value = value;
        }

        public LSPAny()
        {
            Value = null;
        }

        public object Value { get; set; }
    }
}
