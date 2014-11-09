namespace Rikrop.Core.Wcf.Security
{
    public interface ISessionIdInitializer
    {
        void InitializeSessionId(string sessionId);
    }
}