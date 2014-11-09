using System;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Rikrop.Core.Framework;
using Rikrop.Core.Framework.Exceptions;

namespace Rikrop.Core.Wcf
{
    public class BusinessErrorHandler : IErrorHandler
    {
        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            var businessException = error as BusinessException;

            if (businessException == null)
            {
                return;
            }


            object details = businessException.Details;
            Type detailsType = details.GetType();
            string serializedDetails = details.Serialize();

            var faultException = new FaultException(businessException.Message,
                                                    new FaultCode(typeof(BusinessException).Name,
                                                                  businessException.GetType().AssemblyQualifiedName,
                                                                  new FaultCode(serializedDetails,
                                                                                detailsType.AssemblyQualifiedName)));

            var messageFault = faultException.CreateMessageFault();
            fault = Message.CreateMessage(version, messageFault, faultException.Action);
        }

        public bool HandleError(Exception error)
        {
            return error is BusinessException;
        }
    }
}