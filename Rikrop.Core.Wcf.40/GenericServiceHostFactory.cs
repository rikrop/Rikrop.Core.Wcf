using System;
using System.Diagnostics.Contracts;
using System.ServiceModel;

namespace Rikrop.Core.Wcf
{
    public class GenericServiceHostFactory : IServiceHostFactory
    {
        private readonly Func<Type, ServiceHost> _creationDelegate;

        public GenericServiceHostFactory(Func<Type, ServiceHost> creationDelegate)
        {
            Contract.Requires<ArgumentNullException>(creationDelegate != null);

            _creationDelegate = creationDelegate;
        }

        public ServiceHost CreateServiceHost(Type serviceType)
        {
            return _creationDelegate(serviceType);
        }
    }
}