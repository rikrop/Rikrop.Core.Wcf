using System;
using System.Net;
using System.ServiceModel;
using NUnit.Framework;

namespace Rikrop.Core.Wcf.Test
{
    [TestFixture]
    public class NetTcpServiceConnectionTest
    {
        [ServiceContract]
        private interface IContract
        {
             
        }

        [ServiceContract]
        private interface IGenericContract<T>
        {

        }

        [Test]
        [TestCase(typeof(IContract), "net.tcp://10.10.10.10:9000/IContract/")]
        [TestCase(typeof(IGenericContract<int>), "net.tcp://10.10.10.10:9000/IGenericContract`1OfInt32/")]
        public void ShouldBuildCorrectEndpointAddress(Type contractType, string expectedEndpointAddress)
        {
            var serviceConnection = new NetTcpServiceConnection(new DnsEndPoint("10.10.10.10", 9000));
            var serviceEndpoint = serviceConnection.CreateServiceEndpoint(contractType);

            Assert.AreEqual(expectedEndpointAddress, serviceEndpoint.ListenUri.ToString());
        }
    }


}