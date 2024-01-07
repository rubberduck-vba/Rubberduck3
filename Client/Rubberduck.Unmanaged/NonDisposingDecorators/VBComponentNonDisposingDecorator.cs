﻿using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.Model;
using Rubberduck.Unmanaged.Model.Abstract;

namespace Rubberduck.Unmanaged.NonDisposingDecorators
{
    public class VBComponentNonDisposingDecorator<T> : NonDisposingDecoratorBase<T>, IVBComponent
        where T : IVBComponent
    {
        public VBComponentNonDisposingDecorator(T component)
            : base(component)
        { }

        public bool Equals(IVBComponent other)
        {
            return WrappedItem.Equals(other);
        }

        public ComponentType Type => WrappedItem.Type;

        public bool HasCodeModule => WrappedItem.HasCodeModule;

        public ICodeModule CodeModule => WrappedItem.CodeModule;

        public IVBE VBE => WrappedItem.VBE;

        public IVBComponents Collection => WrappedItem.Collection;

        public IProperties Properties => WrappedItem.Properties;

        public IControls Controls => WrappedItem.Controls;

        public IControls SelectedControls => WrappedItem.SelectedControls;

        public bool IsSaved => WrappedItem.IsSaved;

        public bool HasDesigner => WrappedItem.HasDesigner;

        public bool HasOpenDesigner => WrappedItem.HasOpenDesigner;

        public string DesignerId => WrappedItem.DesignerId;

        public string Name
        {
            get => WrappedItem.Name;
            set => WrappedItem.Name = value;
        }

        public IWindow DesignerWindow()
        {
            return WrappedItem.DesignerWindow();
        }

        public void Activate()
        {
            WrappedItem.Activate();
        }

        public void Export(string path)
        {
            WrappedItem.Export(path);
        }

        public string ExportAsSourceFile(string folder, bool isTempFile = false, bool specialCaseDocumentModules = true)
        {
            return WrappedItem.ExportAsSourceFile(folder, isTempFile, specialCaseDocumentModules);
        }

        public int FileCount => WrappedItem.FileCount;

        public string GetFileName(short index)
        {
            return WrappedItem.GetFileName(index);
        }

        public IVBProject ParentProject => WrappedItem.ParentProject;

        public int ContentHash()
        {
            return WrappedItem.ContentHash();
        }

        public IQualifiedModuleName QualifiedModuleName => WrappedItem.QualifiedModuleName;
        public string SafeName { get; }
    }
}