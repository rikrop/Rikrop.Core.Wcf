using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.ServiceModel.Channels;
using System.ServiceModel.Dispatcher;

namespace Rikrop.Core.Wcf
{
    public class AggregatedErrorHandler : IErrorHandler
    {
        private readonly IEnumerable<IErrorHandler> _errorHandlers;

        public AggregatedErrorHandler(IEnumerable<IErrorHandler> errorHandlers)
        {
            Contract.Requires<ArgumentNullException>(errorHandlers != null);

            _errorHandlers = errorHandlers;
        }

        public void ProvideFault(Exception error, MessageVersion version, ref Message fault)
        {
            foreach (var errorHandler in _errorHandlers)
            {
                errorHandler.ProvideFault(error, version, ref fault);
                if(fault != null)
                {
                    return;
                }
            }
        }

        public bool HandleError(Exception error)
        {
            return _errorHandlers.Any(errorHandler => errorHandler.HandleError(error));
        }
    }
}