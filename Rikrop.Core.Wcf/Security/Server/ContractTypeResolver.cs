using System;
using System.Collections.Concurrent;
using System.Diagnostics.Contracts;
using System.Linq;
using System.ServiceModel.Description;

namespace Rikrop.Core.Wcf.Security.Server
{
    internal class ContractTypeResolver
    {
        private readonly ConcurrentDictionary<Uri, Type> _contractsMap;

        public ContractTypeResolver()
        {
            _contractsMap = new ConcurrentDictionary<Uri, Type>();
        }

        public Type GetContractType(Uri endpointAddressUri, ServiceEndpointCollection serviceEndpointCollection)
        {
            Type contractType;
            if (!_contractsMap.TryGetValue(endpointAddressUri, out contractType))
            {
                var contractEndpoint = serviceEndpointCollection.FirstOrDefault(o => Equals(o.Address.Uri, endpointAddressUri));

                Contract.Assume(contractEndpoint != null);

                contractType = contractEndpoint.Contract.ContractType;
                _contractsMap.TryAdd(endpointAddressUri, contractType);
            }
            return contractType;
        }
    }
}