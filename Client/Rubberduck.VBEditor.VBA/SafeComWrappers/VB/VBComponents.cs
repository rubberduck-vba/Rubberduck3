using System;
using System.Collections;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.Unmanaged;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers;
using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.Events;
using Rubberduck.Unmanaged.Model;
using Rubberduck.VBEditor.Utility;
using VB = Microsoft.Vbe.Interop;

// ReSharper disable once CheckNamespace - Special dispensation due to conflicting file vs namespace priorities
namespace Rubberduck.VBEditor.SafeComWrappers.VBA
{
    public sealed class VBComponents : SafeEventedComWrapper<VB.VBComponents, VB._dispVBComponentsEvents>, IVBComponents //, VB._dispVBComponentsEvents
    {
        private readonly IFileSystem _fileSystem = FileSystemProvider.FileSystem;
        private readonly IModuleNameFromFileExtractor _moduleNameFromFileExtractor = new ModuleNameFromFileExtractor();

        public VBComponents(VB.VBComponents target, bool rewrapping = false) 
            : base(target, rewrapping)
        {
        }

        public int Count => IsWrappingNullReference ? 0 : Target.Count;
        public IVBProject Parent => new VBProject((IsWrappingNullReference ? null : Target.Parent)!);
        public IVBE VBE => new VBE((IsWrappingNullReference ? null : Target.VBE)!);
        public IVBComponent this[object index] => new VBComponent((IsWrappingNullReference ? null : Target.Item(index))!);

        public void Remove(IVBComponent item)
        {
            if (item?.Target != null && !IsWrappingNullReference && item.Type != ComponentType.Document)
            {
                Target.Remove((VB.VBComponent)item.Target);
            }
        }

        public IVBComponent Add(ComponentType type)
        {
            return new VBComponent((IsWrappingNullReference ? null : Target.Add((VB.vbext_ComponentType)type))!);
        }

        public IVBComponent Import(string path)
        {
            return new VBComponent((IsWrappingNullReference ? null : Target.Import(path))!);
        }

        public IVBComponent AddCustom(string progId)
        {
            return new VBComponent((IsWrappingNullReference ? null : Target.AddCustom(progId))!);
        }

        IEnumerator<IVBComponent> IEnumerable<IVBComponent>.GetEnumerator()
        {
            return new ComWrapperEnumerator<IVBComponent>(Target, comObject => new VBComponent((VB.VBComponent) comObject));
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return IsWrappingNullReference
                ? (IEnumerator) new List<IEnumerable>().GetEnumerator()
                : ((IEnumerable<IVBComponent>) this).GetEnumerator();
        }

        public override bool Equals(ISafeComWrapper<VB.VBComponents> other)
        {
            return IsEqualIfNull(other) || (other != null && ReferenceEquals(other.Target, Target));
        }

        public bool Equals(IVBComponents? other)
        {
            return Equals((other as SafeComWrapper<VB.VBComponents>)!);
        }

        public override int GetHashCode()
        {
            return IsWrappingNullReference ? 0 : HashCode.Combine(Target);
        }

        public IVBComponent ImportSourceFile(string path)
        {
            if (IsWrappingNullReference)
            {
                return null!;
            }

            var ext = _fileSystem.Path.GetExtension(path);
            if (!_fileSystem.File.Exists(path))
            {
                return null!;
            }

            switch (ext)
            {
                case ComponentTypeExtensions.FormBinaryExtension:
                    return null!;

                case ComponentTypeExtensions.DocClassExtension:
                {
                    var name = _moduleNameFromFileExtractor.ModuleName(path);
                    IVBComponent component = null!;
                    try
                    {
                        component = this[name];
                    }
                    catch
                    {
                        throw new IndexOutOfRangeException($"Could not find document component named '{name}'.");
                    }

                    var codeString = _fileSystem.File.ReadAllText(path, Encoding.UTF8);
                    using (var codeModule = component.CodeModule)
                    {
                        codeModule.Clear();
                        codeModule.AddFromString(codeString);
                        //_logger.Trace($"{nameof(VBComponents)}.{nameof(ImportSourceFile)} cleared document module '{name}'.");
                        //_logger.Warn($"File content was imported using {nameof(ICodeModule)}.{nameof(ICodeModule.AddFromString)}: attributes are lost.");
                        //_logger.Info($"Review 'MissingAttribute' inspection results to recover any attributes lost in the import process of document modules.");
                        //_logger.Info($"Review 'MissingAnnotation' inspection results to mark attributes for recovery and avoid losing them in the import process of document modules.");
                    }

                    return component;
                }

                case ComponentTypeExtensions.FormExtension:
                {
                    var name = _moduleNameFromFileExtractor.ModuleName(path);
                    IVBComponent component = null!;
                    try
                    {
                        component = this[name];
                    }
                    catch
                    {
                        component = Import(path);
                    }

                    //The VBE uses the current ANSI codepage from the windows settings to export and import.
                    var codeString = _fileSystem.File.ReadAllText(path, Encoding.Default); 
                    var codeLines = codeString.Split(new[] {Environment.NewLine}, StringSplitOptions.None);

                    var nonAttributeLines = codeLines
                            .TakeWhile(line => !line.StartsWith("Attribute")).Count();
                    var attributeLines = codeLines.Skip(nonAttributeLines)
                            .TakeWhile(line => line.StartsWith("Attribute")).Count();
                    var declarationsStartLine = nonAttributeLines + attributeLines + 1;
                    
                    var correctCodeString = string.Join(Environment.NewLine,
                    codeLines.Skip(declarationsStartLine - 1).ToArray());

                    using (var codeModule = component.CodeModule)
                    {
                        codeModule.Clear();
                        codeModule.AddFromString(correctCodeString);
                    }
                    
                    return component;
                }
                default:
                    return Import(path);
            }
        }

        public void RemoveSafely(IVBComponent component)
        {
            if (component.IsWrappingNullReference)
            {
                return;
            }

            switch (component.Type)
            {
                case ComponentType.ClassModule:
                case ComponentType.StandardModule:
                case ComponentType.UserForm:
                    Remove(component);
                    break;
                case ComponentType.ActiveXDesigner:
                case ComponentType.Document:
                    using (var codeModule = component.CodeModule)
                    {
                        codeModule.Clear();
                    }
                    break;
            }
        }

        protected override void Dispose(bool disposing) => base.Dispose(disposing);

        #region Events

        public event EventHandler<ComponentEventArgs>? ComponentAdded;
        //void VB._dispVBComponentsEvents.ItemAdded(VB.VBComponent VBComponent)
        //{
        //    OnDispatch(ComponentAdded, VBComponent);
        //}

        public event EventHandler<ComponentEventArgs>? ComponentRemoved;
        //void VB._dispVBComponentsEvents.ItemRemoved(VB.VBComponent VBComponent)
        //{
        //    OnDispatch(ComponentRemoved, VBComponent);
        //}

        public event EventHandler<ComponentRenamedEventArgs>? ComponentRenamed;
        //void VB._dispVBComponentsEvents.ItemRenamed(VB.VBComponent VBComponent, string OldName)
        //{
        //    using (var component = new VBComponent(VBComponent))
        //    {
        //        var handler = ComponentRenamed;
        //        if (handler == null)
        //        {
        //            return;
        //        }

        //        using (var components = component.Collection)
        //        using (var project = components.Parent)
        //        {
        //            if (project.Protection == ProjectProtection.Locked)
        //            {
        //                return;
        //            }
        //        }

        //        var qmn = new QualifiedModuleName(component);
        //        handler.Invoke(component, new ComponentRenamedEventArgs(qmn, OldName));
        //    }
        //}

        public event EventHandler<ComponentEventArgs>? ComponentSelected;
        //void VB._dispVBComponentsEvents.ItemSelected(VB.VBComponent VBComponent)
        //{
        //    OnDispatch(ComponentSelected, VBComponent);
        //}

        public event EventHandler<ComponentEventArgs>? ComponentActivated;
        //void VB._dispVBComponentsEvents.ItemActivated(VB.VBComponent VBComponent)
        //{
        //    OnDispatch(ComponentActivated, VBComponent);
        //}

        public event EventHandler<ComponentEventArgs>? ComponentReloaded;
        //void VB._dispVBComponentsEvents.ItemReloaded(VB.VBComponent VBComponent)
        //{
        //    OnDispatch(ComponentReloaded, VBComponent);
        //}

        //private static void OnDispatch(EventHandler<ComponentEventArgs>? dispatched, VB.VBComponent vbComponent)
        //{
        //    var handler = dispatched;
        //    if (handler is null)
        //    {
        //        return;
        //    }

        //    using var component = new VBComponent(vbComponent);
        //    using var components = component.Collection;
        //    using var project = components.Parent;
        //    if (project.Protection == ProjectProtection.Locked)
        //    {
        //        return;
        //    }

        //    var qmn = new QualifiedModuleName(component);
        //    var eventArgs = new ComponentEventArgs(qmn);
        //    handler.Invoke(component, eventArgs);
        //}

        #endregion
    }
}