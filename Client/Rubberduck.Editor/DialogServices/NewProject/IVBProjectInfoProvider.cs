using Rubberduck.InternalApi.Model.Workspace;
using System.Collections.Generic;

namespace Rubberduck.Editor.DialogServices.NewProject
{
    public interface IVBProjectInfoProvider
    {
        IEnumerable<VBProjectInfo?> GetProjectInfo();
    }
}
