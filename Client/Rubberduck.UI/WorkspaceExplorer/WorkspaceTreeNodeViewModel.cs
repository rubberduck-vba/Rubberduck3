using Rubberduck.InternalApi.Model.Workspace;
using System;
using System.Collections.ObjectModel;

namespace Rubberduck.UI.WorkspaceExplorer
{
    public class WorkspaceTreeNodeViewModel : ViewModelBase
    {
        public static WorkspaceTreeNodeViewModel FromModel(Folder model)
        {
            return new WorkspaceTreeNodeViewModel
            {
                Uri = new Uri(model.Uri),
                Name = model.Name,
                IsInProject = true
            };
        }

        private string _name = null!;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        private Uri _uri = null!;
        public Uri Uri
        {
            get => _uri;
            set
            {
                if (_uri != value)
                {
                    _uri = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isInProject;
        public bool IsInProject
        {
            get => _isInProject;
            set
            {
                if (_isInProject != value)
                {
                    _isInProject = value;
                    OnPropertyChanged();
                }
            }
        }

        private readonly ObservableCollection<WorkspaceTreeNodeViewModel> _childNodes = new();
        public ObservableCollection<WorkspaceTreeNodeViewModel> ChildNodes => _childNodes;
    }
}
