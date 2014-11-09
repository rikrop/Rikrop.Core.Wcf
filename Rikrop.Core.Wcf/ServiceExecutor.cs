using System;
using System.Diagnostics.Contracts;
using System.Threading.Tasks;
using Rikrop.Core.Framework.Services;

namespace Rikrop.Core.Wcf
{
    public class ServiceExecutor<TService> : IServiceExecutor<TService>
    {
        private readonly IChannelWrapperFactory<TService> _channelWrapperFactory;

        public ServiceExecutor(IChannelWrapperFactory<TService> channelWrapperFactory)
        {
            Contract.Requires<ArgumentNullException>(channelWrapperFactory != null);

            _channelWrapperFactory = channelWrapperFactory;
        }

        public static ServiceExecutor<TService> Create(string host, int port)
        {
            var channelWrapperFactory = StandardChannelWrapperFactory<TService>.Create(host, port);

            return new ServiceExecutor<TService>(channelWrapperFactory);
        }

        public async Task Execute(Func<TService, Task> action)
        {
            using (var wrapper = _channelWrapperFactory.CreateWrapper())
            {
                await action(wrapper.Channel);
            }
        }

        public async Task<TResult> Execute<TResult>(Func<TService, Task<TResult>> func)
        {
            using (var wrapper = _channelWrapperFactory.CreateWrapper())
            {
                return await func(wrapper.Channel);
            }
        }
    }
}