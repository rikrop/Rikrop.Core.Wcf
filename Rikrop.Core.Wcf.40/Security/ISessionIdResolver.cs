namespace Rikrop.Core.Wcf.Security
{
    /// <summary>
    /// Получение идентификатора текущей сессии.
    /// </summary>
    public interface ISessionIdResolver
    {
        string SessionId { get; }
    }
}
