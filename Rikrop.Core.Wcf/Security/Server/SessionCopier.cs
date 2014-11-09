namespace Rikrop.Core.Wcf.Security.Server
{
    public class SessionCopier : ISessionCopier<Session>
    {
        public Session Copy(Session session)
        {
            return new Session(session.SessionId, session.UserId) { LastCall = session.LastCall };
        }
    }
}