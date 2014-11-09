using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Rikrop.Core.Wcf
{
    public class BehaviorBasedServiceHostFactory : IServiceHostFactory
    {
        private readonly IEnumerable<IServiceBehavior> _serviceBehaviors;

        public BehaviorBasedServiceHostFactory(IEnumerable<IServiceBehavior> serviceBehaviors)
        {
            Contract.Requires<ArgumentNullException>(serviceBehaviors != null);
            Contract.Requires<ArgumentException>(serviceBehaviors.Count() > 0);

            _serviceBehaviors = serviceBehaviors;
        }

        public ServiceHost CreateServiceHost(Type serviceType)
        {
            return new BehaviorBasedServiceHost(serviceType, _serviceBehaviors);
        }
    }
}