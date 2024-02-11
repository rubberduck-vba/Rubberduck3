using Antlr4.Runtime.Misc;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Parsing.Abstract;
using Rubberduck.Parsing.Annotations.Concrete;
using Rubberduck.Parsing.Grammar;
using Rubberduck.Parsing.Model;
using System.IO.Abstractions;

namespace Rubberduck.Parsing._v3
{
    /// <summary>
    /// A service that processes <c>@Folder</c> annotations in module declarations into workspace folders, effectively migrating an existing Rubberduck VBA project to RD3.
    /// </summary>
    /// <remarks>
    /// <c>@Folder</c> annotations are no longer needed in RD3, since we have <em>actual</em> folders now.
    /// </remarks>
    public class WorkspaceFolderMigrationService
    {
        private readonly IFileSystem _fileSystem;
        private readonly IParser<string> _parser;

        public WorkspaceFolderMigrationService(IFileSystem fileSystem, IParser<string> parser) 
        {
            _fileSystem = fileSystem;
            _parser = parser;
        }

        /// <summary>
        /// Gets a URI for a project component / module, given the content of its <em>declarations</em> section.
        /// </summary>
        /// <param name="workspaceUri">The root URI of the project associated with the workspace.</param>
        /// <param name="fileName">The file name of the module, including its extension.</param>
        /// <param name="declarations">The string content of the <em>declaration lines</em> header section of the module (nothing more is needed).</param>
        /// <returns>A URI under the workspace source root for the specified module, respective of any <c>@Folder</c> annotation found in its <em>declarations</em> header section.</returns>
        public WorkspaceFileUri ParseModuleUri(Uri workspaceRoot, string fileName, string declarations)
        {
            var uri = fileName;
            var name = _fileSystem.Path.GetFileNameWithoutExtension(fileName);

            var listener = new FolderAnnotationsListener();
            var result = _parser.Parse(new WorkspaceFileUri(uri, workspaceRoot), declarations, CancellationToken.None, parseListeners: [listener]);

            if (listener.Folder != null)
            {
                var folder = listener.Folder.Replace(FolderAnnotation.SeparatorChar, _fileSystem.Path.DirectorySeparatorChar);
                uri = _fileSystem.Path.Combine(_fileSystem.Path.DirectorySeparatorChar + folder, fileName);
            }

            return new WorkspaceFileUri(uri, workspaceRoot);
        }

        private class FolderAnnotationsListener : VBAParserBaseListener
        {
            public string? Folder { get; private set; } = null;

            public override void ExitAnnotation([NotNull] VBAParser.AnnotationContext context)
            {
                if (string.Equals(context.annotationName()?.GetText(), "Folder", StringComparison.InvariantCultureIgnoreCase))
                {
                    var arg = context.annotationArgList()?.annotationArg().FirstOrDefault();
                    if (arg != null)
                    {
                        var folderName = arg.GetText()?.Trim().UnQuote();
                        if (!string.IsNullOrWhiteSpace(folderName))
                        {
                            Folder = folderName;
                        }
                    }
                }
            }
        }
    }
}
