using System;
using System.Reflection;
using System.Threading.Tasks;

namespace Rikrop.Core.Wcf.Security.Server
{
    /// <summary>
    ///     Простейшая авторизация на основе сессий.
    /// </summary>
    public class SessionAuthStrategy<TSession> : IAuthStrategy where TSession : ISession
    {
        private readonly MethodInfo _loginMethodInfo;
        private readonly ISessionIdResolver _sessionIdResolver;
        private readonly ISessionRepository<TSession> _sessionRepository;

        public TimeSpan SessionTimeout { get; set; }

        public SessionAuthStrategy(
            MethodInfo loginMethodInfo,
            ISessionRepository<TSession> sessionRepository,
            ISessionIdResolver sessionIdResolver)
        {
            _loginMethodInfo = loginMethodInfo;
            _sessionRepository = sessionRepository;
            _sessionIdResolver = sessionIdResolver;

            SessionTimeout = TimeSpan.FromMinutes(30);
        }

        public bool CheckAccess(MethodInfo methodInfo)
        {
            if (!IsRequireCheckAccess(methodInfo))
            {
                Task.Run(() => CleanupExpiredSessionFromRepository());
                return true;
            }

            var sessionId = _sessionIdResolver.SessionId;
            var hasSession = sessionId != null;
            if (!hasSession)
            {
                return false;
            }

            TSession session;
            if (!_sessionRepository.TryGetById(sessionId, out session))
            {
                return false;
            }

            if (session.LastCall < LastCallThreshold())
            {
                return false;
            }

            session.LastCall = DateTime.Now;
            _sessionRepository.Save(session);

            return true;
        }

        private void CleanupExpiredSessionFromRepository()
        {
            _sessionRepository.DeleteSessionsWithLastCallLessThan(LastCallThreshold());
        }

        protected virtual bool IsRequireCheckAccess(MethodInfo methodInfo)
        {
            return _loginMethodInfo != methodInfo;
        }

        /// <summary>
        ///     Значение, разделяющее устаревшие и нормальные сессии.
        /// </summary>
        /// <returns></returns>
        private DateTime LastCallThreshold()
        {
            return DateTime.Now - SessionTimeout;
        }
    }
}