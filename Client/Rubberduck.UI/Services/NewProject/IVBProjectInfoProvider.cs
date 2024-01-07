using Rubberduck.InternalApi.Model.Workspace;
using System.Collections.Generic;

namespace Rubberduck.UI.Services.NewProject
{
    public interface IVBProjectInfoProvider
    {
        IEnumerable<VBProjectInfo?> GetProjectInfo();
    }
}
