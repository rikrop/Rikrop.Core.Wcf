using System;
using System.Collections.Generic;
using System.Reflection;
using System.ServiceModel.Dispatcher;

namespace Rikrop.Core.Wcf.Security.Client
{
    public class SessionHeaderMessageInspectorFactory : ISessionHeaderMessageInspectorFactory
    {
        private readonly MethodInfo _loginMethod;
        private readonly ISessionIdResolver _sessionIdResolver;
        private readonly ISessionHeaderInfo _sessionHeaderInfo;

        public SessionHeaderMessageInspectorFactory(MethodInfo loginMethod, ISessionIdResolver sessionIdResolver, ISessionHeaderInfo sessionHeaderInfo)
        {
            _loginMethod = loginMethod;
            _sessionIdResolver = sessionIdResolver;
            _sessionHeaderInfo = sessionHeaderInfo;
        }

        public IClientMessageInspector Create(IEnumerable<ClientOperation> clientOperations, Type contractClientType)
        {
            return new SessionHeaderMessageInspector(_loginMethod, _sessionIdResolver, _sessionHeaderInfo, clientOperations, contractClientType);
        }
    }
}