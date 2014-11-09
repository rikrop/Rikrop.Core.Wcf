using System;
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.ServiceModel.Channels;

namespace Rikrop.Core.Wcf.Security.Server
{
    public class AuthorizationManager : ServiceAuthorizationManager
    {
        private readonly IAuthStrategy _authStrategy;
        private readonly ISessionHeaderInfo _sessionHeaderInfo;
        private readonly ISessionIdInitializer _sessionIdInitializer;
        private readonly MethodInfoResolver _methodInfoResolver;

        public AuthorizationManager(
            IAuthStrategy authStrategy,
            ISessionHeaderInfo sessionHeaderInfo,
            ISessionIdInitializer sessionIdInitializer)
        {
            Contract.Requires<ArgumentNullException>(authStrategy != null);
            Contract.Requires<ArgumentNullException>(sessionHeaderInfo != null);
            Contract.Requires<ArgumentNullException>(sessionIdInitializer != null);

            _authStrategy = authStrategy;
            _sessionHeaderInfo = sessionHeaderInfo;
            _sessionIdInitializer = sessionIdInitializer;

            _methodInfoResolver = new MethodInfoResolver();
        }

        public override bool CheckAccess(OperationContext operationContext, ref Message message)
        {            
            string sessionId;
            if (TryGetSessionId(operationContext, out sessionId))
            {
                _sessionIdInitializer.InitializeSessionId(sessionId);
            }

            var methodInfo = _methodInfoResolver.GetMethodInfo(message.Headers.Action, operationContext.EndpointDispatcher);
            return _authStrategy.CheckAccess(methodInfo);
        }

        private bool TryGetSessionId(OperationContext operationContext, out string sessionId)
        {
            var headerIndex = operationContext.IncomingMessageHeaders
                .FindHeader(_sessionHeaderInfo.Name, _sessionHeaderInfo.Namespace);
            if (headerIndex < 0)
            {
                sessionId = null;
                return false;
            }

            sessionId = operationContext.IncomingMessageHeaders.GetHeader<string>(headerIndex);
            return true;
        }
    }
}