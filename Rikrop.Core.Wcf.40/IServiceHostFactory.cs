using System;
using System.ServiceModel;

namespace Rikrop.Core.Wcf
{
    public interface IServiceHostFactory
    {
        ServiceHost CreateServiceHost(Type serviceType);
    }
}