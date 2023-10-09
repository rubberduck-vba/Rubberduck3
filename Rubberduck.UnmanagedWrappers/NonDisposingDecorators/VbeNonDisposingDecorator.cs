using Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB.Enums;
using Rubberduck.Unmanaged.Abstract.SourceCodeProvider;
using Rubberduck.Unmanaged.Model;

namespace Rubberduck.Unmanaged.NonDisposingDecorators
{
    public class VbeNonDisposingDecorator<T> : NonDisposingDecoratorBase<T>, IVBE
        where T : IVBE
    {
        public VbeNonDisposingDecorator(T vbe)
            : base(vbe)
        { }

        public bool Equals(IVBE other)
        {
            return WrappedItem.Equals(other);
        }

        public VBEKind Kind => WrappedItem.Kind;
        public string Version => WrappedItem.Version;
        public object HardReference => WrappedItem.HardReference;
        public IWindow ActiveWindow => WrappedItem.ActiveWindow;

        public ICodePane ActiveCodePane
        {
            get => WrappedItem.ActiveCodePane;
            set => WrappedItem.ActiveCodePane = value;
        }

        public IVBProject ActiveVBProject
        {
            get => WrappedItem.ActiveVBProject;
            set => WrappedItem.ActiveVBProject = value;
        }

        public IVBComponent SelectedVBComponent => WrappedItem.SelectedVBComponent;
        public IWindow MainWindow => WrappedItem.MainWindow;
        public IAddIns AddIns => WrappedItem.AddIns;
        public IVBProjects VBProjects => WrappedItem.VBProjects;
        public ICodePanes CodePanes => WrappedItem.CodePanes;
        public ICommandBars CommandBars => WrappedItem.CommandBars;
        public IWindows Windows => WrappedItem.Windows;

        public IHostApplication HostApplication()
        {
            return WrappedItem.HostApplication();
        }

        public IWindow ActiveMDIChild()
        {
            return WrappedItem.ActiveMDIChild();
        }

        public QualifiedSelection? GetActiveSelection()
        {
            return WrappedItem.GetActiveSelection();
        }

        public bool IsInDesignMode => WrappedItem.IsInDesignMode;

        public int ProjectsCount => WrappedItem.ProjectsCount;

        public ITempSourceFileHandler TempSourceFileHandler => WrappedItem.TempSourceFileHandler;
    }
}