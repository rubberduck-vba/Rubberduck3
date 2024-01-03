namespace Rubberduck.InternalApi.Common
{
    public interface IOperatingSystem
    {
        void ShowFolder(string folderPath);
        WindowsVersion? GetOSVersion();
    }
}
