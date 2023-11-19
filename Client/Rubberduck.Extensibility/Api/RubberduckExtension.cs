namespace Rubberduck.Extensibility.Api
{
    public interface IRubberduckExtension
    {

    }

    public record class ExtensionInfo
    {
        public string Name { get; init; }
        public Version Version { get; init; }
        public Uri SourceTagUri { get; init; }
        public Uri TagAssetDownloadUri { get; init; }
    }

    public abstract class RubberduckExtension
    {
        protected abstract void OnConnect();
        protected abstract void OnDisconnect();
        public abstract bool IsConnected { get; }

        public bool IsEnabled { get; set; }
    }
}
