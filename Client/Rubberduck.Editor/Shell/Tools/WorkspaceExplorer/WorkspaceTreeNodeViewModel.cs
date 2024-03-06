using Rubberduck.InternalApi.Extensions;
using Rubberduck.UI;
using Rubberduck.UI.Shell.Tools.WorkspaceExplorer;
using System;
using System.Collections.ObjectModel;
using System.Linq;

namespace Rubberduck.Editor.Shell.Tools.WorkspaceExplorer
{
    public class WorkspaceTreeNodeViewModel : ViewModelBase, IWorkspaceTreeNode
    {
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

        private WorkspaceUri _uri = null!;
        public WorkspaceUri Uri
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

        private string _fileName = null!;
        public string FileName
        {
            get => _uri.Segments.Last();
            set
            {
                if (_fileName != value)
                {
                    var oldValue = _fileName;
                    _fileName = value;
                    OnPropertyChanged();

                    Uri = new WorkspaceFileUri(System.IO.Path.Combine(_uri.LocalPath[..^(oldValue?.Length ?? 0)], value), _uri.WorkspaceRoot);
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

        private readonly ObservableCollection<IWorkspaceTreeNode> _children = [];
        public ObservableCollection<IWorkspaceTreeNode> Children => _children;


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
            _children.Add(childNode);
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

        private bool _isEditingName;
        public bool IsEditingName
        {
            get => _isEditingName;
            set
            {
                if (_isEditingName != value)
                {
                    _isEditingName = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
