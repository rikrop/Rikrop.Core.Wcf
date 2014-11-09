using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.ServiceModel.Description;

namespace Rikrop.Core.Wcf
{
    public class BehaviorBasedServiceHost : ServiceHost
    {
        private readonly IReadOnlyCollection<IServiceBehavior> _serviceBehaviors;

        public BehaviorBasedServiceHost(Type serviceType, IReadOnlyCollection<IServiceBehavior> serviceBehaviors)
            : base(serviceType)
        {
            Contract.Requires<ArgumentNullException>(serviceBehaviors != null);
            Contract.Requires<ArgumentException>(serviceBehaviors.Count > 0);

            _serviceBehaviors = serviceBehaviors;
        }

        protected override void OnOpening()
        {
            base.OnOpening();

            foreach (var serviceBehavior in _serviceBehaviors)
            {
                var serviceBehaviorType = serviceBehavior.GetType();
                if (Description.Behaviors.Contains(serviceBehaviorType))
                {
                    Description.Behaviors.Remove(serviceBehaviorType);
                }
                Description.Behaviors.Add(serviceBehavior);
            }
        }

        protected virtual void AddDebugBehavior()
        {
            var debugBehavior = Description.Behaviors.Find<ServiceDebugBehavior>();
            if (debugBehavior == null)
            {
                debugBehavior = new ServiceDebugBehavior();
                Description.Behaviors.Add(debugBehavior);
            }
            debugBehavior.IncludeExceptionDetailInFaults = true;
        }
    }
}