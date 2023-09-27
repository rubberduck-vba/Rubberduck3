using System;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office;
using Rubberduck.Unmanaged.Abstract.SourceCodeProvider;
using Rubberduck.Unmanaged.Model;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers
{
    // ReSharper disable once InconsistentNaming
    public interface IVBE : ISafeComWrapper, IEquatable<IVBE>
    {
        VBEKind Kind { get; }
        string Version { get; }
        object HardReference { get; }
        IWindow ActiveWindow { get; }
        ICodePane ActiveCodePane { get; set; }
        IVBProject ActiveVBProject { get; set; }
        IVBComponent SelectedVBComponent { get; }
        IWindow MainWindow { get; }
        IAddIns AddIns { get; }
        IVBProjects VBProjects { get; }
        ICodePanes CodePanes { get; }
        ICommandBars CommandBars { get; }
        IWindows Windows { get; }
        IHostApplication HostApplication();
        IWindow ActiveMDIChild();
        QualifiedSelection? GetActiveSelection();

        bool IsInDesignMode { get; }
        int ProjectsCount { get; }
        
        ITempSourceFileHandler TempSourceFileHandler { get; }
    }
}