using System.ServiceModel;

namespace Rikrop.Core.Wcf.Security.Server
{
    public class OperationContextSessionIdHolder : ISessionIdInitializer, ISessionIdResolver
    {
        public string SessionId
        {
            get
            {
                var context = GetCurrentContext();
                return context.SessionId;
            }
        }

        public void InitializeSessionId(string sessionId)
        {
            var context = GetCurrentContext();

            context.SessionId = sessionId;
        }

        private SessionIdHolderExtension GetCurrentContext()
        {
            var context = OperationContext.Current.Extensions.Find<SessionIdHolderExtension>();
            if (context == null)
            {
                context = new SessionIdHolderExtension();
                OperationContext.Current.Extensions.Add(context);
            }
            return context;
        }
    }
}