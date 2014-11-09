using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.ServiceModel;

namespace Rikrop.Core.Wcf
{
    public class StandardChannelWrapperFactory<TChannel> : IChannelWrapperFactory<TChannel>
    {
        private readonly ChannelFactory<TChannel> _channelFactory;

        public StandardChannelWrapperFactory(IServiceConnection serviceConnection)
        {
            Contract.Requires<ArgumentNullException>(serviceConnection != null);

            _channelFactory = new ChannelFactory<TChannel>(serviceConnection.CreateServiceEndpoint(typeof (TChannel)));
        }

        public static StandardChannelWrapperFactory<TChannel> Create(string host, int port)
        {
            var serviceConnection = new NetTcpServiceConnection(new DnsEndPoint(host, port));
            return new StandardChannelWrapperFactory<TChannel>(serviceConnection);
        }

        public IChannelWrapper<TChannel> CreateWrapper()
        {
            return new ChannelWrapper<TChannel>(_channelFactory.CreateChannel());
        }
    }
}