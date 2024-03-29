﻿using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Abstract;

namespace Rubberduck.Parsing._v3.Pipeline;

public class PipelineParseResult
{
    public WorkspaceFileUri Uri { get; init; } = null!;
    public ParseResult ParseResult { get; init; } = null!;
}
