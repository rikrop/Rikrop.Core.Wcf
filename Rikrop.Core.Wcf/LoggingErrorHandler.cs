using System;
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Rikrop.Core.Framework.Logging;

namespace Rikrop.Core.Wcf
{
    public class LoggingErrorHandler : IErrorHandler
    {
        private readonly ILogger _logger;

        public LoggingErrorHandler(ILogger logger)
        {
            Contract.Requires<ArgumentNullException>(logger != null);

            _logger = logger;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            var identity = Guid.NewGuid().ToString();

            error.Data.Add("Identity", identity);

            var newEx = new FaultException(String.Format("{0}, Exception Code - {1}", error.Message, identity));
            var msgFault = newEx.CreateMessageFault();
            fault = Message.CreateMessage(version, msgFault, newEx.Action);
        }

        public bool HandleError(Exception error)
        {
            _logger.LogError(error);
            return true;
        }
    }
}