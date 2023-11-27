namespace Rubberduck.InternalApi.Model.Workspace
{
    public record class Module : File
    {
        /// <summary>
        /// Identifies the base class (supertype) for specific types of supported document modules, if applicable.
        /// </summary>
        public DocClassType? Super { get; set; }
    }
}
