using Rubberduck.Unmanaged.Abstract;
using Rubberduck.Unmanaged.Model.Abstract;
using System;

namespace Rubberduck.Unmanaged.Model
{
    public readonly struct QualifiedSelection : IComparable<QualifiedSelection>, IEquatable<QualifiedSelection>
    {
        public QualifiedSelection(IQualifiedModuleName qualifiedName, Selection selection)
        {
            QualifiedModuleName = qualifiedName;
            Selection = selection;            
        }

        public IQualifiedModuleName QualifiedModuleName { get; }

        public Selection Selection { get; }

        public bool Contains(QualifiedSelection other)
        {
            return QualifiedModuleName.Equals(other.QualifiedModuleName) && Selection.Contains(other.Selection);
        }

        public int CompareTo(QualifiedSelection other)
        {
            return other.QualifiedModuleName.Equals(QualifiedModuleName)
                ? Selection.CompareTo(other.Selection)
                : string.Compare(QualifiedModuleName.ToString(), other.QualifiedModuleName.ToString(), StringComparison.Ordinal);
        }

        public bool Equals(QualifiedSelection other)
        {
            return other.Selection.Equals(Selection) && other.QualifiedModuleName.Equals(QualifiedModuleName);
        }

        public override string ToString()
        {
            return $"{QualifiedModuleName} {Selection}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(QualifiedModuleName.GetHashCode(), Selection.GetHashCode());
        }

        public static bool operator ==(QualifiedSelection selection1, QualifiedSelection selection2)
        {
            return selection1.Equals(selection2);
        }

        public static bool operator !=(QualifiedSelection selection1, QualifiedSelection selection2)
        {
            return !(selection1 == selection2);
        }

        public override bool Equals(object obj)
        {
            if (obj is QualifiedSelection qualifiedSelection)
            {
                return Equals(qualifiedSelection);
            }
            return false;
        }
    }
}
