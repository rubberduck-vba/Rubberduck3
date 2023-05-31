using Moq;
using System.IO.Abstractions;
//using Rubberduck.Server.Workspace;
using Rubberduck.Tests.Workspace;
using Rubberduck.Client.LSP;

namespace Rubberduck.Tests.WorkspaceServiceTests
{
    public class SetupDotRdFolderTests
    { 
        [Theory]
        [InlineData(false, false, 1, 1)]
        [InlineData(true, true, 0, 0)]
        [InlineData(false, true, 1, 0)]
        [InlineData(true, false, 0, 1)]
        public void SetupDotRdFolder_WorkspaceFolderDoesNotExist_CreatesFolders(
            bool workingExists, bool savedExists,
            int expectedCreateWorking, int expectedCreateSaved)
        {
            //Arrange
            var hostDoc = @"C:\users\duck\MyProjects\TestExcel\TestExcel.xlsm";

            var fs = WorkspaceTestsSupport
                .SetupFileSystemStubToAvoidFileNotFoundException(hostDoc);
            
            var mockWorkspaceInfo 
                = WorkspaceTestsSupport.GetWorkspaceInfoFake(hostDoc, fileSystem: fs);
            Mock.Get(mockWorkspaceInfo.WorkingRepoInfo).Setup<bool>(di => di.Exists)
                .Returns(workingExists);
            Mock.Get(mockWorkspaceInfo.SavedRepoInfo).Setup<bool>(di => di.Exists)
                .Returns(savedExists);

            //Act
            WorkspaceService.SetupDotRdFolder(mockWorkspaceInfo);

            //Assert
            Mock.Get(mockWorkspaceInfo.WorkingRepoInfo).Verify(wr => wr.Create(), 
                Times.Exactly(expectedCreateWorking));

            Mock.Get(mockWorkspaceInfo.SavedRepoInfo).Verify(wr => wr.Create(), 
                Times.Exactly(expectedCreateSaved));
        }

        [Fact]
        public void SetupDotRdFolder_WorkspaceFolderExists()
        {
            //Arrange
            var hostDoc = @"C:\users\duck\MyProjects\TestExcel\TestExcel.xlsm";

            var fs = WorkspaceTestsSupport
                .SetupFileSystemStubToAvoidFileNotFoundException(hostDoc);
            var mockWorkspaceInfo
                = WorkspaceTestsSupport.GetWorkspaceInfoFake(hostDoc, true, fs);

            //Act
            WorkspaceService.SetupDotRdFolder(mockWorkspaceInfo);

            //Assert
            Mock.Get(mockWorkspaceInfo.WorkingRepoInfo)
                .Verify(wri => wri.Create(), Times.Never);

            Mock.Get(mockWorkspaceInfo.SavedRepoInfo)
                .Verify(wri => wri.Create(), Times.Never);
        }

        [Fact]
        public void SetupDotRdFolder_DotRdFolderIsHidden()
        {
            //Arrange
            var hostDoc = @"C:\users\duck\MyProjects\TestExcel\TestExcel.xlsm";

            var fs = WorkspaceTestsSupport
                .SetupFileSystemStubToAvoidFileNotFoundException(hostDoc);
            var mockWorkspaceInfo
                = WorkspaceTestsSupport.GetWorkspaceInfoFake(hostDoc, fileSystem: fs);

            //Act
            WorkspaceService.SetupDotRdFolder(mockWorkspaceInfo);

            //Assert
            Assert.True(mockWorkspaceInfo
                .DotRdInfo.Attributes.HasFlag(FileAttributes.Hidden));
        }
    }
}