using System;
using System.ServiceModel;
using Rikrop.Core.Framework;
using Rikrop.Core.Framework.Exceptions;

namespace Rikrop.Core.Wcf
{
    public class FaultToBusinessExceptionConverter : IFaultExceptionConverter
    {
        private const string FailToCreateBusinessExceptionTypeMessage = "�� ���� ������� ��� ������ ����������: {0};\r\n�������� ����������� ������ �� ������ � �����.";
        private const string FailToCreateDetailsTypeMessage = "�� ���� ������� ��� ������� ����������: {0};\r\n�������� ����������� ������ �� ������ � �����.";


        public bool TryConvertException(FaultException faultException, out Exception convertedException)
        {
            convertedException = null;

            FaultCode businessExceptionFaultCode = faultException.Code;
            if (businessExceptionFaultCode == null || businessExceptionFaultCode.Name != typeof(BusinessException).Name)
            {
                return false;
            }
            
            FaultCode detailsFaultCode = businessExceptionFaultCode.SubCode;
            if(detailsFaultCode == null)
            {
                return false;
            }

            var businessExceptionType = Type.GetType(businessExceptionFaultCode.Namespace);
            if(businessExceptionType == null)
            {
                throw new InvalidOperationException(string.Format(FailToCreateBusinessExceptionTypeMessage, businessExceptionFaultCode.Namespace), faultException);
            }
            var detailsType = Type.GetType(detailsFaultCode.Namespace);
            if(detailsType == null)
            {
                throw new InvalidOperationException(string.Format(FailToCreateDetailsTypeMessage, detailsFaultCode.Namespace), faultException);
            }
            
            object details = detailsFaultCode.Name.Deserialize(detailsType);
            convertedException = (Exception) Activator.CreateInstance(businessExceptionType, details);

            return true;
        }
    }
}