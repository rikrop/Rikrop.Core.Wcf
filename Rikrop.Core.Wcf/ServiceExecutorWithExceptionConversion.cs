using System;
using System.Diagnostics.Contracts;
using System.ServiceModel;
using System.Threading.Tasks;
using Rikrop.Core.Framework.Services;

namespace Rikrop.Core.Wcf
{
    public class ServiceExecutorWithExceptionConversion<TService> : IServiceExecutor<TService>
    {
        private readonly IServiceExecutor<TService> _serviceExecutor;
        private readonly IFaultExceptionConverter _faultExceptionConverter;

        public ServiceExecutorWithExceptionConversion(IServiceExecutor<TService> serviceExecutor, IFaultExceptionConverter faultExceptionConverter )
        {
            Contract.Requires<ArgumentNullException>(serviceExecutor != null);
            Contract.Requires<ArgumentNullException>(faultExceptionConverter != null);

            _serviceExecutor = serviceExecutor;
            _faultExceptionConverter = faultExceptionConverter;
        }

        public async Task Execute(Func<TService, Task> action)
        {
            try
            {
                await _serviceExecutor.Execute(action);
            }
            catch (FaultException ex)
            {
                Exception convertedException;
                if(_faultExceptionConverter.TryConvertException(ex, out convertedException))
                {
                    throw convertedException;
                }
                throw;
            }
            
        }

        public async Task<TResult> Execute<TResult>(Func<TService, Task<TResult>> func)
        {
            try
            {
                return await _serviceExecutor.Execute(func);
            }
            catch (FaultException ex)
            {
                Exception convertedException;
                if (_faultExceptionConverter.TryConvertException(ex, out convertedException))
                {
                    throw convertedException;
                }
                throw;
            }
        }
    }
}