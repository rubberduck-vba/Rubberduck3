namespace Rubberduck.InternalApi.ServerPlatform
{
    public abstract class RpcCommandBase
    {
        public virtual string Name => GetType().Name[..^"Command".Length];
        public abstract void Execute(object? param);
    }
}
