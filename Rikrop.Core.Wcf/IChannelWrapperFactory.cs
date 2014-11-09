namespace Rikrop.Core.Wcf
{
    public interface IChannelWrapperFactory<out TChannel>
    {
        IChannelWrapper<TChannel> CreateWrapper();
    }
}