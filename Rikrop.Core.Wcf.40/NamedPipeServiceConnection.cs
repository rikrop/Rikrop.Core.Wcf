using System;
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.ServiceModel.Channels;
using System.ServiceModel.Description;
using System.Xml;

namespace Rikrop.Core.Wcf
{
    public class NamedPipeServiceConnection : IServiceConnection
    {
        private const string EndPointAddressFormat = "net.pipe://localhost/{0}/{1}/";

        private readonly NetNamedPipeBinding _networkBinding;
        private readonly string _pipeName;
        private readonly EndpointAddressBuilder _endpointAddressBuilder;

        public Binding NetworkBinding
        {
            get { return _networkBinding; }
        }

        public NamedPipeServiceConnection()
            : this("NamedPipe")
        {
        }

        public NamedPipeServiceConnection(string pipeName)
            : this(CreateDefaultBinding(), pipeName, CreateDefaultEndpointAddressBuilder())
        {
        }

        public NamedPipeServiceConnection(NetNamedPipeBinding networkBinding, string pipeName, EndpointAddressBuilder endpointAddressBuilder)
        {
            Contract.Requires<ArgumentNullException>(networkBinding != null);
            Contract.Requires<ArgumentException>(!string.IsNullOrWhiteSpace(pipeName));
            Contract.Requires<ArgumentNullException>(endpointAddressBuilder != null);

            _networkBinding = networkBinding;
            _pipeName = pipeName;
            _endpointAddressBuilder = endpointAddressBuilder;
        }

        public static NetNamedPipeBinding CreateDefaultBinding()
        {
            var result = new NetNamedPipeBinding(NetNamedPipeSecurityMode.None);
            InitializeBinding(result);
            return result;
        }

        public static EndpointAddressBuilder CreateDefaultEndpointAddressBuilder()
        {
            return new EndpointAddressBuilder();
        }

        private static void InitializeBinding(NetNamedPipeBinding binding)
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

        public virtual ServiceEndpoint CreateServiceEndpoint(Type contractType)
        {
            var contractDescription = CreateContractDescription(contractType);

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

        private string BuildEndpointAddress(Type contractType)
        {
            var contractName = ContractNameBuilder.BuildContractName(contractType);

            return string.Format(EndPointAddressFormat, _pipeName, contractName);
        }
    }
}