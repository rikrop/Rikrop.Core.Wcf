using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Net;
using System.Reflection;
using System.ServiceModel;

namespace Rikrop.Core.Wcf
{
    public class ServiceHostManager
    {
        private readonly IServiceConnection _serviceConnection;
        private readonly IServiceHostFactory _serviceHostFactory;

        protected Dictionary<Type, ServiceHost> Hosts { get; private set; }

        public ServiceHostManager(IServiceConnection serviceConnection, IServiceHostFactory serviceHostFactory)
            : this()
        {
            Contract.Requires<ArgumentNullException>(serviceConnection != null);
            Contract.Requires<ArgumentNullException>(serviceHostFactory != null);

            _serviceConnection = serviceConnection;
            _serviceHostFactory = serviceHostFactory;
        }

        private ServiceHostManager()
        {
            Hosts = new Dictionary<Type, ServiceHost>();
        }

        private static IEnumerable<Type> GetContractsForService(Type service)
        {
            return service.GetDirectInterfaces().Where(o => o.IsDefined(typeof (ServiceContractAttribute), true));
        }

        public static ServiceHostManager Create(string host, int port)
        {
            var serviceConnection = new NetTcpServiceConnection(new DnsEndPoint(host, port));
            var serviceHostFactory = new StandardServiceHostFactory();

            return new ServiceHostManager(serviceConnection, serviceHostFactory);
        }

        public void StartServices(Assembly assembly, IEnumerable<Type> exceptedTypes = null)
        {
            var supportedTypes = GetSupportedTypes(assembly, exceptedTypes);
            StartServices(supportedTypes.ToList());
        }

        public virtual void StartServices(IReadOnlyCollection<Type> services)
        {
            Contract.Requires<ArgumentNullException>(services != null);

            var typesWithContracts = services.ToDictionary(service => service, GetContractsForService);
            foreach (var pair in typesWithContracts)
            {
                var classType = pair.Key;
                var contractTypes = pair.Value;

                foreach (var contractType in contractTypes)
                {
                    var host = Create(classType, contractType);
                    OpenHost(host);
                    Hosts.Add(contractType, host);
                }
            }
        }


        public virtual void StopServices()
        {
            foreach (var serviceHost in Hosts.Values)
            {
                try
                {
                    CloseHost(serviceHost);
                }
                catch (Exception)
                {
                    AbortHost(serviceHost);
                }
            }
        }

        protected virtual void AddServiceEndpoint(ServiceHost host, Type contractType)
        {
            if (host != null)
            {
                var serviceEndpoint = _serviceConnection.CreateServiceEndpoint(contractType);

                host.AddServiceEndpoint(serviceEndpoint);
            }
        }

        protected virtual void OpenHost(ServiceHost host)
        {
            if (host != null)
            {
                host.Open();
            }
        }

        protected virtual void CloseHost(ServiceHost host)
        {
            if (host != null)
            {
                host.Close();
            }
        }

        protected virtual void AbortHost(ServiceHost host)
        {
            if (host != null)
            {
                host.Abort();
            }
        }

        protected virtual void OnHostCreation(ServiceHost host)
        {
        }

        protected virtual IEnumerable<Type> GetSupportedTypes(Assembly assembly, IEnumerable<Type> exceptedTypes)
        {
            exceptedTypes = exceptedTypes ?? Enumerable.Empty<Type>();

            return assembly
                .GetTypes()
                .Where(o => !o.IsInterface && !o.IsAbstract && !o.ContainsGenericParameters)
                .Where(o => GetContractsForService(o).Any())
                .Except(exceptedTypes);
        }

        private ServiceHost Create(Type serviceType, Type contractType)
        {
            var host = _serviceHostFactory.CreateServiceHost(serviceType);

            OnHostCreation(host);

            AddServiceEndpoint(host, contractType);

            return host;
        }
    }
}