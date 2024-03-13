using Microsoft.VisualStudio.TestTools.UnitTesting;
using Rubberduck.InternalApi.Extensions;
using Rubberduck.InternalApi.Model.Workspace;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SUT = Rubberduck.InternalApi.Extensions.WorkspaceFileUri;
namespace Rubberduck.Tests.DocumentUri;

[TestClass]
public class WorkspaceFileUriTests
{
    private static readonly string _workspaceRootPath = "C:\\Dev\\VBA\\Workspaces\\TestProject1";
    private static readonly Uri _workspaceRoot = new(_workspaceRootPath);

    [TestMethod]
    [TestCategory("WorkspaceUri")]
    public void GivenNullRelativeString_IsWorkspaceSourceRoot()
    {
        var expected = WorkspaceUri.SourceRootName;
        var result = new SUT(relativeUriString: null!, _workspaceRoot);

        Assert.IsFalse(result.IsAbsoluteUri);
        Assert.AreEqual(expected, result.ToString());
    }

    [TestMethod]
    [TestCategory("WorkspaceUri")]
    public void GivenAbsolutePath_StripsWorkspaceRootForRelativeUri()
    {
        var expected = "SomeFolder\\Module1.bas";
        var path = $"{_workspaceRootPath}\\{expected}";
        var result = new SUT(path, _workspaceRoot);

        Assert.IsFalse(result.IsAbsoluteUri);
        Assert.AreEqual(expected.Replace("\\", "/"), result.ToString());
    }
}
