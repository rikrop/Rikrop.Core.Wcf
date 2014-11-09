namespace Rikrop.Core.Wcf.Security.Server
{
    public interface ISessionResolver<out TSession> where TSession : ISession
    {
        TSession GetSession();
        bool HasSession();
    }
}