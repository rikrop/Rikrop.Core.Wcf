using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Rikrop.Core.Wcf.Security.Server
{
    /// <summary>
    ///     Хранилище сессий в памяти.
    /// </summary>
    /// <typeparam name="TSession"> </typeparam>
    public class InMemorySessionRepository<TSession> : ISessionRepository<TSession>, IEnumerable<TSession>
        where TSession : ISession
    {
        private readonly ISessionCopier<TSession> _sessionCopier;
        private readonly ConcurrentDictionary<string, TSession> _sessionDic;

        public InMemorySessionRepository(ISessionCopier<TSession> sessionCopier)
        {
            _sessionCopier = sessionCopier;
            _sessionDic = new ConcurrentDictionary<string, TSession>();
        }

        public void Add(TSession session)
        {
            if (!_sessionDic.TryAdd(session.SessionId, session))
            {
                throw new InvalidOperationException("Session already exists in the repository.");
            }
        }

        public bool Delete(string sessionId)
        {
            TSession result;
            return _sessionDic.TryRemove(sessionId, out result);
        }

        public void DeleteSessionsWithLastCallLessThan(DateTime dateTime)
        {
            var sessionIdsToDelete = _sessionDic
                .Where(keyValuePair => keyValuePair.Value.LastCall < dateTime)
                .Select(keyValuePair => keyValuePair.Key);

            foreach (var sessionId in sessionIdsToDelete)
            {
                Delete(sessionId);
            }
        }

        public bool TryGetById(string sessionId, out TSession session)
        {
            TSession sessionFromRepository;
            if (_sessionDic.TryGetValue(sessionId, out sessionFromRepository))
            {
                session = _sessionCopier.Copy(sessionFromRepository);
                return true;
            }

            session = default(TSession);
            return false;
        }

        public bool Save(TSession session)
        {
            TSession sessionFromRepository;
            if (!_sessionDic.TryGetValue(session.SessionId, out sessionFromRepository))
            {
                return false;
            }

            var copy = _sessionCopier.Copy(session);
            return _sessionDic.TryUpdate(session.SessionId, copy, sessionFromRepository);
        }

        public IEnumerator<TSession> GetEnumerator()
        {
            return _sessionDic.Values.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}