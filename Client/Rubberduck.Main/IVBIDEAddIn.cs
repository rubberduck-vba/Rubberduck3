using System.Threading.Tasks;

namespace Rubberduck
{
    internal interface IVBIDEAddIn
    {
        /// <summary>
        /// Initializes the add-in. Any subsequent invocation should be no-op.
        /// </summary>
        /// <remarks>
        /// Some hosts connect VBE add-ins in different ways, more or less compliant with how IDTExtensibility2 intended it.
        /// </remarks>
        Task InitializeAsync();

        /// <summary>
        /// Shuts down the add-in. Any subsequent invocation should be no-op.
        /// </summary>
        /// <remarks>
        /// Some hosts disconnect VBE add-ins in different ways, more or less compliant with how IDTExtensibility2 intended it.
        /// </remarks>
        void Shutdown();
    }
}