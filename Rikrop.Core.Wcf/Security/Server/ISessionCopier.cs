namespace Rikrop.Core.Wcf.Security.Server
{
    /// <summary>
    /// Копировальщик сессий. Предназначен для совместного использования с <a href='InMemorySessionRepository' />.
    /// </summary>
    /// <typeparam name="TSession"></typeparam>
    public interface ISessionCopier<TSession> where TSession : ISession
    {
        /// <summary>
        /// Копировать сессию.
        /// </summary>
        /// <param name="session"></param>
        /// <returns></returns>
        TSession Copy(TSession session);
    }
}