using System;
using System.Collections.Generic;
using System.ServiceModel.Dispatcher;

namespace Rikrop.Core.Wcf.Security.Client
{
    public interface ISessionHeaderMessageInspectorFactory
    {
        SessionHeaderMessageInspector Create(IReadOnlyCollection<ClientOperation> clientOperations, Type contractClientType);
    }
}