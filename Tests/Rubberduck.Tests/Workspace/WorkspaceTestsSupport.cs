using Moq;
using Rubberduck.Client.LSP;
using System.IO.Abstractions;

namespace Rubberduck.Tests.Workspace
{
    public class WorkspaceTestsSupport
    {
        public static IWorkspaceInfo GetWorkspaceInfoFake(
            string hostDocPath, bool allPathsExist = false)
        {
            var wsInfoReal = new WorkspaceInfo(hostDocPath);

            var wsInfoFake = new WorkspaceInfo(hostDocPath, 
                dirInfoFunc: (s) => Mock.Of<IDirectoryInfo>());

            var directoryInfoPairs
                = new (IDirectoryInfo fake, IDirectoryInfo real)[]
                {
                    (wsInfoFake.DotRdInfo, wsInfoReal.DotRdInfo),
                    (wsInfoFake.WorkspaceDirectoryInfo,
                        wsInfoReal.WorkspaceDirectoryInfo),
                    (wsInfoFake.WorkingRepoInfo, wsInfoReal.WorkingRepoInfo),
                    (wsInfoFake.SavedRepoInfo, wsInfoReal.SavedRepoInfo)
                };

            foreach (var (fake, real) in directoryInfoPairs)
            {
                Mock.Get(fake).Setup<string>(di => di.Name)
                   .Returns(real.Name);

                Mock.Get(fake).Setup<string>(di => di.FullName)
                   .Returns(real.FullName);

                Mock.Get(fake).Setup<bool>(di => di.Exists)
                    .Returns(allPathsExist);
            }

            return wsInfoFake as IWorkspaceInfo;
        }
    }
}
