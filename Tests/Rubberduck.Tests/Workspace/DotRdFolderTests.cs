using Moq;
using Rubberduck.Client.LSP;
using System;
using System.Collections.Generic;
using System.IO.Abstractions;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Rubberduck.Tests.Workspace
{
    public class DotRdFolderTests
    {
        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Constructor_RaisesArgumentNullException(string argValue)
        {
            //Arrange
            //Act
            //Assert
            Assert.Throws<ArgumentNullException>(
                () => new WorkspaceInfo(argValue));
        }

        [Fact]
        public void Constructor_RaisesFileNotFoundException()
        {
            //Arrange
            var fs = Mock.Of<IFileSystem>();
            Mock.Get(fs).Setup<bool>(fs => fs.File.Exists(It.IsAny<string>()))
                .Returns(false);
            
            //Act
            //Assert
            Assert.Throws<FileNotFoundException>(
                () => new WorkspaceInfo("NotAnExistingPath.accdb", fs));
        }

        [Theory]
        [InlineData(0, "TestExcel.xlsm")]
        [InlineData(1, "TestExcel_xlsm")]
        [InlineData(2, "C:\\users\\duck\\MyProjects\\TestExcel\\.rd")]
        [InlineData(3, "C:\\users\\duck\\MyProjects\\TestExcel\\.rd\\TestExcel_xlsm\\WorkspaceFolders\\saved")]
        [InlineData(4, "C:\\users\\duck\\MyProjects\\TestExcel\\.rd\\TestExcel_xlsm\\WorkspaceFolders\\working")]
        public void DotRDFolderIDirInfoTests(int accessorIndex, string expected)
        {
            //Arrange
            var hostDoc 
                = @"C:\users\duck\MyProjects\TestExcel\TestExcel.xlsm";

            var IDirInfoTestsActualAccessors
                = new Dictionary<int, Func<WorkspaceInfo, string>>()
                {
                    [0] = (ws) => ws.HostDocumentName,
                    [1] = (ws) => ws.WorkspaceDirectoryInfo.Name,
                    [2] = (ws) => ws.DotRdInfo.FullName,
                    [3] = (ws) => ws.SavedRepoInfo.FullName,
                    [4] = (ws) => ws.WorkingRepoInfo.FullName,
                };

            var fs = WorkspaceTestsSupport
                .SetupFileSystemStubToAvoidFileNotFoundException(hostDoc);

            //Act
            var wsInfo = new WorkspaceInfo(hostDoc, fs);

            //Assert
            var actual 
                = IDirInfoTestsActualAccessors[accessorIndex](wsInfo);
            Assert.Equal(expected, actual);
        }
    }
}
