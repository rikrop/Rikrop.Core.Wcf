using System;
using System.Diagnostics.Contracts;

namespace Rikrop.Core.Wcf.Security.Server
{
    public class SessionResolver<TSession> : ISessionResolver<TSession> where TSession : ISession
    {
        private readonly ISessionRepository<TSession> _sessionRepository;
        private readonly ISessionIdResolver _sessionIdResolver;

        public SessionResolver(ISessionRepository<TSession> sessionRepository, ISessionIdResolver sessionIdResolver)
        {
            _sessionRepository = sessionRepository;
            _sessionIdResolver = sessionIdResolver;
        }

        public TSession GetSession()
        {
            var sessionId = _sessionIdResolver.SessionId;
            
            Contract.Assume(sessionId != null, "Session Id not initialized.");

            TSession session;
            if (!_sessionRepository.TryGetById(sessionId, out session))
            {
                throw new InvalidOperationException(string.Format("Session with Id = {0} not found.", sessionId));
            }

            return session;
        }

        public bool HasSession()
        {
            return _sessionIdResolver.SessionId != null;
        }
    }
}