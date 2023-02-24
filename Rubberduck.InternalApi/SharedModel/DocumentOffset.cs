using System;

namespace Rubberduck.InternalApi.Model
{

    public readonly struct DocumentOffset : IEquatable<DocumentOffset>, IComparable<DocumentOffset>
    {
        public static DocumentOffset Invalid { get; } = new DocumentOffset(0, -1);

        public DocumentOffset(int start, int end)
        {
            Start = start;
            End = end;
            Length = end - start + 1;
        }

        public int Start { get; }
        public int End { get; }
        public int Length { get; }

        public int CompareTo(DocumentOffset other)
        {
            if (Start < other.Start || End < other.End)
            {
                return -1;
            }

            if (Start > other.End || End > other.End)
            {
                return 1;
            }

            return 0;
        }

        public bool Contains(DocumentOffset other)
        {
            return other.Start >= Start && other.End <= End;
        }

        public bool Equals(DocumentOffset other)
        {
            return other.Start == Start && other.End == End;
        }

        public override bool Equals(object obj) => Equals((DocumentOffset)obj);

        public override int GetHashCode() => HashCode.Combine(Start, End);
    }
}
