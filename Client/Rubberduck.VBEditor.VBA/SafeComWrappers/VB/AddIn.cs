﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using VB = Microsoft.Vbe.Interop;

// ReSharper disable once CheckNamespace - Special dispensation due to conflicting file vs namespace priorities
namespace Rubberduck.VBEditor.SafeComWrappers.VBA
{
    public class AddIn : SafeComWrapper<VB.AddIn>, IAddIn
    {
        // this one is an index, not an ID:
        private const int MenuBar = 1;

        private const int WindowMenu = 30009;
        private const int ListProperties = 2529;
        private const int ProjectProperties = 2578;
        private const int ViewCode = 2558;

        public AddIn(VB.AddIn target, bool rewrapping = false) 
            : base(target, rewrapping)
        {
            CommandBarLocations = new ReadOnlyDictionary<CommandBarSite, CommandBarLocation>(new Dictionary<CommandBarSite, CommandBarLocation>
            {
                {CommandBarSite.MenuBar, new CommandBarLocation(MenuBar, WindowMenu)},
                {CommandBarSite.CodePaneContextMenu, new CommandBarLocation(VbeCommandBarMenuNames.CodePaneContext, ListProperties)},
                {CommandBarSite.ProjectExplorerContextMenu, new CommandBarLocation(VbeCommandBarMenuNames.ProjectExplorerContext, ProjectProperties)},
                {CommandBarSite.FormDesignerContextMenu, new CommandBarLocation(VbeCommandBarMenuNames.FormDesignerContext, ViewCode)},
                {CommandBarSite.FormDesignerControlContextMenu, new CommandBarLocation(VbeCommandBarMenuNames.FormDesignerControlContext, ViewCode)},
            });
        }


        public IReadOnlyDictionary<CommandBarSite, CommandBarLocation> CommandBarLocations { get; }

        public string ProgId => IsWrappingNullReference ? string.Empty : Target.ProgId;

        public string Guid => IsWrappingNullReference ? string.Empty : Target.Guid;

        public IVBE VBE => new VBE(IsWrappingNullReference ? null : Target.VBE);

        public IAddIns Collection => new AddIns(IsWrappingNullReference ? null : Target.Collection);

        public string Description
        {
            get => IsWrappingNullReference ? string.Empty : Target.Description;
            set
            {
                if (!IsWrappingNullReference)
                {
                    Target.Description = value;
                }
            }
        }

        public bool Connect
        {
            get => !IsWrappingNullReference && Target.Connect;
            set
            {
                if (!IsWrappingNullReference)
                {
                    Target.Connect = value;
                }
            }
        }

        public object Object // definitely leaks a COM object
        {
            get => IsWrappingNullReference ? null : Target.Object;
            set
            {
                if (!IsWrappingNullReference)
                {
                    Target.Object = value;
                }
            }
        }

        public override bool Equals(ISafeComWrapper<VB.AddIn> other)
        {
            return IsEqualIfNull(other) || (other != null && other.Target.ProgId == ProgId && other.Target.Guid == Guid);
        }

        public bool Equals(IAddIn other)
        {
            return Equals(other as SafeComWrapper<VB.AddIn>);
        }

        public override int GetHashCode()
        {
            return IsWrappingNullReference ? 0 : HashCode.Combine(ProgId, Guid);
        }

        protected override void Dispose(bool disposing) => base.Dispose(disposing);
    }
}