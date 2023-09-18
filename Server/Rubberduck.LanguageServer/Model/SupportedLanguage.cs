namespace Rubberduck.LanguageServer.Model
{
    public abstract class SupportedLanguage
    {
        protected SupportedLanguage(string id, string name)
        {
            Id = id;
            Name = name;
        }

        public string Id { get; }
        public string Name { get; }

        public abstract string[] FileTypes { get; }
    }

    public class VisualBasicLanguage : SupportedLanguage
    {
        public const string LanguageId = "vb6";
        public const string LanguageName = "Visual Basic 6.0";

        public VisualBasicLanguage() : base(LanguageId, LanguageName) { }

        public override string[] FileTypes { get; } = new[]
        {
            "*.vbp",
            "*.bas",
            "*.cls",
            "*.doccls",
            "*.frm",
            //...
        };
    }

    public class VisualBasicForApplicationsLanguage : SupportedLanguage
    {
        public const string LanguageId = "vba";
        public const string LanguageName = "Visual Basic for Applications";

        public VisualBasicForApplicationsLanguage() : base(LanguageId, LanguageName) { }

        public override string[] FileTypes { get; } = new[]
        {
            "*.bas",
            "*.cls",
            "*.doccls",
            "*.frm",
            //...
        };
    }
}