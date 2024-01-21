using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using Rubberduck.InternalApi.Model.Declarations;
using Rubberduck.Parsing.Grammar;

namespace Rubberduck.Parsing.Annotations;

public interface IAnnotationFactory
{
    IParseTreeAnnotation Create(VBAParser.AnnotationContext context, Location location);
}
