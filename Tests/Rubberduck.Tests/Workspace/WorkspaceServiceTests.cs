using Moq;
using Rubberduck.Client.LSP;
using Rubberduck.Tests.Workspace;
using System.IO.Abstractions;

namespace Rubberduck.Tests.WorkspaceServiceTests
{
    public class WorkspaceServiceTests
    {
        [Theory]
        [InlineData(0, "TestExcel.xlsm(Saved)", 
            @"C:\users\duck\MyProjects\TestExcel\.rd\TestExcel_xlsm\WorkspaceFolders\saved")]
        [InlineData(1, "TestExcel.xlsm(Working)",
            @"C:\users\duck\MyProjects\TestExcel\.rd\TestExcel_xlsm\WorkspaceFolders\working")]
        public void TestGetWorkspaceFolderTuples(int wsFoldersIndex, 
            string expectedName, 
            string expectedUri)
        {
            //Arrange
            var hostDocPath =
                @"C:\users\duck\MyProjects\TestExcel\TestExcel.xlsm";

            var fs = WorkspaceTestsSupport
                .SetupFileSystemStubToAvoidFileNotFoundException(hostDocPath);
            
            var wsInfoStub = WorkspaceTestsSupport
                .GetWorkspaceInfoFake(hostDocPath, fileSystem: fs);

            //Act
            var wsFolders = WorkspaceService.GetWorkspaceFolderTuples(wsInfoStub);

            //Assert
            Assert.Equal(2, wsFolders.Length);
            Assert.Equal(expectedName, wsFolders[wsFoldersIndex].Name);
            Assert.Equal(expectedUri, wsFolders[wsFoldersIndex].Folderpath);
        }
    }
}
