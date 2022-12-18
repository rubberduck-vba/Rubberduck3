namespace Rubberduck.InternalApi.Model
{

    public readonly struct DocumentOffset
    {
        public static DocumentOffset Invalid = new DocumentOffset(0, -1);

        public DocumentOffset(int start, int end)
        {
            Start = start;
            End = end;
            Length = end - start + 1;
        }

        public int Start { get; }
        public int End { get; }
        public int Length { get; }

        public bool Contains(DocumentOffset other)
        {
            return other.Start >= Start && other.End <= End;
        }
    }
}
