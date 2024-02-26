using Microsoft.VisualBasic.FileIO;
using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Linq;

namespace Rubberduck.InternalApi.ServerPlatform.LanguageServer;

public class SupportedLanguage
{
    // Rubberduck.LanguageServer LSP server supports VBA and VB6:
    public static SupportedLanguage VBA = new("vba", "Microsoft Visual Basic for Applications", "*.bas", "*.cls", "*.frm", "*.doccls");
    public static SupportedLanguage VB6 = new("vb6", "Microsoft Visual Basic 6.0", "*.bas", "*.cls", "*.frm");

    // eventually other Rubberduck LSP servers could support editing e.g. .json or .sql files
    public static SupportedLanguage JSON = new("json", "Javascript Object Notation", "*.json", "*.rdproj");
    public static SupportedLanguage ASQL = new("asql", "Microsoft Access SQL", "*.sql");


    private SupportedLanguage(string id, string name, params string[] fileTypes)
    {
        Id = id;
        Name = name;
        FileTypes = fileTypes;
    }

    public string Id { get; }
    public string Name { get; }
    public string[] FileTypes { get; }

    public string FilterString => string.Join(";", FileTypes.Select(fileType => $"**/{fileType}").ToArray());
    public TextDocumentSelector ToTextDocumentSelector() => new(
        new TextDocumentFilter
        {
            Language = Id,
            Pattern = FilterString,
        });
}