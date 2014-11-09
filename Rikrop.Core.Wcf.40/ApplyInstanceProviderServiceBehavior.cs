using System;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Rikrop.Core.Wcf
{
    public class ApplyInstanceProviderServiceBehavior : IServiceBehavior
    {
        private readonly IInstanceProvider _instanceProvider;

        public ApplyInstanceProviderServiceBehavior(IInstanceProvider instanceProvider)
        {
            Contract.Requires<ArgumentNullException>(instanceProvider != null);

            _instanceProvider = instanceProvider;
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
                foreach (EndpointDispatcher endpointDispatcher in dispatcher.Endpoints)
                {
                    endpointDispatcher.DispatchRuntime.InstanceProvider = _instanceProvider;
                }
            }
        }
    }
}