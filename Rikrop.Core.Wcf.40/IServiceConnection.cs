using System;
using System.ServiceModel.Description;

namespace Rikrop.Core.Wcf
{
    public interface IServiceConnection
    {
        ServiceEndpoint CreateServiceEndpoint(Type contractType);
    }
}