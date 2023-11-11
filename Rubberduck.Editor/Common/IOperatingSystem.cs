namespace Rubberduck.Editor.Common
{
    public interface IOperatingSystem
    {
        void ShowFolder(string folderPath);
        WindowsVersion? GetOSVersion();
    }
}
