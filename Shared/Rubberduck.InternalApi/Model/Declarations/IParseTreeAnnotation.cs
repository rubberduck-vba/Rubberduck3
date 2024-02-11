using OmniSharp.Extensions.LanguageServer.Protocol.Models;
using System.Collections.Generic;

namespace Rubberduck.InternalApi.Model.Declarations;

public interface IParseTreeAnnotation
{
    // needs to be accessible to all external consumers
    IAnnotation Annotation { get; }
    IReadOnlyList<string> AnnotationArguments { get; }

    // needs to be accessible to IllegalAnnotationInspection
    int? AnnotatedLine { get; }
    Location Location { get; }
}