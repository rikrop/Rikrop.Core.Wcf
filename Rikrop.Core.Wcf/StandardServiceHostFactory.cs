using System;
using System.ServiceModel;

namespace Rikrop.Core.Wcf
{
    public class StandardServiceHostFactory : IServiceHostFactory
    {
        public ServiceHost CreateServiceHost(Type serviceType)
        {
            return new ServiceHost(serviceType);
        }
    }
}