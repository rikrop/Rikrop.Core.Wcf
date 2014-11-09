using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Reflection;
using System.ServiceModel.Dispatcher;
using System.Threading.Tasks;

namespace Rikrop.Core.Wcf.Security.Server
{
    internal class MethodInfoResolver
    {
        private const string AsyncMethod = "Async";
        private readonly ConcurrentDictionary<string, MethodInfo> _actionMap;
        private readonly ContractTypeResolver _contractTypeResolver;

        public MethodInfoResolver()
        {
            _contractTypeResolver = new ContractTypeResolver();
            _actionMap = new ConcurrentDictionary<string, MethodInfo>();
        }

        private static void FillMethods(Type interfaceType, IDictionary<string, MethodInfo> methods)
        {
            const BindingFlags bindingFlags = BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly;

            foreach (var method in interfaceType.GetMethods(bindingFlags))
            {
                methods.Add(GetOperationName(method), method);
            }

            foreach (var childInterface in interfaceType.GetInterfaces())
            {
                FillMethods(childInterface, methods);
            }
        }

        private static string GetOperationName(MethodInfo method)
        {
            if (method.Name.EndsWith(AsyncMethod) && typeof(Task).IsAssignableFrom(method.ReturnType))
            {
                return method.Name.Substring(0, method.Name.Length - AsyncMethod.Length);
            }
            return method.Name;
        }

        private static IDictionary<string, MethodInfo> GetMethods(Type contractType)
        {
            var methods = new Dictionary<string, MethodInfo>();
            FillMethods(contractType, methods);
            return methods;
        }

        public MethodInfo GetMethodInfo(string action, EndpointDispatcher endpointDispatcher)
        {
            MethodInfo methodInfo;
            if (!_actionMap.TryGetValue(action, out methodInfo))
            {
                var operation = endpointDispatcher.DispatchRuntime.Operations.FirstOrDefault(o => o.Action == action);

                Contract.Assume(operation != null);

                var contractType = _contractTypeResolver.GetContractType(endpointDispatcher.EndpointAddress.Uri, endpointDispatcher.ChannelDispatcher.Host.Description.Endpoints);

                methodInfo = GetMethodInfo(operation.Name, contractType);

                _actionMap.TryAdd(action, methodInfo);
            }
            return methodInfo;
        }

        public MethodInfo GetMethodInfo(string operationName, Type contractType)
        {
            MethodInfo methodInfo;
            if (!_actionMap.TryGetValue(operationName, out methodInfo))
            {
                var methods = GetMethods(contractType);

                if (!methods.TryGetValue(operationName, out methodInfo))
                {
                    throw new Exception(string.Format("В контракте {0} не найден метод с именем {1}.", contractType, operationName));
                }

                _actionMap.TryAdd(operationName, methodInfo);
            }
            return methodInfo;
        }
    }
}