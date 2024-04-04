namespace Rubberduck.Unmanaged.Model
{
    public class QualifiedMemberName
    {
        private QualifiedModuleName qualifiedModuleName;
        private string member;

        public QualifiedMemberName(QualifiedModuleName qualifiedModuleName, string member)
        {
            this.qualifiedModuleName = qualifiedModuleName;
            this.member = member;
        }
    }
}