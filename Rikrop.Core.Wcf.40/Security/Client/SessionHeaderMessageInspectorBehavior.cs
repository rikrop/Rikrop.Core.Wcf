using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.ServiceModel.Dispatcher;

namespace Rikrop.Core.Wcf.Security.Client
{
    public class SessionHeaderMessageInspectorBehavior : IEndpointBehavior
    {
        private readonly ISessionHeaderMessageInspectorFactory _sessionHeaderInspectorFactory;

        public SessionHeaderMessageInspectorBehavior(ISessionHeaderMessageInspectorFactory sessionHeaderInspectorFactory)
        {
            _sessionHeaderInspectorFactory = sessionHeaderInspectorFactory;
        }

        public void AddBindingParameters(ServiceEndpoint endpoint, BindingParameterCollection bindingParameters)
        {
        }

        public void ApplyClientBehavior(ServiceEndpoint endpoint, ClientRuntime clientRuntime)
        {
            // todo: проверить как это работает.
            clientRuntime.MessageInspectors.Add(_sessionHeaderInspectorFactory.Create((ICollection<ClientOperation>)clientRuntime.Operations, clientRuntime.ContractClientType));
        }

        public void ApplyDispatchBehavior(ServiceEndpoint endpoint, EndpointDispatcher endpointDispatcher)
        {
        }

        public void Validate(ServiceEndpoint endpoint)
        {
        }
    }
}
