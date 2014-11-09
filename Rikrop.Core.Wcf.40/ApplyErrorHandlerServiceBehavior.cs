using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Rikrop.Core.Wcf
{
    public class ApplyErrorHandlerServiceBehavior : IServiceBehavior
    {
        private readonly IErrorHandler _errorHandler;

        public ApplyErrorHandlerServiceBehavior(IErrorHandler errorHandler)
        {
            Contract.Requires<ArgumentNullException>(errorHandler != null);

            _errorHandler = errorHandler;
        }

        public void Validate(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            
        }

        public void AddBindingParameters(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase, Collection<ServiceEndpoint> endpoints, BindingParameterCollection bindingParameters)
        {

        }

        public void ApplyDispatchBehavior(ServiceDescription serviceDescription, ServiceHostBase serviceHostBase)
        {
            foreach (ChannelDispatcher dispatcher in serviceHostBase.ChannelDispatchers)
            {
                dispatcher.ErrorHandlers.Add(_errorHandler);
            }
        }
    }
}