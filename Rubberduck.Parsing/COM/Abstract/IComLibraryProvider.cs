using System.Runtime.InteropServices.ComTypes;
using Rubberduck.Parsing.Model.ComReflection;
using Rubberduck.VBEditor;

namespace Rubberduck.Parsing.COM.Abstract
{
    public interface IComLibraryProvider
    {
        ITypeLib LoadTypeLibrary(string libraryPath);
        IComDocumentation GetComDocumentation(ITypeLib typelib);
        ReferenceInfo GetReferenceInfo(ITypeLib typelib, string name, string path);
    }
}
