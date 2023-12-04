using Rubberduck.InternalApi.Model.Workspace;
using System;

namespace Rubberduck.UI.Services.WorkspaceExplorer
{
    public class WorkspaceSourceFileViewModel : WorkspaceFileViewModel
    {
        public static WorkspaceSourceFileViewModel FromModel(Module model)
        {
            return new WorkspaceSourceFileViewModel
            {
                Uri = new Uri(model.Uri, UriKind.Relative),
                Name = model.Name,
                IsAutoOpen = model.IsAutoOpen,
                DocumentClassType = model.Super,
                IsInProject = true
            };
        }

        private DocClassType? _documentClassType;
        public DocClassType? DocumentClassType
        {
            get => _documentClassType;
            set
            {
                if (_documentClassType != value)
                {
                    _documentClassType = value;
                    OnPropertyChanged();
                }
            }
        }
    }
}
