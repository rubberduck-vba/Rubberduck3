using Dragablz;
using Rubberduck.UI.Abstract;
using System;
using System.Collections;
using System.Collections.ObjectModel;
using System.Windows.Documents;

namespace Rubberduck.UI.Xaml
{
    public interface IWindowViewModel
    {
        IInterTabClient InterTabClient { get; }
        object Partition { get; }
        void ClosingTabItemHandler(object sender, EventArgs e);
    }

    public class ShellWindowViewModel : IWindowViewModel
    {
        public ShellWindowViewModel(IInterTabClient interTabClient, IStatusBarViewModel status)
        {
            Status = status;
            InterTabClient = interTabClient;
            Partition = Guid.NewGuid().ToString();

            Items = new ObservableCollection<DocumentTabViewModel>(new DocumentTabViewModel[]
            {
                new MarkdownDocumentTabViewModel(new Uri("file://rd3/welcome.md"), "Welcome", "TODO"),
            });
        }

        public ObservableCollection<DocumentTabViewModel> Items { get; init; }

        public object Partition { get; init; }
        public IInterTabClient InterTabClient { get; init; }

        public IStatusBarViewModel Status { get; init; }

        public void ClosingTabItemHandler(object sender, EventArgs e)
        {
            var item = (ChildWindowViewModel)sender;
            // TODO notify language server of closed document URI
        }
    }

    public class ChildWindowViewModel : ShellWindowViewModel
    {
        public ChildWindowViewModel(IInterTabClient interTabClient, IStatusBarViewModel status, object partition)
            : base(interTabClient, status)
        {
            Partition = partition;
        }
    }

    public abstract class DocumentTabViewModel : ViewModelBase
    {

        public DocumentTabViewModel(Uri documentUri, string title, string content)
        {
            DocumentUri = documentUri;
            Title = title;
            Content = content;
        }

        public Uri DocumentUri { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
    }

    public class TextDocumentTabViewModel : DocumentTabViewModel
    {
        public TextDocumentTabViewModel(Uri documentUri, string title, string content) 
            : base(documentUri, title, content)
        {
        }
    }

    public class MarkdownDocumentTabViewModel : DocumentTabViewModel
    {
        public MarkdownDocumentTabViewModel(Uri documentUri, string title, string content) 
            : base(documentUri, title, content)
        {
        }
    }

    public class CodeDocumentTabViewModel : DocumentTabViewModel
    {
        public CodeDocumentTabViewModel(Uri documentUri, string title, string content) : base(documentUri, title, content)
        {
        }
    }
}
