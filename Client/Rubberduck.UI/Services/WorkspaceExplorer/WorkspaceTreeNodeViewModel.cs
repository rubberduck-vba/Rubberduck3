using Rubberduck.InternalApi.Model.Workspace;
using Rubberduck.UI.WorkspaceExplorer;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Rubberduck.UI.Services.WorkspaceExplorer
{
    public class WorkspaceTreeNodeViewModel : ViewModelBase, IWorkspaceUriInfo, IWorkspaceTreeNode
    {
        public static WorkspaceTreeNodeViewModel FromModel(Folder model, Uri srcRoot)
        {
            return new WorkspaceTreeNodeViewModel
            {
                Uri = new Uri(srcRoot, model.Uri),
                RelativeUri = model.Name == ProjectFile.SourceRoot ? null : new Uri(model.Name, UriKind.Relative),
                Name = model.Name,
                IsInProject = true,
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

        private Uri? _relativeUri = null!;
        public Uri? RelativeUri
        {
            get => _relativeUri;
            set
            {
                if (_relativeUri != value)
                {
                    _relativeUri = value;
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

        private readonly ObservableCollection<IWorkspaceTreeNode> _childNodes = new();
        public ObservableCollection<IWorkspaceTreeNode> ChildNodes => _childNodes;


        private int _version;
        public int Version
        {
            get => _version;
            set
            {
                if (_version != value)
                {
                    _version = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isOpen;
        public bool IsOpen
        {
            get => _isOpen;
            set
            {
                if (_isOpen != value)
                {
                    _isOpen = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isFileWatcherEnabled;
        public bool IsFileWatcherEnabled
        {
            get => _isFileWatcherEnabled;
            set
            {
                if (_isFileWatcherEnabled != value)
                {
                    _isFileWatcherEnabled = value;
                    OnPropertyChanged();
                }
            }
        }

        public IEnumerable<IWorkspaceTreeNode> Children => _childNodes;

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged();
                }
            }
        }

        public void AddChildNode(IWorkspaceTreeNode childNode)
        {
            _childNodes.Add(childNode);
        }

        private bool _isFiltered;
        public bool Filtered
        {
            get => _isFiltered;
            set
            {
                if (_isFiltered != value)
                {
                    _isFiltered = value;
                    OnPropertyChanged();
                }
            }
        }

        private bool _isExpanded;
        public bool IsExpanded
        {
            get => _isExpanded;
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
