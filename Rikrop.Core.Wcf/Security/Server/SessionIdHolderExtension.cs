using System.ServiceModel;

namespace Rikrop.Core.Wcf.Security.Server
{
    public class SessionIdHolderExtension : IExtension<OperationContext>
    {
        public string SessionId { get; set; }

        public void Attach(OperationContext owner)
        {
            
        }

        public void Detach(OperationContext owner)
        {
            
        }
    }
}