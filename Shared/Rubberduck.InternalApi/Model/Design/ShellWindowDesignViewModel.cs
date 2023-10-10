using Rubberduck.InternalApi.Model.Abstract;
using Rubberduck.Resources;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

///<summary>
///This file provides design-time data for the <see cref="EditorShellControl.xaml"/> control.
///This is only to support working in the XAML designer and nothing in this file should be used otherwise.
///</summary>
namespace Rubberduck.InternalApi.Model.Design
{
    internal class ShellWindowDesignViewModel : IShellWindowViewModel, INotifyPropertyChanged
    {
        public ShellWindowDesignViewModel()
        {
            Partition = Guid.NewGuid().ToString();
        }

        public IEnumerable<IDocumentTabViewModel> DocumentTabs { get; set; } = new ObservableCollection<IDocumentTabViewModel>
        {
            new DocumentTabDesignViewModel { Title = "Welcome", DocumentUri = new Uri("file://rubberduck/welcome.md"), Content = "## Welcome to Rubberduck 3.0" },
            new DocumentTabDesignViewModel { Title = "Module1", DocumentUri = new Uri("file://workspace/Module1.bas"), Content = "Option Explicit\r\n" },
        };

        public IDocumentTabViewModel? SelectedDocumentTab
        {
            get => DocumentTabs.FirstOrDefault();
            set { }
        }

        public IStatusBarViewModel StatusBar { get; set; } = new StatusBarDesignViewModel();

        public IEnumerable<ISyntaxErrorViewModel> SyntaxErrors => Enumerable.Empty<ISyntaxErrorViewModel>();

        public object? Partition { get; init; }
        public object? InterTabClient { get; init; }

        public string Title { get; } = RubberduckUI.Rubberduck;

        public event PropertyChangedEventHandler? PropertyChanged;
    }
}