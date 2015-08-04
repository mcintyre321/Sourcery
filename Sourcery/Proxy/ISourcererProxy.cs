namespace Sourcery.Proxy
{
    public interface ISourcererProxy
    {
        ISourcererProxy Parent { get; }
        ISourcerer RootSourcerer { get; }
        string Fragment { get; }
        object Inner { get; }
    }
}