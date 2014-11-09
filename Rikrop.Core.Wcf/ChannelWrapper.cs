using System;
using System.Diagnostics.Contracts;
using System.ServiceModel;

namespace Rikrop.Core.Wcf
{
    public class ChannelWrapper<TChannel> : IChannelWrapper<TChannel>
    {
        private readonly TChannel _channel;
        private bool _isOpen;

        public TChannel Channel
        {
            get
            {
                if (!_isOpen)
                {
                    OpenChannel();
                }
                return _channel;
            }
        }

        public ChannelWrapper(TChannel channel)
        {
            Contract.Requires<ArgumentNullException>(!Equals(channel, null));
            Contract.Requires<ArgumentException>(channel is ICommunicationObject);
            Contract.Requires<ArgumentException>(((ICommunicationObject) channel).State != CommunicationState.Faulted);

            _channel = channel;
            _isOpen = ((ICommunicationObject) _channel).State == CommunicationState.Opened;
        }

        public void Dispose()
        {
            CloseConnection();
        }

        protected virtual void CloseConnection()
        {
            CloseChannel((ICommunicationObject) _channel);
        }

        private void CloseChannel(ICommunicationObject channel)
        {
            try
            {
                channel.Close();
            }
            catch (Exception)
            {
                channel.Abort();
            }

            _isOpen = false;
        }

        private void OpenChannel()
        {
            ((ICommunicationObject) _channel).Open();
            _isOpen = true;
        }
    }
}