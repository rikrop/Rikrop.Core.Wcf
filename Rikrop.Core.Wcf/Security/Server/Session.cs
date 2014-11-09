using System;

namespace Rikrop.Core.Wcf.Security.Server
{
    public class Session : ISession
    {
        private readonly string _sessionId;
        private readonly long _userId;

        public string SessionId
        {
            get { return _sessionId; }
        }

        public long UserId
        {
            get { return _userId; }
        }

        public DateTime LastCall { get; set; }

        public Session(string sessionId, long userId)
        {
            _sessionId = sessionId;
            _userId = userId;
        }

        public static Session Create(long userId)
        {
            var newGuid = Guid.NewGuid().ToString();
            return new Session(newGuid, userId) { LastCall = DateTime.Now };
        }
    }
}