namespace Rubberduck.Unmanaged.VBERuntime
{
    public interface IVbeSettings
    {
        DllVersion Version { get; }
        bool CompileOnDemand { get; set; }
        bool BackGroundCompile { get; set; }
    }
}