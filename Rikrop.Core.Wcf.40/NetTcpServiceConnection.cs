using System;
using System.Diagnostics.Contracts;
using System.Net;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace Rikrop.Core.Wcf
{
    public class NetTcpServiceConnection : IServiceConnection
    {
        private const string EndPointAddressFormat = "net.tcp://{0}:{1}/{2}/";

        private readonly NetTcpBinding _networkBinding;
        private readonly DnsEndPoint _endPoint;
        private readonly EndpointAddressBuilder _endpointAddressBuilder;

        public DnsEndPoint EndPoint
        {
            get { return _endPoint; }
        }

        public Binding NetworkBinding
        {
            get { return _networkBinding; }
        }

        public static NetTcpBinding CreateDefaultBinding()
        {
            var result = new NetTcpBinding(SecurityMode.None);
            InitializeBinding(result);
            return result;
        }

        public static EndpointAddressBuilder CreateDefaultEndpointAddressBuilder()
        {
            return new EndpointAddressBuilder();
        }

        public NetTcpServiceConnection(DnsEndPoint endPoint)
            : this(CreateDefaultBinding(), endPoint, CreateDefaultEndpointAddressBuilder())
        {
        }

        public NetTcpServiceConnection(NetTcpBinding networkBinding, DnsEndPoint endPoint, EndpointAddressBuilder endpointAddressBuilder)
        {
            Contract.Requires<ArgumentNullException>(networkBinding != null);
            Contract.Requires<ArgumentNullException>(endPoint != null);
            Contract.Requires<ArgumentException>(endPoint.Port > 0);
            Contract.Requires<ArgumentNullException>(endpointAddressBuilder != null);

            _networkBinding = networkBinding;
            _endPoint = endPoint;
            _endpointAddressBuilder = endpointAddressBuilder;
        }

        private static void InitializeBinding(NetTcpBinding binding)
        {
            binding.MaxReceivedMessageSize = 100*1024*1024;
            binding.CloseTimeout = new TimeSpan(0, 0, 10);
            binding.OpenTimeout = new TimeSpan(0, 0, 10);
            binding.ReceiveTimeout = new TimeSpan(0, 5, 0);
            binding.SendTimeout = new TimeSpan(0, 5, 0);

            InitializeReaderQuotas(binding.ReaderQuotas);
        }

        private static void InitializeReaderQuotas(XmlDictionaryReaderQuotas readerQuotas)
        {
            readerQuotas.MaxArrayLength = 100*1024*1024;
            readerQuotas.MaxBytesPerRead = 100*1024*1024;
            readerQuotas.MaxNameTableCharCount = 100*1024*1024;
            readerQuotas.MaxStringContentLength = int.MaxValue;
            readerQuotas.MaxDepth = 1024;
        }

        private string BuildEndpointAddress(Type contractType)
        {
            var contractName = ContractNameBuilder.BuildContractName(contractType);

            return string.Format(EndPointAddressFormat, _endPoint.Host, _endPoint.Port, contractName);
        }

        public virtual ServiceEndpoint CreateServiceEndpoint(Type contractType)
        {
            ContractDescription contractDescription = CreateContractDescription(contractType); 

            _endpointAddressBuilder.Uri = new Uri(BuildEndpointAddress(contractType));

            return new ServiceEndpoint(contractDescription, NetworkBinding, _endpointAddressBuilder.ToEndpointAddress());
        }

        protected virtual ContractDescription CreateContractDescription(Type contractType)
        {
            var contractDescription = ContractDescription.GetContract(contractType);

            foreach (var operation in contractDescription.Operations)
            {
                var serializerBehavior = operation.Behaviors.Find<DataContractSerializerOperationBehavior>();
                if (serializerBehavior == null)
                {
                    serializerBehavior = new DataContractSerializerOperationBehavior(operation);
                    operation.Behaviors.Add(serializerBehavior);
                }

                serializerBehavior.DataContractResolver = new SharedTypeResolver();
                serializerBehavior.MaxItemsInObjectGraph = int.MaxValue;
            }

            return contractDescription;
        }
    }
}