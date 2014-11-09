using System;
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;
using Rikrop.Core.Framework.Services;
using Timer = System.Timers.Timer;

namespace Rikrop.Core.Wcf
{
    public class ServiceExecutorWithWrapperCaching<TService> : IServiceExecutor<TService>
    {
        private readonly IChannelWrapperFactory<TService> _channelWrapperFactory;
        private readonly object _wrapperLock = new object();
        private readonly object _timerLock = new object();
        private readonly Timer _channelLifetimeTimer;

        private IChannelWrapper<TService> _wrapper;
        private volatile bool _isConnectionEstablished;
        private int _callCount;

        public ServiceExecutorWithWrapperCaching(IChannelWrapperFactory<TService> channelWrapperFactory)
            : this(channelWrapperFactory, TimeSpan.FromMinutes(1))
        {
        }

        public ServiceExecutorWithWrapperCaching(IChannelWrapperFactory<TService> channelWrapperFactory, TimeSpan channelLifetime)
        {
            Contract.Requires<ArgumentException>(channelLifetime > TimeSpan.Zero);
            Contract.Requires<ArgumentNullException>(channelWrapperFactory != null);

            _channelWrapperFactory = channelWrapperFactory;
            _channelLifetimeTimer = new Timer(channelLifetime.TotalMilliseconds) {AutoReset = false};
            _channelLifetimeTimer.Elapsed += ChannelLifetimeTimerOnElapsed;
        }

        public async Task Execute(Func<TService, Task> action)
        {
            try
            {
                BeginExecute();

                await Task.Run(() => CheckChanelConnection());

                await action(_wrapper.Channel);
            }
            catch (CommunicationException)
            {
                _isConnectionEstablished = false;
                throw;
            }
            finally
            {
                EndExecute();
            }
        }

        public async Task<TResult> Execute<TResult>(Func<TService, Task<TResult>> func)
        {
            try
            {
                BeginExecute();

                await Task.Run(() => CheckChanelConnection());

                return await func(_wrapper.Channel);
            }
            catch (CommunicationException)
            {
                _isConnectionEstablished = false;
                throw;
            }
            finally
            {
                EndExecute();
            }
        }

        private void BeginExecute()
        {
            var callCount = Interlocked.Increment(ref _callCount);
            if (callCount != 1)
            {
                return;
            }

            if (!_channelLifetimeTimer.Enabled)
            {
                return;
            }

            lock (_timerLock)
            {
                _channelLifetimeTimer.Stop();
            }
        }

        private void EndExecute()
        {
            var callCount = Interlocked.Decrement(ref _callCount);
            if (callCount != 0)
            {
                return;
            }

            if (_channelLifetimeTimer.Enabled)
            {
                return;
            }

            lock (_timerLock)
            {
                _channelLifetimeTimer.Start();
            }
        }

        private void ChannelLifetimeTimerOnElapsed(object sender, ElapsedEventArgs elapsedEventArgs)
        {
            _isConnectionEstablished = false;

            lock(_wrapperLock)
            {
                if (_wrapper != null)
                {
                    _wrapper.Dispose();
                    _wrapper = null;
                }
            }
        }

        private void OnChannelFaulted(object sender, EventArgs e)
        {
            _isConnectionEstablished = false;

            var channel = (ICommunicationObject) sender;
            channel.Faulted -= OnChannelFaulted;
        }

        private void CheckChanelConnection()
        {
            if (_isConnectionEstablished)
            {
                return;
            }

            lock(_wrapperLock)
            {
                if (_isConnectionEstablished)
                {
                    return;
                }
                if (_wrapper != null)
                {
                    _wrapper.Dispose();
                }

                _wrapper = _channelWrapperFactory.CreateWrapper();

                var channel = (ICommunicationObject) _wrapper.Channel;
                channel.Faulted += OnChannelFaulted;

                _isConnectionEstablished = true;
            }
        }
    }
}