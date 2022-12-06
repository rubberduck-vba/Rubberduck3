namespace Rubberduck.Parsing.Model
{
    public abstract class TypedNamedOffsetInfo : NamedOffsetInfo
    {
        protected TypedNamedOffsetInfo(string name, DocumentOffset offset, string typeName, bool hasExplicitType) 
            : base(name, offset)
        {
            TypeName = typeName;
            HasExplicitType = hasExplicitType;
        }

        public string TypeName { get; }
        public bool HasExplicitType { get; }
    }
}
