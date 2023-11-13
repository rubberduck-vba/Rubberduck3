﻿using Dragablz;
using Rubberduck.InternalApi.Model.Abstract;
using System;

namespace Rubberduck.Editor.Shell
{
    public class ChildWindowViewModel : IWindowViewModel
    {
        public ChildWindowViewModel(IInterTabClient interTabClient, StatusBarViewModel statusBar, object partition)
        {
            InterTabClient = interTabClient;
            Partition = partition;

            StatusBar = statusBar;
            Title = "Rubberduck Editor"; // tab title?
        }

        public string Title { get; }
        public StatusBarViewModel StatusBar { get; }

        public object /*IInterTabClient*/ InterTabClient { get; }

        public object Partition { get; }

        public void ClosingTabItemHandler(object sender, EventArgs e)
        {
            throw new NotImplementedException();
        }
    }
}
