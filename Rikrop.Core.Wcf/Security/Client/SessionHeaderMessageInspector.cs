using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;
using Rikrop.Core.Wcf.Security.Server;

namespace Rikrop.Core.Wcf.Security.Client
{
    public class SessionHeaderMessageInspector : IClientMessageInspector
    {
        private const string SessionNotInitializedMessage = "Не удалось добавить заголовок сессии для '{0}'.\r\nСессия не инициализирована в ISessionIdResolver'e!\r\nЗарегистрированный метод получения сессии '{1}'";

        private readonly MethodInfo _loginMethod;
        private readonly ISessionIdResolver _sessionIdResolver;
        private readonly ISessionHeaderInfo _sessionHeaderInfo;
        private readonly Dictionary<string, string> _clientOperations;
        private readonly Type _contractClientType;
        private readonly MethodInfoResolver _methodInfoResolver;

        public SessionHeaderMessageInspector(MethodInfo loginMethod,
                                             ISessionIdResolver sessionIdResolver,
                                             ISessionHeaderInfo sessionHeaderInfo,
                                             IReadOnlyCollection<ClientOperation> clientOperations,
                                             Type contractClientType)
        {
            Contract.Requires<ArgumentNullException>(loginMethod != null);
            Contract.Requires<ArgumentNullException>(sessionIdResolver != null);
            Contract.Requires<ArgumentNullException>(sessionHeaderInfo != null);
            Contract.Requires<ArgumentNullException>(clientOperations != null);
            Contract.Requires<ArgumentNullException>(contractClientType != null);

            _loginMethod = loginMethod;
            _sessionIdResolver = sessionIdResolver;
            _sessionHeaderInfo = sessionHeaderInfo;
            _clientOperations = clientOperations.ToDictionary(o => o.Action, o => o.Name);
            _contractClientType = contractClientType;

            _methodInfoResolver = new MethodInfoResolver();
        }

        public object BeforeSendRequest(ref Message request, IClientChannel channel)
        {
            string clientOperationName;
            if (!_clientOperations.TryGetValue(request.Headers.Action, out clientOperationName))
            {
                throw new InvalidOperationException(string.Format("Не найдена операция '{0}'.", request.Headers.Action));
            }

            var methodInfo = _methodInfoResolver.GetMethodInfo(clientOperationName, _contractClientType);
            if (methodInfo != _loginMethod)
            {
                var sid = _sessionIdResolver.SessionId;
                if (sid == null)
                {
                    throw new InvalidOperationException(string.Format(SessionNotInitializedMessage, request.Headers.Action, _loginMethod.Name));
                }


                var header = MessageHeader.CreateHeader(_sessionHeaderInfo.Name,
                                                        _sessionHeaderInfo.Namespace,
                                                        sid);

                request.Headers.Add(header);
            }

            return null;
        }

        public void AfterReceiveReply(ref Message reply, object correlationState)
        {
        }
    }
}