using System;
using System.Collections.Generic;
using System.ServiceModel.Dispatcher;

namespace Rikrop.Core.Wcf.Security.Client
{
    public interface ISessionHeaderMessageInspectorFactory
    {
        IClientMessageInspector Create(IEnumerable<ClientOperation> clientOperations, Type contractClientType);
    }
}