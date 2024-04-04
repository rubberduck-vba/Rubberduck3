using Rubberduck.Unmanaged.Abstract.SafeComWrappers.VB;
using Rubberduck.Unmanaged.Model.Abstract;
using System.Collections.Generic;
using System.Linq;

// ReSharper disable once CheckNamespace - Special dispensation due to conflicting file vs namespace priorities
namespace Rubberduck.VBEditor.SafeComWrappers.VBA
{
    public sealed class FallbackApp : IHostApplication
    {
        public FallbackApp(IVBE vbe) { }
        public string ApplicationName => "(unknown)";
        public IEnumerable<HostDocument> GetDocuments() => Enumerable.Empty<HostDocument>();
        public HostDocument GetDocument(IQualifiedModuleName moduleName)
        {
            return null!;
        }
        public bool CanOpenDocumentDesigner(IQualifiedModuleName moduleName) => false;
        public bool TryOpenDocumentDesigner(IQualifiedModuleName moduleName) => false;
        public IEnumerable<HostAutoMacro> AutoMacroIdentifiers => Enumerable.Empty<HostAutoMacro>();
        public void Dispose() { }
    }
}
