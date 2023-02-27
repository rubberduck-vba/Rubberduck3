﻿extern alias Office_v8;
using Rubberduck.VBEditor.SafeComWrappers.Abstract;
using Rubberduck.VBEditor.SafeComWrappers.Office;
using System;
using MSO = Office_v8::Office;

// ReSharper disable once CheckNamespace - Special dispensation due to conflicting file vs namespace priorities
namespace Rubberduck.VBEditor.SafeComWrappers.Office8
{
    public sealed class CommandBar : SafeComWrapper<MSO.CommandBar>, ICommandBar
    {
        private readonly IVBE _vbe;

        public CommandBar(MSO.CommandBar target, IVBE vbe, bool rewrapping = false)
            : base(target, rewrapping)
        {
            _vbe = vbe;
        }

        public int Id => -1; // Office v8 does not have this property.

        public bool IsBuiltIn => !IsWrappingNullReference && Target.BuiltIn;

        public ICommandBarControls Controls => new CommandBarControls(IsWrappingNullReference ? null : Target.Controls, _vbe);

        public bool IsEnabled
        {
            get => !IsWrappingNullReference && Target.Enabled;
            set
            {
                if (!IsWrappingNullReference)
                {
                    Target.Enabled = value;
                }
            }
        }

        public int Height
        {
            get => IsWrappingNullReference ? 0 : Target.Height;
            set
            {
                if (!IsWrappingNullReference)
                {
                    Target.Height = value;
                }
            }
        }

        public int Index => IsWrappingNullReference ? 0 : Target.Index;

        public int Left
        {
            get => IsWrappingNullReference ? 0 : Target.Left;
            set
            {
                if (!IsWrappingNullReference)
                {
                    Target.Left = value;
                }
            }
        }

        public string Name
        {
            get => IsWrappingNullReference ? string.Empty : Target.Name;
            set
            {
                if (!IsWrappingNullReference)
                {
                    Target.Name = value;
                }
            }
        }

        public CommandBarPosition Position
        {
            get => IsWrappingNullReference ? 0 : (CommandBarPosition) Target.Position;
            set
            {
                if (!IsWrappingNullReference)
                {
                    Target.Position = (MSO.MsoBarPosition) value;
                }
            }
        }

        public int Top
        {
            get => IsWrappingNullReference ? 0 : Target.Top;
            set
            {
                if (!IsWrappingNullReference)
                {
                    Target.Top = value;
                }
            }
        }

        public CommandBarType Type => IsWrappingNullReference ? 0 : (CommandBarType) Target.Type;

        public bool IsVisible
        {
            get => !IsWrappingNullReference && Target.Visible;
            set
            {
                if (!IsWrappingNullReference)
                {
                    Target.Visible = value;
                }
            }
        }

        public int Width
        {
            get => IsWrappingNullReference ? 0 : Target.Width;
            set
            {
                if (!IsWrappingNullReference)
                {
                    Target.Width = value;
                }
            }
        }

        public ICommandBarControl FindControl(int id)
        {
            return new CommandBarControl(IsWrappingNullReference ? null : Target.FindControl(Id: id), _vbe);
        }

        public ICommandBarControl FindControl(ControlType type, int id)
        {
            return new CommandBarControl(IsWrappingNullReference ? null : Target.FindControl(type, id), _vbe);
        }

        public void Delete()
        {
            if (!IsWrappingNullReference)
            {
                Target.Delete();
            }
        }

        public override bool Equals(ISafeComWrapper<MSO.CommandBar> other)
        {
            return IsEqualIfNull(other) ||
                   (other != null
                    && (int) other.Target.Type == (int) Type
                    && other.Target.Index == Index
                    && other.Target.BuiltIn == IsBuiltIn
                    && ReferenceEquals(other.Target.Parent, Target.Parent));
        }

        public bool Equals(ICommandBar other)
        {
            return Equals(other as SafeComWrapper<MSO.CommandBar>);
        }

        public override int GetHashCode()
        {
            return IsWrappingNullReference ? 0 : HashCode.Combine(Type, Id, Index, IsBuiltIn, Target.Parent);
        }

        protected override void Dispose(bool disposing) => base.Dispose(disposing);
    }
}
