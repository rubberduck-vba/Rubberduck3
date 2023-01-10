using System.IO;

namespace Rubberduck.VBEditor.SourceCodeHandling
{
    /// <summary>
    /// An object that can rewrite a module's contents.
    /// </summary>
    public interface ISourceCodeHandler<TContent> : ISourceCodeProvider<TContent>
    {
        /// <summary>
        /// Replaces the entire module's contents with the specified code.
        /// </summary>
        void SubstituteCode(QualifiedModuleName module, string newCode);
        /// <summary>
        /// Replaces one or more specific line(s) in the specified module.
        /// </summary>
        void SubstituteCode(QualifiedModuleName module, CodeString newCode);
        void SetSelection(QualifiedModuleName module, Selection selection);
        CodeString GetCurrentLogicalLine(QualifiedModuleName module);
    }

    /// <summary>
    /// An object that can manipulate the code in the AvalonEdit editor.
    /// </summary>
    public interface IEditorSourceCodeHandler : ISourceCodeHandler<TextReader>
    {
    }

    /// <summary>
    /// An object that can manipulate the code in a CodePane.
    /// </summary>
    public interface ICodePaneHandler : ISourceCodeHandler<string>
    {
        CodeString Prettify(QualifiedModuleName module, CodeString original);
    }
}
