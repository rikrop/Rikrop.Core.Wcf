namespace Rikrop.Core.Wcf.Security.Client
{
    public class SessionIdHolder : ISessionIdInitializer, ISessionIdResolver
    {
        private volatile string _sessionId;

        public string SessionId
        {
            get { return _sessionId; }
            private set { _sessionId = value; }
        }

        public void InitializeSessionId(string sessionId)
        {
            SessionId = sessionId;
        }
    }
}
