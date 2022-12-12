using Rubberduck.CodeAnalysis.Abstract;
using Rubberduck.Parsing;
using Rubberduck.VBEditor;
using System.Xml.Linq;

namespace Rubberduck.CodeAnalysis
{
    public enum CodeInspectionType
    {
        Uncategorized,
        RubberduckOpportunities,
        LanguageOpportunities,
        NamingAndConventionsIssues,
        CodeQualityIssues,
        Performance,
    }
}
