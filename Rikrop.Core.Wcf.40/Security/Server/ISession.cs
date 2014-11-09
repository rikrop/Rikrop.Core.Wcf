using System;

namespace Rikrop.Core.Wcf.Security.Server
{
    /// <summary>
    /// Сессия.
    /// </summary>
    public interface ISession
    {
        /// <summary>
        /// Дата и время последнего вызова для данной сессии.
        /// </summary>
        DateTime LastCall { get; set; }

        /// <summary>
        /// Идентификатор сессии.
        /// </summary>
        string SessionId { get; }
    }
}