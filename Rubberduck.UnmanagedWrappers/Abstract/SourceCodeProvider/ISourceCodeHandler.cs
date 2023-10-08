using Rubberduck.Unmanaged.Abstract;
using Rubberduck.Unmanaged.Model;
using Rubberduck.Unmanaged.Model.Abstract;
using System.IO;

namespace Rubberduck.Unmanaged.Abstract.SourceCodeProvider
{
    /// <summary>
    /// An object that can rewrite a module's contents.
    /// </summary>
    public interface ISourceCodeHandler<TContent> : ISourceCodeProvider<TContent>
    {
        /// <summary>
        /// Replaces the entire module's contents with the specified code.
        /// </summary>
        void SubstituteCode(IQualifiedModuleName module, string newCode);
        /// <summary>
        /// Replaces one or more specific line(s) in the specified module.
        /// </summary>
        void SubstituteCode(IQualifiedModuleName module, CodeString newCode);
        void SetSelection(IQualifiedModuleName module, Selection selection);
        CodeString GetCurrentLogicalLine(IQualifiedModuleName module);
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
        CodeString Prettify(IQualifiedModuleName module, CodeString original);
    }
}
