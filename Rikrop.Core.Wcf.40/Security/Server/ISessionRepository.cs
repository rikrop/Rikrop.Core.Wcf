using System;
using System.Diagnostics.Contracts;
using Rikrop.Core.Wcf.Security.Server.Contracts;

namespace Rikrop.Core.Wcf.Security.Server
{
    /// <summary>
    ///     Хранилище сессий.
    /// </summary>
    /// <typeparam name="TSession"> </typeparam>
    [ContractClass(typeof (SessionRepositoryContracts<>))]
    public interface ISessionRepository<TSession> where TSession : ISession
    {
        void Add(TSession session);
        bool Delete(string sessionId);
        void DeleteSessionsWithLastCallLessThan(DateTime dateTime);
        bool TryGetById(string sessionId, out TSession session);
        bool Save(TSession session);
    }

    namespace Contracts
    {
        [ContractClassFor(typeof (ISessionRepository<>))]
        internal abstract class SessionRepositoryContracts<TSession> : ISessionRepository<TSession>
            where TSession : ISession
        {
            public void Add(TSession session)
            {
                throw new NotImplementedException();
            }

            public bool Delete(string sessionId)
            {
                Contract.Requires(sessionId != null);
                throw new NotImplementedException();
            }

            public void DeleteSessionsWithLastCallLessThan(DateTime dateTime)
            {
                throw new NotImplementedException();
            }

            public bool TryGetById(string sessionId, out TSession session)
            {
                Contract.Requires(sessionId != null);
                throw new NotImplementedException();
            }

            public bool Save(TSession session)
            {
                throw new NotImplementedException();
            }
        }
    }
}