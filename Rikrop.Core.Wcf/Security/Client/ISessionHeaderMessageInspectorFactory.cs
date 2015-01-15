using System;
using System.Collections.Generic;
using System.ServiceModel.Dispatcher;

namespace Rikrop.Core.Wcf.Security.Client
{
    public interface ISessionHeaderMessageInspectorFactory
    {
        IClientMessageInspector Create(IReadOnlyCollection<ClientOperation> clientOperations, Type contractClientType);
    }
}