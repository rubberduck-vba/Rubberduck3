using Moq;
using Rubberduck.Client.LSP;
using System.IO.Abstractions;

namespace Rubberduck.Tests.Workspace
{
    public class WorkspaceTestsSupport
    {
        public static IWorkspaceInfo GetWorkspaceInfoFake(
            string hostDocPath, bool allPathsExist = false,
            IFileSystem? fileSystem = null)
        {
            var wsInfoReal = new WorkspaceInfo(hostDocPath, fileSystem);

            var wsInfoFake = new WorkspaceInfo(hostDocPath, fileSystem,
                (s) => Mock.Of<IDirectoryInfo>());

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

        public static IFileSystem SetupFileSystemStubToAvoidFileNotFoundException(string hostDoc)
        {
            var fs = Mock.Of<IFileSystem>();

            //Avoids FileNotFoundException
            Mock.Get(fs).Setup<bool>(f => f.File.Exists(hostDoc))
                .Returns(true);

            //Setup mock to return actual path data
            Mock.Get(fs).Setup<IFileInfo>(f => f.FileInfo.New(hostDoc))
                .Returns(new FileSystem().FileInfo.New(hostDoc));

            var dir = Mock.Of<IDirectory>();

#pragma warning disable CS8603, CS8604 // Possible null reference return, argument
            Mock.Get(dir).Setup<IDirectoryInfo>(d => d.GetParent(hostDoc))
                .Returns(new FileSystem().Directory.GetParent(hostDoc));
#pragma warning restore CS8603, CS8604 // Possible null reference return, argument.

            Mock.Get(fs).Setup<IDirectory>(f => f.Directory)
                .Returns(dir);

            Mock.Get(fs).Setup<IFileInfo>(f => f.FileInfo.New(hostDoc))
                .Returns(new FileSystem().FileInfo.New(hostDoc));

            return fs;
        }
    }
}
