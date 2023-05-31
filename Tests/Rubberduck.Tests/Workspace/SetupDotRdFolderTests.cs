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

            var fs = DotRdFolderTestsSupport
                .SetupFileSystemStubToAvoidFileNotFoundException(hostDoc);
            
            var mockDotRdInfo 
                = DotRdFolderTestsSupport.GetDotRdFolderInfoFake(hostDoc, fileSystem: fs);
            Mock.Get(mockDotRdInfo.WorkingRepoInfo).Setup<bool>(di => di.Exists)
                .Returns(workingExists);
            Mock.Get(mockDotRdInfo.SavedRepoInfo).Setup<bool>(di => di.Exists)
                .Returns(savedExists);

            //Act
            WorkspaceService.SetupDotRdFolder(mockDotRdInfo);

            //Assert
            Mock.Get(mockDotRdInfo.WorkingRepoInfo).Verify(wr => wr.Create(), 
                Times.Exactly(expectedCreateWorking));

            Mock.Get(mockDotRdInfo.SavedRepoInfo).Verify(wr => wr.Create(), 
                Times.Exactly(expectedCreateSaved));
        }

        [Fact]
        public void SetupDotRdFolder_WorkspaceFolderExists()
        {
            //Arrange
            var hostDoc = @"C:\users\duck\MyProjects\TestExcel\TestExcel.xlsm";

            var fs = DotRdFolderTestsSupport
                .SetupFileSystemStubToAvoidFileNotFoundException(hostDoc);
            var mockDotRdInfo
                = DotRdFolderTestsSupport.GetDotRdFolderInfoFake(hostDoc, true, fs);

            //Act
            WorkspaceService.SetupDotRdFolder(mockDotRdInfo);

            //Assert
            Mock.Get(mockDotRdInfo.WorkingRepoInfo)
                .Verify(wri => wri.Create(), Times.Never);

            Mock.Get(mockDotRdInfo.SavedRepoInfo)
                .Verify(wri => wri.Create(), Times.Never);
        }

        [Fact]
        public void SetupDotRdFolder_DotRdFolderIsHidden()
        {
            //Arrange
            var hostDoc = @"C:\users\duck\MyProjects\TestExcel\TestExcel.xlsm";

            var fs = DotRdFolderTestsSupport
                .SetupFileSystemStubToAvoidFileNotFoundException(hostDoc);
            var mockDotRdInfo
                = DotRdFolderTestsSupport.GetDotRdFolderInfoFake(hostDoc, fileSystem: fs);

            //Act
            WorkspaceService.SetupDotRdFolder(mockDotRdInfo);

            //Assert
            Assert.True(mockDotRdInfo
                .DotRdInfo.Attributes.HasFlag(FileAttributes.Hidden));
        }
    }
}