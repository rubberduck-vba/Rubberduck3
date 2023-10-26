using System.Threading.Tasks;

namespace Rubberduck.Main
{
    internal interface IVBIDEAddIn
    {
        /// <summary>
        /// Initializes the add-in. Any subsequent invocation should be no-op.
        /// </summary>
        /// <remarks>
        /// Some hosts connect VBE add-ins in different ways, more or less compliant with how IDTExtensibility2 intended it.
        /// </remarks>
        void Initialize();

        /// <summary>
        /// Shuts down the add-in. Any subsequent invocation should be no-op.
        /// </summary>
        /// <remarks>
        /// Some hosts disconnect VBE add-ins in different ways, more or less compliant with how IDTExtensibility2 intended it.
        /// </remarks>
        /// <param name="force">If true, forces shutdown regardless of initialization state.</param>
        void Shutdown(bool force = false);
    }
}