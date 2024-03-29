using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB.Enums;
using Rubberduck.Unmanaged.Model;
using Rubberduck.Unmanaged.Model.Abstract;
using System;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB
{
    public interface ICodePane : ISafeComWrapper, IEquatable<ICodePane>
    {
        IVBE VBE { get; }
        ICodePanes Collection { get; }
        IWindow Window { get; }
        int TopLine { get; set; }
        int CountOfVisibleLines { get; }
        ICodeModule CodeModule { get; }
        CodePaneView CodePaneView { get; }
        /// <summary>
        /// Gets or sets a 1-based <see cref="Selection"/> representing the current selection in the code pane.
        /// </summary>
        Selection Selection { get; set; }
        QualifiedSelection? GetQualifiedSelection();
        IQualifiedModuleName QualifiedModuleName { get; }
        void Show();
    }
}