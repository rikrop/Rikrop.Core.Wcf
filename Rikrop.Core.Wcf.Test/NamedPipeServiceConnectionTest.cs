using System;
using System.ServiceModel;
using NUnit.Framework;

namespace Rikrop.Core.Wcf.Test
{
    [TestFixture, Timeout(5000)]
    public class NamedPipeServiceConnectionTest
    {
        [ServiceContract]
        private interface IContract
        {
            [OperationContract]
            void Do();
        }

        [ServiceContract]
        private interface IGenericContract<T>
        {
            [OperationContract]
            void Do();
        }

        [ServiceBehavior(InstanceContextMode = InstanceContextMode.Single)]
        private class Service : IContract
        {
            public bool WasDo { get; private set; }

            public void Do()
            {
                WasDo = true;
            }
        }

        [Test]
        [TestCase(typeof(IContract), "net.pipe://localhost/NamedPipe/IContract/")]
        [TestCase(typeof(IGenericContract<int>), "net.pipe://localhost/NamedPipe/IGenericContract`1OfInt32/")]
        public void ShouldBuildCorrectEndpointAddress(Type contractType, string expectedEndpointAddress)
        {
            var serviceConnection = new NamedPipeServiceConnection();
            var serviceEndpoint = serviceConnection.CreateServiceEndpoint(contractType);

            Assert.AreEqual(expectedEndpointAddress, serviceEndpoint.ListenUri.ToString());
        }

        [Test]
        public void ShouldBeCorrectInRealDataTransfer()
        {
            var service = new Service();
            var serviceHost = new ServiceHost(service);

            var serviceConnection = new NamedPipeServiceConnection("TestPipeNameSeed");
            var serviceEndpoint = serviceConnection.CreateServiceEndpoint(typeof(IContract));
            serviceHost.AddServiceEndpoint(serviceEndpoint);
            serviceHost.Open();

            var serviceConnection2 = new NamedPipeServiceConnection("TestPipeNameSeed");
            var serviceEndpoint2 = serviceConnection2.CreateServiceEndpoint(typeof(IContract));
            var channelFactory = new ChannelFactory<IContract>(serviceEndpoint2);
            
            channelFactory.CreateChannel().Do();

            Assert.True(service.WasDo);
        }
    }
}