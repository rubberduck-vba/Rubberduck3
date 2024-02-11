namespace Rubberduck.InternalApi.ServerPlatform.LanguageServer;

public class SupportedLanguage
{
    public static SupportedLanguage VBA = new("vba", "Microsoft Visual Basic for Applications", "*.bas", "*.cls", "*.frm", "*.doccls");
    public static SupportedLanguage VB6 = new("vb6", "Microsoft Visual Basic 6.0", "*.bas", "*.cls", "*.frm");

    private SupportedLanguage(string id, string name, params string[] fileTypes)
    {
        Id = id;
        Name = name;
        FileTypes = fileTypes;
    }

    public string Id { get; }
    public string Name { get; }

    public string[] FileTypes { get; }
}