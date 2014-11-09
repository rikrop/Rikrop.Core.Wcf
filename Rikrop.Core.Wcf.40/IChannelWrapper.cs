using System;

namespace Rikrop.Core.Wcf
{
    public interface IChannelWrapper<out TChannelType> : IDisposable
    {
        TChannelType Channel { get; }
    }
}