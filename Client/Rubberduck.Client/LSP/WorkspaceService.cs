using System.IO;
using System.IO.Abstractions;

namespace Rubberduck.Client.LSP
{
    public class WorkspaceService
    {
        /// <summary>
        /// Returns an array of (string, Folderpath) tuples used
        /// to create/initialize LSP WorkspaceFolder objects
        /// </summary>
        public static (string Name, string Folderpath)[] GetWorkspaceFolderTuples(
            IDotRdFolderInfo wsInfo)
        {
            return new (string Name, string Folderpath)[]
            {
                ($"{wsInfo.HostDocumentName}(Saved)",
                    wsInfo.SavedRepoInfo.FullName),
                ($"{wsInfo.HostDocumentName}(Working)",
                    wsInfo.WorkingRepoInfo.FullName)
            };
        }

        /// <summary>
        /// Creates the .rd folder structure for a given host document.
        /// If the .rd folder and path(s) already exist, the function does nothing.
        /// </summary>
        public static void SetupDotRdFolder(IDotRdFolderInfo wsInfo)
        {
            if (!wsInfo.WorkingRepoInfo.Exists)
            {
                wsInfo.WorkingRepoInfo.Create();
            }

            if (!wsInfo.SavedRepoInfo.Exists)
            {
                wsInfo.SavedRepoInfo.Create();
            }

            if (wsInfo.DotRdInfo.Attributes != FileAttributes.Hidden)
            {
                wsInfo.DotRdInfo.Attributes = FileAttributes.Hidden;
            }
        }
    }
}
