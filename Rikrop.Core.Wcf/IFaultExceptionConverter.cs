using System;
using System.ServiceModel;

namespace Rikrop.Core.Wcf
{
    public interface IFaultExceptionConverter
    {
        bool TryConvertException(FaultException faultException, out Exception convertedException);
    }
}