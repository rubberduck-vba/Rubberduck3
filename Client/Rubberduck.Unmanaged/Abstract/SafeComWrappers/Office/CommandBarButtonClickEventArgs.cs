﻿using System;

namespace Rubberduck.Unmanaged.Abstract.SafeComWrappers.Office
{
    public class CommandBarButtonClickEventArgs : EventArgs
    {
        public CommandBarButtonClickEventArgs(ICommandBarButton control)
        {
            Tag = control.Tag;
            Caption = control.Caption;
            TargetHashCode = control.Target.GetHashCode();
        }

        public string Tag { get; }
        public string Caption { get; }
        public int TargetHashCode { get; }

        public bool Cancel { get; set; }
        public bool Handled { get; set; } // Only used in VB6
    }
}