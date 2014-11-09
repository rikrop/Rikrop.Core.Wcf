using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.ServiceModel.Description;

namespace Rikrop.Core.Wcf
{
    public class ServiceConnectionWithBehaviors : IServiceConnection
    {
        private readonly IEnumerable<IEndpointBehavior> _behaviors;
        private readonly IServiceConnection _connection;

        public ServiceConnectionWithBehaviors(IServiceConnection serviceConnection, IEnumerable<IEndpointBehavior> behaviors)
        {
            Contract.Requires<ArgumentNullException>(serviceConnection != null);
            Contract.Requires<ArgumentNullException>(behaviors != null);
            Contract.Requires<ArgumentException>(behaviors.Count() > 0);

            _connection = serviceConnection;
            _behaviors = behaviors;
        }

        public ServiceEndpoint CreateServiceEndpoint(Type contractType)
        {
            var result = _connection.CreateServiceEndpoint(contractType);

            foreach (var serviceBehavior in _behaviors)
            {
                result.Behaviors.Add(serviceBehavior);
            }

            return result;
        }
    }
}